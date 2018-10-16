// ���� ifdef ���Ǵ���ʹ�� DLL �������򵥵�
// ��ı�׼�������� DLL �е������ļ��������������϶���� FFMPEG_EXPORTS
// ���ű���ġ���ʹ�ô� DLL ��
// �κ�������Ŀ�ϲ�Ӧ����˷��š�������Դ�ļ��а������ļ����κ�������Ŀ���Ὣ
// FFMPEG_API ������Ϊ�Ǵ� DLL ����ģ����� DLL ���ô˺궨���
// ������Ϊ�Ǳ������ġ�
#pragma once
#ifdef FFMPEG_EXPORTS
#define FFMPEG_API __declspec(dllexport)
#else
#define FFMPEG_API __declspec(dllimport)
#endif
#ifdef __cplusplus  
extern "C"
{
#endif  
 
FFMPEG_API HANDLE InitRecord(const char* url);
FFMPEG_API void StartRecord(HANDLE handle);
FFMPEG_API void PauseRecord(HANDLE handle);
FFMPEG_API void StopRecord(HANDLE handle);

#ifdef __cplusplus  
}
#endif 