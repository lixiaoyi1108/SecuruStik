/*!
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	4/7/2014   19:11

	* @filename	: 	UnitTest.cpp
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	UnitTest
	* @file ext	:	cpp

	* @author	:	JianYe Huang
	
	*brief	:	对程序功能进行单元测试
	*/
#define DEBUG
#include<openssl\applink.c>
#include "openssl\evp.h"
#include"PRE.h"
#include"Setup.h"
#include"Encrypt_Decrypt.h"

#include<stdio.h>
#include<string.h>
#include<time.h>
#include <conio.h>
#include <process.h>

clock_t begin,end;
int r_time = 1000000;
int g_time = 100;

unsigned char *m = (unsigned char*)"000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F";

/*****************************/
/* memory leak test          */
/****************************/
void mem_debug_begin()
{
	CRYPTO_malloc_debug_init();
	CRYPTO_set_mem_debug_options(V_CRYPTO_MDEBUG_ALL);
	CRYPTO_mem_ctrl(CRYPTO_MEM_CHECK_ON);
}
void mem_debug_end()
{
	char *logFile = "leak.log";
	CRYPTO_mem_ctrl(CRYPTO_MEM_CHECK_OFF);
	BIO *b = BIO_new_file(logFile,"w");
	CRYPTO_mem_leaks(b);
	BIO_free(b);
}
/**************************************************/
/* Tests                                     */
/*********************************************/
int Setup_Test()
{
	int j = g_time;
	while(j--)
	{
		int i = r_time;
		begin = clock();
		while(i--)PRE_Setup();PRE_UnSetup();
		end = clock();
		printf("%d:Setup : %f ms...\n",10-j,(double)(end-begin));
	}
	return 1;
}
int Hash_Test()
{
	int j = g_time;
	while(j--)
	{
		unsigned char* test;
		int i = r_time;
		begin = clock();
		//while(i--)params->Hash2(params->G,params->q,params->g);
		while(i--)test = Hash_point2bin(params->g);
		end = clock();
		printf("%d:Hash : %f ms...\n",10-j,(double)(end-begin));
		OPENSSL_free(test);
	}
	return 1;
}
int KeyGen_Test()
{
	int j = g_time;
	while(j--)
	{
		PRE_KEY *key;
		int i = r_time;
		begin = clock();
		while(i--)key = new PRE_KEY();
		end = clock();
		printf("%d:KeyGen : %f ms...\n",10-j,(double)(end-begin));
		key->~PRE_KEY();
	}
	return 1;
}
int ReKeyGen_Test()
{
	int j = g_time;
	while(j--)
	{
		PRE_KEY *key= new PRE_KEY();
		PRE_REKEY *reKey;
		int i = r_time;

		begin = clock();
		while(i--) reKey = new PRE_REKEY(key,key->pk);
		end = clock();

		printf("%d:ReKeyGen : %f ms...\n",10-j,(double)(end-begin));
		reKey->~PRE_REKEY();
		key->~PRE_KEY();
	}
	return 1;
}
int Encrypt_Test(PRE_KEY *key)
{
	int j = g_time;
	while(j--)
	{
		int i = r_time;
		Cipher *C = Encrypt(key->pk,m);
		begin = clock();
		while(i--) {OPENSSL_free(Decrypt(key,C));}
		end = clock();
		C->~pre_cipher_st();

		printf("%d:Encrypt : %f ms...\n",10-j,(double)(end-begin));
	}

	return 1;
}
int ReEncrypt_Test(PRE_REKEY *rePreKey,Cipher *C)
{
	int j = g_time;
	while(j--)
	{

		int i = r_time;
		begin = clock();
		while(i--) {(ReEncrypt(rePreKey,C))->~pre_cipher_rekey_st();}
		end = clock();
		printf("%d:ReEncrypt : %f ms...\n",10-j,(double)(end-begin));
	}
	return 1;
}
int ReDecrypt_Test(PRE_KEY *key,Cipher_REKEY *C)
{
	int j = g_time;
	while(j--)
	{

		int i = r_time;
		begin = clock();
		while(i--){OPENSSL_free(Decrypt(key,C));}
		end = clock();
		printf("%d:ReDecrypt : %f ms...\n",10-j,(double)(end-begin));

	}
	return 1;
}
//#define runTime 1000000
#define runTime 10
int stressTest()
{
	begin = clock();

	int i = runTime;
	while( i-- )
	{
	}

	end = clock();
	printf("StressTest :		\n");
	printf("Run for %d times.	\n",runTime);
	printf("Take up %f ms...	\n",(double)(end-begin));

	return 0;
}
int StressTest()
{
	//mem_debug_begin();
	PRE_err_init();

	stressTest();

	PRE_err_cleanup();
	//mem_debug_end();
	//system("pause");
	return 0;
}

void APITest()
{
	PRE_Setup();
	unsigned char* m =(unsigned char*) "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";

	PRE_KEY_str* key1 = PRE_KEYstr_Create();
	PRE_KEY_str* key2 = PRE_KEYstr_Create();

	CipherStr_REKEY *C = PRE_Encrypt(key1,key2->pk,m);
	unsigned char* mm = PRE_Decrypt(key2,C);

	printf("%s\n",mm);

	PRE_UnSetup();
}

void Test()
{
	char* t = "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF";
	unsigned char* t1 = CipherBytes2HexString((unsigned char*)t);
	printf("%s\n(%d)",t1,strlen((char*)t1));
	unsigned char* t2 = HexString2CipherBytes((unsigned char*)t1);
	printf("%s\n",t2);
}
int main(int argc,char* argv[])
{
	APITest();
	char c;
	scanf("%c",&c);
	//Test();
	return 0;
}
