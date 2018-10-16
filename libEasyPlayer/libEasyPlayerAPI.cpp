/*
	Copyright (c) 2013-2014 EasyDarwin.ORG.  All rights reserved.
	Github: https://github.com/EasyDarwin
	WEChat: EasyDarwin
	Website: http://www.easydarwin.org
	Author: Sword@easydarwin.org
*/
#include "libEasyPlayerAPI.h"
#include "ChannelManager.h"

CChannelManager	*g_pChannelManager = NULL;

#define KEY "59617A414C5969576B5A75416D7942617064396A4575784659584E3555477868655756794C564A55545641755A58686C567778576F50365334456468646D6C754A6B4A68596D397A595541794D4445325257467A65555268636E6470626C526C5957316C59584E35"

// ��ʼ��SDK
LIB_EASYPLAYER_API int  EasyPlayer_Init()
{
	int isEasyRTSPClientActivated = EasyRTMPClient_Activate(KEY);
	switch(isEasyRTSPClientActivated)
	{
	case EASY_ACTIVATE_INVALID_KEY:
		printf("EasyRTMPClient  is EASY_ACTIVATE_INVALID_KEY!\n");
		break;
	case EASY_ACTIVATE_TIME_ERR:
		printf("EasyRTMPClient is EASY_ACTIVATE_TIME_ERR!\n");
		break;
	case EASY_ACTIVATE_PROCESS_NAME_LEN_ERR:
		printf("EasyRTMPClient is EASY_ACTIVATE_PROCESS_NAME_LEN_ERR!\n");
		break;
	case EASY_ACTIVATE_PROCESS_NAME_ERR:
		printf("EasyRTMPClient is EASY_ACTIVATE_PROCESS_NAME_ERR!\n");
		break;
	case EASY_ACTIVATE_VALIDITY_PERIOD_ERR:
		printf("EasyRTMPClient is EASY_ACTIVATE_VALIDITY_PERIOD_ERR!\n");
		break;
	case EASY_ACTIVATE_SUCCESS:
		printf("EasyRTMPClient is EASY_ACTIVATE_SUCCESS!\n");
		break;
	}

	if(EASY_ACTIVATE_SUCCESS != isEasyRTSPClientActivated)
		return -1;

	if (NULL == g_pChannelManager)
	{
		g_pChannelManager = new CChannelManager();
		g_pChannelManager->Initial();
	}

	if (NULL == g_pChannelManager)		return -1;

	return 0;
}

// Release
LIB_EASYPLAYER_API void  EasyPlayer_Release()
{
	if (NULL != g_pChannelManager)
	{
		delete g_pChannelManager;
		g_pChannelManager = NULL;
	}
}


LIB_EASYPLAYER_API int  EasyPlayer_OpenStream(const char *url, HWND hWnd, RENDER_FORMAT renderFormat, int rtpovertcp,const char *username, const char *password, MediaSourceCallBack callback, void *userPtr, bool bHardDecode)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->OpenStream(url, hWnd, renderFormat, rtpovertcp, username, password, callback, userPtr, bHardDecode);
}

LIB_EASYPLAYER_API void  EasyPlayer_CloseStream(int channelId)
{
	if (NULL == g_pChannelManager)		return;

	g_pChannelManager->CloseStream(channelId);
}

LIB_EASYPLAYER_API int  EasyPlayer_SetFrameCache(int channelId, int cache)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetFrameCache(channelId, cache);
}
LIB_EASYPLAYER_API int  EasyPlayer_SetShownToScale(int channelId, int shownToScale)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetShownToScale(channelId, shownToScale);
}

LIB_EASYPLAYER_API int  EasyPlayer_SetDecodeType(int channelId, int decodeKeyframeOnly)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetDecodeType(channelId, decodeKeyframeOnly);
}
LIB_EASYPLAYER_API int  EasyPlayer_SetRenderRect(int channelId, LPRECT lpSrcRect)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetRenderRect(channelId, lpSrcRect);
}

LIB_EASYPLAYER_API int  EasyPlayer_ShowStatisticalInfo(int channelId, int show)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->ShowStatisticalInfo(channelId, show);
}

LIB_EASYPLAYER_API int  EasyPlayer_ShowOSD(int channelId, int show, EASY_PALYER_OSD osd)
{

	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->ShowOSD(channelId, show,osd);
}


LIB_EASYPLAYER_API int  EasyPlayer_SetDragStartPoint(int channelId, POINT pt)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetDragStartPoint(channelId, pt);
}
LIB_EASYPLAYER_API int  EasyPlayer_SetDragEndPoint(int channelId, POINT pt)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetDragEndPoint(channelId, pt);
}
LIB_EASYPLAYER_API int  EasyPlayer_ResetDragPoint(int channelId)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->ResetDragPoint(channelId);
}


LIB_EASYPLAYER_API int  EasyPlayer_PlaySound(int channelId)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->PlaySound(channelId);
}
LIB_EASYPLAYER_API int  EasyPlayer_StopSound()
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->StopSound();
}

LIB_EASYPLAYER_API int  EasyPlayer_StartManuRecording(int channelId)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->StartManuRecording(channelId);
}

LIB_EASYPLAYER_API int  EasyPlayer_StopManuRecording(int channelId)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->StopManuRecording(channelId);
}

LIB_EASYPLAYER_API int		EasyPlayer_SetManuRecordPath(int channelId, const char* recordPath)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetManuRecordPath(channelId, recordPath);
}

LIB_EASYPLAYER_API int		EasyPlayer_SetManuPicShotPath(int channelId, const char* shotPath)
{
	if (NULL == g_pChannelManager)		return -1;

	return g_pChannelManager->SetManuPicShotPath(channelId, shotPath);
}

LIB_EASYPLAYER_API int		EasyPlayer_StartManuPicShot(int channelId)
{
	if (NULL == g_pChannelManager)		return -1;
	return g_pChannelManager->StartManuPicShot(channelId);
}

LIB_EASYPLAYER_API int		EasyPlayer_StopManuPicShot(int channelId)
{
	if (NULL == g_pChannelManager)		return -1;
	return g_pChannelManager->StopManuPicShot(channelId);
}
