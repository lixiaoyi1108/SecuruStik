/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	16/7/2014   11:21

	* @filename	: 	PRE_IO.h
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	PRE_IO
	* @file ext	:	h

	* @author	:	JianYe Huang
	
	* @brief	:	IO处理借口
    */
#pragma once
#include <stdio.h>
#include "openssl/ec.h"
#include "KeyGen.h"
#include "Encrypt_Decrypt.h"
#include "PRE.h"

//print the block to console
int PRE_IO_printBlock(char *m);
int PRE_IO_printBlock_ln(char *m);

int PRE_IO_printBlock_hex(char *m,size_t size);
int PRE_IO_printBlock_hex_ln(char *m,size_t size);

/************************************************************************/
/* Transform keys and files                                      */
/************************************************************************/
/*prekeys operation */
void PRE_IO_prekey2file(char *file,PRE_KEY *key,EC_GROUP *G);
/* rekeys operation */
void PRE_IO_rekey2file(char *file,PRE_REKEY *rekey,EC_GROUP *G);
void PRE_IO_file2rekey(PRE_REKEY *rekey,EC_GROUP *G,char *file);
/* keys operation */
void PRE_IO_file2key(char*filePath,char **key,PRE_KEY *key2);
void PRE_IO_key2file(char* dir,char* fileName,char *key,PRE_KEY *key1,PRE_KEY *key2);
void PRE_IO_key2file(char* fullPath,char *key,PRE_KEY *key1,PRE_KEY *key2);
