/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	4/7/2014   19:27

	* @filename	: 	PRE.h
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	PRE
	* @file ext	:	h

	* @author	:	JianYe Huang
	
	* @brief	:	
    */
#pragma once
#include <openssl/ec.h>
#include <openssl/ecdsa.h>
#include <openssl/ecdh.h>
#include "PRE_err.h"

#define PRE_SUCCESS		1
#define PRE_FAILE		0
#define FAILE			-1
#define PFAILE			NULL


///Block size(SHA-256)
#define BLOCK_SIZE		64

//Ellipic point conversion method ( default : compressed )
#define PRE_POINT_CONVERSION POINT_CONVERSION_COMPRESSED
#define PRE_POINT_COMPRESSED POINT_CONVERSION_COMPRESSED
#define PRE_POINT_UNCOMPRESSED POINT_CONVERSION_UNCOMPRESSED

//Globle
//Elliptic curve security paramter structure
typedef struct ec_SecParams_st
{
	//Ellipic curve
	EC_GROUP *G;
	//Order of the G
	BIGNUM *q;
	//Generator
	const EC_POINT *g;
	//Hash(ellipic =>Zp)
	BIGNUM* (*Hash1)(EC_GROUP *G,BIGNUM *q,const EC_POINT *point);
	BIGNUM* (*Hash2)(EC_GROUP *G,BIGNUM *q,const EC_POINT *point);
}SecParams,*pSecParams;

extern SecParams *params;

#ifdef _EXPORTING
#	define PRE_API extern "C" __declspec(dllexport)
#else
#	define PRE_API extern "C"
#endif
