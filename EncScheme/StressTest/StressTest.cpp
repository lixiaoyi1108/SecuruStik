#include<windows.h>
#include <conio.h>
#include <process.h>
#include<stdio.h>
#include<stdlib.h>
#include <io.h>
#include<direct.h>

int System_StatusTest()
{
	FILE *fp = fopen("RunningStatus.txt","a");
	if(NULL == fp)return 1;
	MEMORYSTATUSEX *ms = new MEMORYSTATUSEX();
	ms->dwLength = (UINT)sizeof(MEMORYSTATUSEX);
	GlobalMemoryStatusEx(ms);
	fprintf(fp,"Total physical mem      : %0.2f MB\r\n",(float)(ms->ullTotalPhys/1024/1024));
	fprintf(fp,"Availible physical mem  : %0.2f MB\r\n",(float)(ms->ullAvailPhys/1024/1024));
	fprintf(fp,"Memory Usage            : %0.2f %\r\n",(float)(ms->dwMemoryLoad));
	//fprintf(fp,"Total Virtual mem       : %0.2f MB\r\n",(float)(ms->ullTotalVirtual/1024/1024));
	//fprintf(fp,"Availible physical mem  : %0.2f MB\r\n",(float)(ms->ullAvailVirtual/1024/1024));
	fprintf(fp,"===================================================\r\n");
	delete(ms);
	fclose(fp);
	return 0;
}
void nstrcat(char* dest,...)
{
	va_list     ap;   
	char        *p;   

	va_start(ap, dest);
	while( (p = va_arg(ap, char *)) != NULL ){
		strcat(dest, p);   
		dest += strlen(p);
	}     

	va_end(ap);  
}
#define runTime 100
int haveFinished = 0;
DWORD WINAPI stressTest(LPVOID lpParam)
{
	char *cmd = (char*)malloc(1024);memset(cmd,0,1024);

	char *cwd = getcwd(NULL,0);
	/* cd __Current_Dir__
	 * cd ..
	 * cd Debug/SecuruStik/bin
	 * PRE.exe
	 */
	nstrcat(cmd,"cd ",cwd,"&&","cd .. && cd ProxyReEncryption/bin && PRE.exe");
	system(cmd);
	free(cwd);
	haveFinished++;
	return 0;
}
int main()
{
	System_StatusTest();

	HANDLE hThread[runTime];
	//LPDWORD dwThreadId[runTime];
	for( int i = 0 ; i < runTime ; i++ )
	{
		hThread[i]=CreateThread(
			NULL,//default security attributes
			0,//use default stacksize
			stressTest,//thread function
			NULL,//argument to thread function
			0,//use default creation flags
			NULL);//returns the thread id
		//Checkthereturnvalueforsuccess.
		if( hThread[i] == NULL )
		{
			ExitProcess(i);
			printf("Thread created failed!!\n");
		}
		else
		{
			printf("Thread %d is created!!\n",hThread[i]);
		}

	}
	//Wait until all threads have terminated.
	WaitForMultipleObjects(runTime,hThread,TRUE,INFINITE);
	//Close all thread hand les upon completion.
	for( int i=0 ; i < runTime; i++ )
	{
		CloseHandle(hThread[i]);
	}
	while( haveFinished < 100 ){
		System_StatusTest();
		Sleep(60000);//每隔10s测量一次;
	}
	return 0;
}