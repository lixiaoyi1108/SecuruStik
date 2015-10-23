/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	10/7/2014   19:29

	* @filename	: 	PRE_ERR.cpp
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	PRE_ERR
	* @file ext	:	cpp

	* @author	:	JianYe Huang
	
	* @brief	:	Exception
	*/
#include<openssl\err.h>
#include<time.h>
#include<string.h>
#define ERR_BUF_SIZE 600//(>=120）
#define DEBUG

///err message
static char ErrBuff[ERR_BUF_SIZE];
///err code
static long ErrCode = 0;
///err list num
static long ErrNo = 1;

static char* LogFilePath = "./log.txt";

//Get last err info-string
char* GetLastErrorString()
{
	ErrCode = ERR_peek_last_error();
	ERR_error_string(ErrCode,ErrBuff);
	return ErrBuff;
}
//Get last err info-string according the err Code
char* GetErrorString(unsigned long errCode)
{
	return ERR_error_string(errCode,NULL);
}
//Set the err info
unsigned long SetLastError(int ec)
{
	ErrCode = ec;
	return 1;
}

void Logging(char* errFile,char* function,int line,char* info)
{

	FILE* fp = fopen(LogFilePath,"a");
	if(NULL == fp)return;
	time_t st = time(NULL);//Get system time

	fprintf(fp,"File : %s\r\n",errFile);
	fprintf(fp,"Function : %s\r\n",function);
	fprintf(fp,"Line : %d\r\n",line);
	fprintf(fp,"Message : %s\r\n",info);
	fputs("======================================\r\n",fp);

	fclose(fp);
}
//register all codes of libcrypto functions in OpenSSL
void PRE_err_init()
{
	ERR_load_crypto_strings();
	memset(ErrBuff,0,ERR_BUF_SIZE);
}
//Clear all the loaded err information
void PRE_err_cleanup()
{
	ERR_free_strings();
}
//Get last err informations(without logging)
unsigned long PRE_ERR_GetLastError()
{
	ErrCode = ERR_peek_last_error();
	ERR_error_string(ErrCode,ErrBuff);
#ifdef DEBUG
	printf("%d_%s\n",ErrNo++,ErrBuff);
#endif
	return ErrCode;
}
///Get last err informations(log the debug information)
unsigned long PRE_ERR_GetLastError(char* filePath,char* function,int line)
{
	PRE_ERR_GetLastError();
	char* fileName = strrchr(filePath,'\\');
	Logging(filePath,function,line,GetLastErrorString());
	printf("\t@ File     : %s(Line:%d)\n\t@ Function : %s\n",fileName+1,line,function);
	return ErrCode;
}