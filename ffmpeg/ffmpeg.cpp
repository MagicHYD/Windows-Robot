// ffmpeg.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "ffmpeg.h"


#include <string.h> 
#include <stdio.h>

#include <string> 
extern "C"
{
#include "libavformat/avformat.h"
#include "libavformat/avio.h"
#include "libavcodec/avcodec.h"
#include "libswscale/swscale.h"
#include "libavutil/avutil.h"
#include "libavutil/mathematics.h"
#include "libswresample/swresample.h"
#include "libavutil/opt.h"
#include "libavutil/channel_layout.h"
#include "libavutil/samplefmt.h"
#include "libavdevice/avdevice.h"  //摄像头所用
#include "libavfilter/avfilter.h"
#include "libavutil/error.h"
#include "libavutil/mathematics.h"  
#include "libavutil/time.h"  
#include "inttypes.h"
#include "stdint.h"
};

#pragma comment(lib,"lib/avformat.lib")
#pragma comment(lib,"lib/avcodec.lib")
#pragma comment(lib,"lib/avdevice.lib")
#pragma comment(lib,"lib/avfilter.lib")
#pragma comment(lib,"lib/avutil.lib")
#pragma comment(lib,"lib/postproc.lib")
#pragma comment(lib,"lib/swresample.lib")
#pragma comment(lib,"lib/swscale.lib")


#include <windows.h>  
#include <vector>  
#include <dshow.h>  


#ifndef MACRO_GROUP_DEVICENAME  
#define MACRO_GROUP_DEVICENAME  

#define MAX_FRIENDLY_NAME_LENGTH    128  
#define MAX_MONIKER_NAME_LENGTH     256  

typedef struct _TDeviceName
{
	WCHAR FriendlyName[MAX_FRIENDLY_NAME_LENGTH];   // 设备友好名  
	WCHAR MonikerName[MAX_MONIKER_NAME_LENGTH];     // 设备Moniker名  
} TDeviceName;
#endif  

#ifdef __cplusplus  
extern "C"
{
#endif  

	/*
	功能：获取音频视频输入设备列表
	参数说明：
	vectorDevices：用于存储返回的设备友好名及Moniker名
	guidValue：
	CLSID_AudioInputDeviceCategory：获取音频输入设备列表
	CLSID_VideoInputDeviceCategory：获取视频输入设备列表
	返回值：
	错误代码
	说明：
	基于DirectShow
	列表中的第一个设备为系统缺省设备
	capGetDriverDescription只能获得设备驱动名
	*/
	HRESULT DS_GetAudioVideoInputDevices(std::vector<TDeviceName> &vectorDevices, REFGUID guidValue);

#ifdef __cplusplus  
}
#endif  



#pragma comment(lib, "Strmiids.lib")  

HRESULT DS_GetAudioVideoInputDevices(std::vector<TDeviceName> &vectorDevices, REFGUID guidValue)
{
	TDeviceName name;
	HRESULT hr;

	// 初始化  
	vectorDevices.clear();

	// 初始化COM  
	hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
	if (FAILED(hr))
	{
		return hr;
	}

	// 创建系统设备枚举器实例  
	ICreateDevEnum *pSysDevEnum = NULL;
	hr = CoCreateInstance(CLSID_SystemDeviceEnum, NULL, CLSCTX_INPROC_SERVER, IID_ICreateDevEnum, (void **)&pSysDevEnum);
	if (FAILED(hr))
	{
		CoUninitialize();
		return hr;
	}

	// 获取设备类枚举器  
	IEnumMoniker *pEnumCat = NULL;
	hr = pSysDevEnum->CreateClassEnumerator(guidValue, &pEnumCat, 0);
	if (hr == S_OK)
	{
		// 枚举设备名称  
		IMoniker *pMoniker = NULL;
		ULONG cFetched;
		while (pEnumCat->Next(1, &pMoniker, &cFetched) == S_OK)
		{
			IPropertyBag *pPropBag;
			hr = pMoniker->BindToStorage(NULL, NULL, IID_IPropertyBag, (void **)&pPropBag);
			if (SUCCEEDED(hr))
			{
				// 获取设备友好名  
				VARIANT varName;
				VariantInit(&varName);

				hr = pPropBag->Read(L"FriendlyName", &varName, NULL);
				if (SUCCEEDED(hr))
				{
					wcscpy(name.FriendlyName, varName.bstrVal);

					// 获取设备Moniker名  
					LPOLESTR pOleDisplayName = reinterpret_cast<LPOLESTR>(CoTaskMemAlloc(MAX_MONIKER_NAME_LENGTH * 2));
					if (pOleDisplayName != NULL)
					{
						hr = pMoniker->GetDisplayName(NULL, NULL, &pOleDisplayName);
						if (SUCCEEDED(hr))
						{
							wcscpy(name.MonikerName, pOleDisplayName);
							vectorDevices.push_back(name);
						}

						CoTaskMemFree(pOleDisplayName);
					}
				}

				VariantClear(&varName);
				pPropBag->Release();
			}

			pMoniker->Release();
		} // End for While  

		pEnumCat->Release();
	}

	pSysDevEnum->Release();
	CoUninitialize();

	return hr;
}



#include <windows.h>
#define AUDIO_ID            0                             //packet 中的ID ，如果先加入音频 pocket 则音频是 0  视频是1，否则相反
#define VEDIO_ID            1
typedef struct {
	int nRet;
	AVFormatContext* icodec;
	AVInputFormat* ifmt;
	char szError[256];
	AVFormatContext* oc;
	AVOutputFormat* ofmt;
	AVStream * video_st;
	AVStream * audio_st;
	AVCodec *audio_codec;
	AVCodec *video_codec;
	double audio_pts;
	double video_pts;
	int video_stream_idx;
	int audio_stream_idx;
	AVPacket pkt;
	AVBitStreamFilterContext * vbsf_aac_adtstoasc ;

	//video param
	int m_dwWidth ;
	int m_dwHeight;
	double m_dbFrameRate;  //帧率                                                  
	AVCodecID video_codecID;
	AVPixelFormat video_pixelfromat;
	char spspps[100];
	int spspps_size;

	//audio param
	int m_dwChannelCount;      //声道
	int m_dwBitsPerSample;    //样本
	int m_dwFrequency;     //采样率
	AVCodecID audio_codecID;
	int audio_frame_size;

	int paused;
	int running;
	std::string url;
	HANDLE hThread;
}ffmpeg_data;

int InitInput(ffmpeg_data *data,WCHAR * devName, AVFormatContext ** iframe_c, AVInputFormat** iinputframe);
int InitOutput(ffmpeg_data *data);
AVStream * Add_output_stream_2(ffmpeg_data *data, AVFormatContext* output_format_context, AVMediaType codec_type_t, AVCodecID codecID, AVCodec **codec);
int OpenCodec(ffmpeg_data *data, AVStream * istream, AVCodec * icodec); //打开编解码器
void read_frame(ffmpeg_data *data, AVFormatContext *oc);    //从文件读取一帧数据根据ID读取不同的帧
void write_frame_3(ffmpeg_data *data, AVFormatContext *oc, int ID, AVPacket pkt_t); //这个是根据传过来的buf 和size 写入文件
int Es2Mux_2(ffmpeg_data *data); //通过时间戳控制写入音视频帧顺序
int UintInput(ffmpeg_data *data);
int UintOutput(ffmpeg_data *data);


static char *dup_wchar_to_utf8(WCHAR *w)
{
	char *s = NULL;
	int l = WideCharToMultiByte(CP_UTF8, 0, w, -1, 0, 0, 0, 0);
	s = (char *)new char[l];
	if (s)
		WideCharToMultiByte(CP_UTF8, 0, w, -1, s, l, 0, 0);
	return s;
}

int InitInput(ffmpeg_data *data,WCHAR * devName, AVFormatContext ** iframe_c, AVInputFormat** iinputframe)
{
	int i = 0;



	/* nRet = avformat_open_input(iframe_c, Filename, (*iinputframe), NULL);
	if (nRet != 0)
	{
	av_strerror(nRet, szError, 256);
	printf(szError);
	printf("\n");
	printf("Call avformat_open_input function failed!\n");
	return 0;
	}*/

	*iframe_c = avformat_alloc_context();
	AVInputFormat *pInputFmt = av_find_input_format("dshow");
	WCHAR *audio = L"audio=";
	int len = wcslen(devName);
	WCHAR *dev = new WCHAR[wcslen(devName) + 100];

	swprintf(dev, L"audio=%s", devName);


	char * psDevName = dup_wchar_to_utf8(dev);

	avformat_open_input(iframe_c, psDevName, pInputFmt, NULL);
	delete dev;
	delete psDevName;


	//if (av_find_stream_info(*iframe_c) < 0)
	{
		//	printf("Call av_find_stream_info function failed!\n");
		//	return 0;
	}
	//输出视频信息
	//av_dump_format(*iframe_c, -1, Filename, 0);

	//添加音频信息到输出context
	for (i = 0; i < (*iframe_c)->nb_streams; i++)
	{
		/*if ((*iframe_c)->streams[i]->codec->codec_type == AVMEDIA_TYPE_VIDEO)
		{
		video_stream_idx = i;
		m_dwWidth = (*iframe_c)->streams[i]->codec->width;
		m_dwHeight = (*iframe_c)->streams[i]->codec->height;
		m_dbFrameRate = av_q2d((*iframe_c)->streams[i]->r_frame_rate);
		video_codecID = (*iframe_c)->streams[i]->codec->codec_id;
		video_pixelfromat = (*iframe_c)->streams[i]->codec->pix_fmt;
		spspps_size = (*iframe_c)->streams[i]->codec->extradata_size;
		memcpy(spspps, (*iframe_c)->streams[i]->codec->extradata, spspps_size);
		}
		else
		*/if ((*iframe_c)->streams[i]->codec->codec_type == AVMEDIA_TYPE_AUDIO)
		{
			data->audio_stream_idx = i;
			data->m_dwChannelCount = (*iframe_c)->streams[i]->codec->channels;
			switch ((*iframe_c)->streams[i]->codec->sample_fmt)
			{
			case AV_SAMPLE_FMT_U8:
				data->m_dwBitsPerSample = 8;
				break;
			case AV_SAMPLE_FMT_S16:
				data->m_dwBitsPerSample = 16;
				break;
			case AV_SAMPLE_FMT_S32:
				data->m_dwBitsPerSample = 32;
				break;
			default:
				break;
			}
			data->m_dwFrequency = (*iframe_c)->streams[i]->codec->sample_rate;
			data->audio_codecID = (*iframe_c)->streams[i]->codec->codec_id;
			data->audio_frame_size = (*iframe_c)->streams[i]->codec->frame_size;
		}
	}
	return 1;
}

int InitOutput(ffmpeg_data *data)
{
	int i = 0;
	/* allocate the output media context */

	avformat_alloc_output_context2(&data->oc, NULL, "flv", data->url.c_str());
	if (!data->oc)
	{
		return -1;
	}
	data->ofmt = data->oc->oformat;

	/* open the output file, if needed */
	if (!(data->ofmt->flags & AVFMT_NOFILE))
	{
		if (avio_open(&data->oc->pb, data->url.c_str(), AVIO_FLAG_WRITE) < 0)
		{
			printf("Could not open '%s'\n", data->url);
			return -1;
		}
	}

	//添加音频信息到输出context
	if (data->audio_stream_idx != -1)
	{
		data->ofmt->audio_codec = data->audio_codecID;
		data->audio_st = Add_output_stream_2(data, data->oc, AVMEDIA_TYPE_AUDIO, data->audio_codecID, &data->audio_codec);
	}
	/*
	//添加视频信息到输出context
	ofmt->video_codec = video_codecID;
	video_st = Add_output_stream_2(oc, AVMEDIA_TYPE_VIDEO, video_codecID, &video_codec);

	if (OpenCodec(video_st, video_codec) < 0)   //打开视频编码器
	{
	printf("can not open video codec\n");
	return getchar();
	}
	*/
	if (data->audio_stream_idx != -1)
	{
		if (OpenCodec(data,data->audio_st, data->audio_codec) < 0)   //打开音频编码器
		{
			printf("can not open audio codec\n");
			return -1;
		}
	}

	av_dump_format(data->oc, 0, data->url.c_str(), 1);

	if (avformat_write_header(data->oc, NULL))
	{
		printf("Call avformat_write_header function failed.\n");
		return -1;
	}

	if (data->audio_stream_idx != -1)
	{
		if ((strstr(data->oc->oformat->name, "flv") != NULL) ||
			(strstr(data->oc->oformat->name, "mp4") != NULL) ||
			(strstr(data->oc->oformat->name, "mov") != NULL) ||
			(strstr(data->oc->oformat->name, "3gp") != NULL))
		{
			if (data->audio_st->codec->codec_id == AV_CODEC_ID_AAC) //AV_CODEC_ID_AAC
			{
				data->vbsf_aac_adtstoasc = av_bitstream_filter_init("aac_adtstoasc");
			}
		}
	}
	if (data->vbsf_aac_adtstoasc == NULL)
	{
		return 0;
	}
	return 0;
}

AVStream * Add_output_stream_2(ffmpeg_data *data, AVFormatContext* output_format_context, AVMediaType codec_type_t, AVCodecID codecID, AVCodec **codec)
{
	AVCodecContext* output_codec_context = NULL;
	AVStream * output_stream = NULL;

	/* find the encoder */
	*codec = avcodec_find_encoder(codecID);
	if (!(*codec))
	{
		return NULL;
	}
	output_stream = avformat_new_stream(output_format_context, *codec);
	if (!output_stream)
	{
		return NULL;
	}

	output_stream->id = output_format_context->nb_streams - 1;
	output_codec_context = output_stream->codec;
	output_codec_context->codec_id = codecID;
	output_codec_context->codec_type = codec_type_t;

	switch (codec_type_t)
	{
	case AVMEDIA_TYPE_AUDIO:
		AVRational CodecContext_time_base;
		CodecContext_time_base.num = 1;
		CodecContext_time_base.den = data->m_dwFrequency;
		output_stream->time_base = CodecContext_time_base;
		output_codec_context->time_base = CodecContext_time_base;
		output_stream->start_time = 0;
		output_codec_context->sample_rate = data->m_dwFrequency;
		output_codec_context->channels = data->m_dwChannelCount;
		output_codec_context->frame_size = data->audio_frame_size;
		switch (data->m_dwBitsPerSample)
		{
		case 8:
			output_codec_context->sample_fmt = AV_SAMPLE_FMT_U8;
			break;
		case 16:
			output_codec_context->sample_fmt = AV_SAMPLE_FMT_S16;
			break;
		case 32:
			output_codec_context->sample_fmt = AV_SAMPLE_FMT_S32;
			break;
		default:
			break;
		}
		output_codec_context->block_align = 0;
		if (!strcmp(output_format_context->oformat->name, "mp4") ||
			!strcmp(output_format_context->oformat->name, "mov") ||
			!strcmp(output_format_context->oformat->name, "3gp") ||
			!strcmp(output_format_context->oformat->name, "flv"))
		{
			output_codec_context->flags |= AV_CODEC_FLAG_GLOBAL_HEADER;
		}
		break;
	case AVMEDIA_TYPE_VIDEO:
		AVRational r_frame_rate_t;
		r_frame_rate_t.num = 1000;
		r_frame_rate_t.den = (int)(data->m_dbFrameRate * 1000);
		output_stream->time_base = r_frame_rate_t;
		output_codec_context->time_base = r_frame_rate_t;

		AVRational r_frame_rate_s;
		r_frame_rate_s.num = (int)(data->m_dbFrameRate * 1000);
		r_frame_rate_s.den = 1000;
		output_stream->r_frame_rate = r_frame_rate_s;

		output_stream->start_time = 0;
		output_codec_context->pix_fmt = data->video_pixelfromat;
		output_codec_context->width = data->m_dwWidth;
		output_codec_context->height = data->m_dwHeight;
		output_codec_context->extradata = (uint8_t *)data->spspps;
		output_codec_context->extradata_size = data->spspps_size;
		//这里注意不要加头，demux的时候 h264filter过滤器会改变文件本身信息
		//这里用output_codec_context->extradata 来显示缩略图
		//if(! strcmp( output_format_context-> oformat-> name,  "mp4" ) ||
		//	!strcmp (output_format_context ->oformat ->name , "mov" ) ||
		//	!strcmp (output_format_context ->oformat ->name , "3gp" ) ||
		//	!strcmp (output_format_context ->oformat ->name , "flv" ))
		//{
		//	output_codec_context->flags |= CODEC_FLAG_GLOBAL_HEADER;
		//}
		break;
	default:
		break;
	}
	return output_stream;
}

int OpenCodec(ffmpeg_data *data, AVStream * istream, AVCodec * icodec)
{
	AVCodecContext *c = istream->codec;
	data->nRet = avcodec_open2(c, icodec, NULL);
	return data->nRet;
}

int UintInput(ffmpeg_data *data)
{
	/* free the stream */
	if (data->icodec == NULL)
	{
		return 0;
	}
	av_free(data->icodec);
	data->icodec = NULL;
	return 1;
}

int UintOutput(ffmpeg_data *data)
{
	if (data->oc == NULL)
	{
		return 0; 
	}
	int i = 0;
	data->nRet = av_write_trailer(data->oc);
	if (data->nRet < 0)
	{
		av_strerror(data->nRet, data->szError, 256);
		printf(data->szError);
		printf("\n");
		printf("Call av_write_trailer function failed\n");
	}
	if (data->vbsf_aac_adtstoasc != NULL)
	{
		av_bitstream_filter_close(data->vbsf_aac_adtstoasc);
		data->vbsf_aac_adtstoasc = NULL;
	}
	av_dump_format(data->oc, -1, data->url.c_str(), 1);
	/* Free the streams. */
	for (i = 0; i < data->oc->nb_streams; i++)
	{
		av_freep(&data->oc->streams[i]->codec);
		av_freep(&data->oc->streams[i]);
	}
	if (!(data->ofmt->flags & AVFMT_NOFILE))
	{
		/* Close the output file. */
		avio_close(data->oc->pb);
	}
	av_free(data->oc);
	data->oc = NULL;
	return 1;
}

void read_frame(ffmpeg_data *data, AVFormatContext *oc)
{
	int ret = 0;
	if (oc == NULL)
	{
		return;
	}
	if (data->running == 0)
	{
		return;
	}
	memset(&data->pkt, 0x00, sizeof(data->pkt));
	ret = av_read_frame(data->icodec, &data->pkt);
	if (!data->pkt.buf)
	{
		return;
	}
	if (data->nRet != 0)
	{
		return;
	}
	/*if (pkt.stream_index == video_stream_idx)
	{
	write_frame_3(oc, VEDIO_ID, pkt);
	}
	else */
	if (data->pkt.stream_index == data->audio_stream_idx)
	{
		write_frame_3(data, data->oc, AUDIO_ID, data->pkt);
	}
}

void write_frame_3(ffmpeg_data *data, AVFormatContext *oc, int ID, AVPacket pkt_t)
{
	if (data->running == 0)
	{
		return;
	}
	int64_t pts = 0, dts = 0;
	int nRet = -1;
	AVRational time_base_t;
	time_base_t.num = 1;
	time_base_t.den = 1000;

	if (ID == VEDIO_ID)
	{
		AVPacket videopacket_t;
		av_init_packet(&videopacket_t);

		if (av_dup_packet(&videopacket_t) < 0)
		{
			av_free_packet(&videopacket_t);
		}

		videopacket_t.pts = pkt_t.pts;
		videopacket_t.dts = pkt_t.dts;
		videopacket_t.pos = 0;
//		videopacket_t.priv = 0;
		videopacket_t.flags = 1;
		videopacket_t.convergence_duration = 0;
		videopacket_t.side_data_elems = 0;
		videopacket_t.stream_index = VEDIO_ID;
		videopacket_t.duration = 0;
		videopacket_t.data = pkt_t.data;
		videopacket_t.size = pkt_t.size;
		nRet = av_interleaved_write_frame(oc, &videopacket_t);
		if (nRet != 0)
		{
			printf("error av_interleaved_write_frame _ video\n");
		}
		av_free_packet(&videopacket_t);
	}
	else if (ID == AUDIO_ID)
	{
		AVPacket audiopacket_t;
		av_init_packet(&audiopacket_t);

		if (av_dup_packet(&audiopacket_t) < 0)
		{
			av_free_packet(&audiopacket_t);
		}

		audiopacket_t.pts = pkt_t.pts;
		audiopacket_t.dts = pkt_t.dts;
		audiopacket_t.pos = 0;
//		audiopacket_t.priv = 0;
		audiopacket_t.flags = 1;
		audiopacket_t.duration = 0;
		audiopacket_t.convergence_duration = 0;
		audiopacket_t.side_data_elems = 0;
		audiopacket_t.stream_index = AUDIO_ID;
		audiopacket_t.duration = 0;
		audiopacket_t.data = pkt_t.data;
		audiopacket_t.size = pkt_t.size;

		//添加过滤器
		if (!strcmp(oc->oformat->name, "mp4") ||
			!strcmp(oc->oformat->name, "mov") ||
			!strcmp(oc->oformat->name, "3gp") ||
			!strcmp(oc->oformat->name, "flv"))
		{
			if (data->audio_st->codec->codec_id == AV_CODEC_ID_AAC)
			{
				if (data->vbsf_aac_adtstoasc != NULL)
				{
					AVPacket filteredPacket = audiopacket_t;
					int a = av_bitstream_filter_filter(data->vbsf_aac_adtstoasc,
						data->audio_st->codec, NULL, &filteredPacket.data, &filteredPacket.size,
						audiopacket_t.data, audiopacket_t.size, audiopacket_t.flags & AV_PKT_FLAG_KEY);
					if (a >  0)
					{
						av_free_packet(&audiopacket_t);
						//filteredPacket.destruct = av_destruct_packet;
						audiopacket_t = filteredPacket;
					}
					else if (a == 0)
					{
						audiopacket_t = filteredPacket;
					}
					else if (a < 0)
					{
						fprintf(stderr, "%s failed for stream %d, codec %s",
							data->vbsf_aac_adtstoasc->filter->name, audiopacket_t.stream_index, data->audio_st->codec->codec ? data->audio_st->codec->codec->name : "copy");
						av_free_packet(&audiopacket_t);

					}
				}
			}
		}
		nRet = av_interleaved_write_frame(oc, &audiopacket_t);
		if (nRet != 0)
		{
			printf("error av_interleaved_write_frame _ audio\n");
			if (data->running)
			{
				data->paused = 1;
				UintOutput(data);
				InitOutput(data);
				data->paused = 0;
			}
		}
		else{
			printf("sucess av_interleaved_write_frame _ audio\n");
		}
		av_free_packet(&audiopacket_t);
	}
}

int Es2Mux_2(ffmpeg_data *data)
{
	while (data->running)
	{
		if (data->paused)
		{
			Sleep(10);
			continue;
		}
		read_frame(data, data->oc);
 	}
	return 1;
}


// 这是导出函数的一个示例。
FFMPEG_API HANDLE InitRecord(const char* url)
{

	std::vector<TDeviceName> name;
	DS_GetAudioVideoInputDevices(name, CLSID_AudioInputDeviceCategory);
	if (name.size() < 1) return  NULL;

	ffmpeg_data * ret = new ffmpeg_data;
	memset(ret, 0x00, sizeof(ffmpeg_data));
	ret->video_stream_idx = -1;
	ret->audio_stream_idx = -1;
	ret->m_dbFrameRate = 25.0;  //帧率                                                  
	ret->video_codecID = AV_CODEC_ID_H264;
	ret->video_pixelfromat = AV_PIX_FMT_YUV420P;

	//audio param
	ret->m_dwChannelCount = 2;      //声道
	ret->m_dwBitsPerSample = 16;    //样本
	ret->m_dwFrequency = 44100;     //采样率
	ret->audio_codecID = AV_CODEC_ID_AAC;
	ret->audio_frame_size = 1024;
	ret->url = url;
	ret->running = 1;
	av_register_all();
	avdevice_register_all();

	avformat_network_init();

	//打开输入设备，

	
	InitInput(ret, name.at(0).FriendlyName, &ret->icodec, &ret->ifmt);


	if (InitOutput(ret) != 0)
	{
		delete ret;
		return NULL;
	}	 
 
	return ret;
}



DWORD WINAPI ThreadProFunc(LPVOID lpParam)
{
	ffmpeg_data * ret = (ffmpeg_data *)lpParam;
	ret->running = 1;
	Es2Mux_2(ret);	
	CloseHandle(ret->hThread);	//关闭线程句柄，内核引用计数减一
	ret->hThread = NULL;
	return 0;
}
FFMPEG_API void StartRecord(HANDLE handle)
{
	ffmpeg_data * ret = (ffmpeg_data *)handle;
	if (ret->hThread)
	{
		ret->running = 0;
		Sleep(20);
		if (ret->hThread)
		{
			TerminateThread(ret->hThread, 0);
			CloseHandle(ret->hThread);
		}
	}

	DWORD dwThreadId;
	ret->hThread = CreateThread(NULL	// 默认安全属性
		, NULL		// 默认堆栈大小
		, ThreadProFunc // 线程入口地址
		, handle	//传递给线程函数的参数
		, 0		// 指定线程立即运行
		, &dwThreadId	//线程ID号
		);
	
}


FFMPEG_API void PauseRecord(HANDLE handle)
{
	ffmpeg_data * ret = (ffmpeg_data *)handle;
	ret->paused = 0;
}


FFMPEG_API void StopRecord(HANDLE handle)
{
	ffmpeg_data * ret = (ffmpeg_data *)handle;
	ret->running = 0;
	Sleep(15);
	ret->running = 0;
	UintOutput(ret);
	UintInput(ret);

	for (int i = 0; i < 100; i++)
	{
		if (ret->hThread == NULL)
		{
			break;
		}
		Sleep(10);
	}
	memset(ret, 0x00, sizeof(ffmpeg_data));
	delete ret;
}

