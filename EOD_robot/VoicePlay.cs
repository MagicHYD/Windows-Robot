using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace EOD_robot
{
    class VoicePlay
    {
        enum __RENDER_FORMAT
        {
            DISPLAY_FORMAT_YV12 = 842094169,
            DISPLAY_FORMAT_YUY2 = 844715353,
            DISPLAY_FORMAT_UYVY = 1498831189,
            DISPLAY_FORMAT_A8R8G8B8 = 21,
            DISPLAY_FORMAT_X8R8G8B8 = 22,
            DISPLAY_FORMAT_RGB565 = 23,
            DISPLAY_FORMAT_RGB555 = 25,

            DISPLAY_FORMAT_RGB24_GDI = 26
        } ;


        /* 帧信息 */
        public struct EASY_FRAME_INFO
        {
            int codec;				/* 音视频格式 */

            int type;				/* 视频帧类型 */
            char fps;				/* 视频帧率 */
            short width;				/* 视频宽 */
            short height;				/* 视频高 */

            int reserved1;			/* 保留参数1 */
            int reserved2;			/* 保留参数2 */

            int sample_rate;		/* 音频采样率 */
            int channels;			/* 音频声道数 */
            int bits_per_sample;	/* 音频采样精度 */

            int length;				/* 音视频帧大小 */
            int timestamp_usec;		/* 时间戳,微妙 */
            int timestamp_sec;		/* 时间戳 秒 */

            float bitrate;			/* 比特率 */
            float losspacket;			/* 丢包率 */
        };

        const int EASY_SDK_EVENT_CODEC_ERROR = 0x63657272 ;	/* ERROR */
        const int EASY_SDK_EVENT_CODEC_EXIT = 0x65786974 ;	/* EXIT */
        const int EASY_SDK_VIDEO_FRAME_FLAG	 = 0x00000001	;	/* 视频帧标志 */
        const int EASY_SDK_AUDIO_FRAME_FLAG	 = 0x00000002	;	/* 音频帧标志 */
        const int EASY_SDK_EVENT_FRAME_FLAG =	0x00000004	;	/* 事件帧标志 */
        const int EASY_SDK_RTP_FRAME_FLAG	=	0x00000008	;	/* RTP帧标志 */
        const int EASY_SDK_SDP_FRAME_FLAG	=	0x00000010	;	/* SDP帧标志 */
        const int EASY_SDK_MEDIA_INFO_FLAG =	0x00000020	;	/* 媒体类型标志*/

        public delegate int MediaSourceCallBackDelegate(int _channelId, ref int _channelPtr, int _frameType, string pBuf, ref EASY_FRAME_INFO _frameInfo); //声明委托  

        [DllImport("libEasyPlayer-RTMP.dll", EntryPoint = "EasyPlayer_Init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int EasyPlayer_Init();

        [DllImport("libEasyPlayer-RTMP.dll", EntryPoint = "EasyPlayer_OpenStream", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int EasyPlayer_OpenStream(string url, IntPtr hWnd, int renderFormat, int rtpovertcp, string username, string password, MediaSourceCallBackDelegate callback, IntPtr userPtr, bool bHardDecode);

        [DllImport("libEasyPlayer-RTMP.dll", EntryPoint = "EasyPlayer_PlaySound", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int EasyPlayer_PlaySound(int channelId);


        [DllImport("libEasyPlayer-RTMP.dll", EntryPoint = "EasyPlayer_StopSound", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int EasyPlayer_StopSound();


        [DllImport("libEasyPlayer-RTMP.dll", EntryPoint = "EasyPlayer_SetFrameCache", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int EasyPlayer_SetFrameCache(int channelId, int cache);

         [DllImport("libEasyPlayer-RTMP.dll", EntryPoint = "EasyPlayer_CloseStream", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void EasyPlayer_CloseStream(int channelId);


         [DllImport("hfffmpeg.dll", EntryPoint = "InitRecord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
         public extern static IntPtr InitRecord(string url);


         [DllImport("hfffmpeg.dll", EntryPoint = "StartRecord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
         public extern static void StartRecord(IntPtr handle);


         [DllImport("hfffmpeg.dll", EntryPoint = "PauseRecord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
         public extern static IntPtr PauseRecord(IntPtr handle);


         [DllImport("hfffmpeg.dll", EntryPoint = "StopRecord", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
         public extern static IntPtr StopRecord(IntPtr handle);

         
        int m_ChannelId;
        MediaSourceCallBackDelegate callback = null;
        public string host = "";
        public string micro = "麦克风 (Realtek High Definition Au";


        public IntPtr recordHandle = IntPtr.Zero;
         public  void EasyPlayer_OpenStream(string url)
        {
            if(callback != null)
            {
                EasyPlayer_CloseStream();
            }
            callback = new MediaSourceCallBackDelegate(MediaSourceCallBack);
            m_ChannelId = EasyPlayer_OpenStream(url, IntPtr.Zero, 26, 1, "", "", callback, IntPtr.Zero, true);

            EasyPlayer_SetFrameCache(m_ChannelId, 0);

            EasyPlayer_PlaySound(m_ChannelId);



            startRecord();

        }
         int MediaSourceCallBack(int _channelId, ref int _channelPtr, int _frameType, string pBuf, ref EASY_FRAME_INFO _frameInfo)
        {
            if (_frameType == EASY_SDK_EVENT_FRAME_FLAG)
	        {

            }
            return 0;
        }

        public void EasyPlayer_CloseStream()
        {
            EasyPlayer_CloseStream(this.m_ChannelId);
            closeRecord();
        }

        public void   startRecord()
        {
            App.Current.Dispatcher.Invoke((Action)delegate()
            {
                if (recordHandle == IntPtr.Zero)
                {
                    recordHandle = InitRecord("rtmp://" + host + ":2017/live/test2");
                }
            
            });

            if (recordHandle != IntPtr.Zero)
            {
                StartRecord(recordHandle);
            }           
           
        }
        public void closeRecord()
        {
           /* process.Kill();
            process.Close();
            process = null;*/
            try
            {
                if (recordHandle != IntPtr.Zero)
                {
                    StopRecord(recordHandle);
                }
                recordHandle = IntPtr.Zero;
            }
            catch
            {

            }
        }

    }
}
