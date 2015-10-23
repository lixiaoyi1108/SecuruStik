/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	10/7/2014   15:22

	* @filename	: 	Encrypt_Decrypt.h
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	Encrypt_Decrypt
	* @file ext	:	h

	* @author	:	JianYe Huang
	
	* @brief	:	Functions for encryption & decryption
    */
#pragma once
#include<string.h>
#include"KeyGen.h"
#include "PRE_IO.h"
typedef struct pre_cipher_st
{
	unsigned char* E;
	EC_POINT* F;
	pre_cipher_st()
	{
		this->E = NULL;
		this->F = EC_POINT_new(params->G);
	}
	~pre_cipher_st()
	{
		free();
	}
	int free()
	{
		OPENSSL_free(this->E);
		EC_POINT_free(this->F);
		return 0;
	}
}Cipher;

typedef struct pre_cipher_rekey_st
{
	unsigned char*E;
	EC_POINT *F;
	EC_POINT *U;
	EC_POINT *W;
	pre_cipher_rekey_st()
	{
		this->E = (unsigned char*)OPENSSL_malloc(sizeof(char)*BLOCK_SIZE);memset(this->E,0,BLOCK_SIZE);
		this->F = EC_POINT_new(params->G);
		this->U = EC_POINT_new(params->G);
		this->W = EC_POINT_new(params->G);
	}
	~pre_cipher_rekey_st()
	{
		free();
	}
	int free()
	{
		OPENSSL_free(this->E);
		EC_POINT_free(this->F);
		EC_POINT_free(this->U);
		EC_POINT_free(this->W);
		return 0;
	}
}Cipher_REKEY;
typedef struct 
{
	unsigned char* E;
	unsigned char* F;
	unsigned char* U;
	unsigned char* W;
}CipherStr_REKEY;

/************************************************************************/
/* Encrypt && Decrypt                                                   */
/************************************************************************/
Cipher* Encrypt( PRE_PK *pki,unsigned char *m);
Cipher* Encrypt( PRE_PK_str *pki_str,unsigned char* m);
Cipher_REKEY* ReEncrypt( PRE_REKEY *rePreKey,Cipher *C);
Cipher_REKEY* ReEncrypt( PRE_REKEY_str *reKey_str,Cipher *C);

unsigned char* Decrypt(PRE_KEY *key,Cipher* C);
unsigned char* Decrypt(PRE_KEY *key,Cipher_REKEY* C);
unsigned char* Decrypt(PRE_KEY_str *key_str,Cipher* C);
unsigned char* Decrypt(PRE_KEY_str *key_str,Cipher_REKEY *C);

/************************************************************************/
/*                       Encrypt/Decrypt API           */
/************************************************************************/
PRE_API CipherStr_REKEY* PRE_Encrypt(PRE_KEY_str *key_str,PRE_PK_str* pkj_str, unsigned char* m);
PRE_API unsigned char* PRE_Decrypt(PRE_KEY_str *key_str,CipherStr_REKEY *strC);

/************************************************************************/
/*                       BinaryString <=> HexString          */
/************************************************************************/
unsigned char *CipherBytes2HexString(unsigned char* E);
unsigned char *HexString2CipherBytes(unsigned char* HexString);