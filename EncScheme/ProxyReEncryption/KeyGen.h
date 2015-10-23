/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	11/7/2014   19:24

	* @filename	: 	KeyGen.h
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	KeyGen
	* @file ext	:	h

	* @author	:	JianYe Huang
	
	* @brief	:	Operations for keys(pk,sk,rekey)
    */
#pragma once
#include"PRE.h"
/************************************************************************/
/*                    Key structures                                    */
/************************************************************************/
typedef struct pre_sk_st
{
	BIGNUM* x1;
	BIGNUM* x2;
	pre_sk_st()
	{
		this->x1 = BN_new();
		this->x2 = BN_new();
	}
	~pre_sk_st()
	{
		free();
	}
	int free()
	{
		BN_free(this->x1);
		BN_free(this->x2);
		return 0;
	}
}PRE_SK;
typedef struct pre_pk_st
{
	EC_POINT* pk1;
	EC_POINT* pk2;
	pre_pk_st()
	{
		this->pk1 = EC_POINT_new(params->G);
		this->pk2 = EC_POINT_new(params->G);
	}
	~pre_pk_st()
	{
		free();
	}
	int free()
	{
		EC_POINT_free(this->pk1);
		EC_POINT_free(this->pk2);
		return 0;
	}
}PRE_PK;
typedef struct pre_Key_st
{
	PRE_PK *pk;
	PRE_SK *sk;
	pre_Key_st()
	{
		this->pk = new PRE_PK();
		this->sk = new PRE_SK();

		BN_CTX *ctx = BN_CTX_new();

		BN_rand_range(this->sk->x1,params->q);
		BN_rand_range(this->sk->x2,params->q);
		EC_POINT_mul(params->G,this->pk->pk1,this->sk->x1,NULL,NULL,ctx);
		EC_POINT_mul(params->G,this->pk->pk2,this->sk->x2,NULL,NULL,ctx);

		BN_CTX_free(ctx);
	}
	~pre_Key_st()
	{
		free();
	}
	int free()
	{
		this->pk->~PRE_PK();
		this->sk->~PRE_SK();
		return 0;
	}
}PRE_KEY;
typedef struct pre_ReKey_st
{
	BIGNUM *v;
	EC_POINT *U;
	EC_POINT *W;
	pre_ReKey_st()
	{
		this->v = BN_new();
		this->U = EC_POINT_new(params->G);
		this->W = EC_POINT_new(params->G);
	}
	pre_ReKey_st(PRE_KEY *keyi,PRE_PK *pkj)
	{
		int ret;
		BN_CTX *ctx = BN_CTX_new();
		
		this->v = BN_new();
		this->U = EC_POINT_new(params->G);
		this->W = EC_POINT_new(params->G);

		/*
		*1、Randomly pick V from G and u from Zp
		*/
		EC_POINT *V = EC_POINT_new(params->G);
		BIGNUM *rM = BN_new();
		ret = BN_rand_range(rM,params->q);//Get a random bignum rM
		ret = EC_POINT_mul(params->G,V,rM,NULL,NULL,ctx);//compute V = rM*g

		BIGNUM *u = BN_new();
		ret = BN_rand_range(u,params->q);

		/*
		*2、Compute v = H1(V)*(x1*H2(pk2)+x2)^-1(mod q)
		*/
		BIGNUM *H1_V = params->Hash1(params->G,params->q,V);//H1(V)
		BIGNUM *H2_PK2 = params->Hash2(params->G,params->q,keyi->pk->pk2);//H2(pk2)------pkj->pk2还是keyi->pk->pk2?

		ret = BN_mod_mul(this->v,keyi->sk->x1,H2_PK2,params->q,ctx);//x1*H2(pk2) (mod q)
		ret = BN_mod_add(this->v,this->v,keyi->sk->x2,params->q,ctx);//x1*H2(pk2)+x2 (mod q)
		this->v = BN_mod_inverse(this->v,this->v,params->q,ctx);//(x1*H2(pk2)+x2)^-1(mod q)
		ret = BN_mod_mul(this->v,H1_V,this->v,params->q,ctx);//H1(V)*(x1*H2(pk2)+x2)^-1(mod q)

		/*
		*2、Compute U = V+u*g
		*/
		ret = EC_POINT_mul(params->G,this->U,u,NULL,NULL,ctx);//u*g
		ret = EC_POINT_add(params->G,this->U,V,this->U,ctx);//U = V+u*g

		/*
		*3、Compute W = u*pk2
		*/
		ret = EC_POINT_mul(params->G,this->W,NULL,pkj->pk2,u,ctx);

		//release mem
		EC_POINT_free(V);
		BN_free(rM);
		BN_free(u);
		BN_free(H1_V);
		BN_free(H2_PK2);

		BN_CTX_free(ctx);
	}
	~pre_ReKey_st()
	{
		free();
	}
	int free()
	{
		EC_POINT_free(this->U);
		EC_POINT_free(this->W);
		BN_free(this->v);
		return 0;
	}
}PRE_REKEY;

typedef struct pre_sk_str
{
	char* x1;
	char* x2;
}PRE_SK_str;
typedef struct pre_pk_str
{
	char* pk1;
	char* pk2;
}PRE_PK_str;
typedef struct pre_Key_str
{
	PRE_PK_str *pk;
	PRE_SK_str *sk;
}PRE_KEY_str;
typedef struct pre_ReKey_str
{
	char* v;
	char* U;
	char* W;
}PRE_REKEY_str;

/************************************************************************/
/*                   KeyStr structure opts                     */
/************************************************************************/
PRE_REKEY_str* PRE_REKEY_Create(PRE_KEY_str *keyi_str,PRE_PK_str *pkj_str);

PRE_API PRE_KEY_str* PRE_KEYstr_Create();

/************************************************************************/
/*                       key <==> key-string               */
/************************************************************************/
PRE_SK_str* PRE_SK_sk2bin(const PRE_SK *sk);
PRE_PK_str* PRE_PK_pk2bin(const PRE_PK *pk);
PRE_KEY_str* PRE_KEY_key2bin(const PRE_KEY *key);
PRE_REKEY_str* PRE_REKEY_rekey2bin(const PRE_REKEY *rekey);

PRE_SK* PRE_SK_bin2sk(const PRE_SK_str *bin_sk);
PRE_PK* PRE_PK_bin2pk(const PRE_PK_str *bin_pk);
PRE_KEY* PRE_KEY_bin2key(const PRE_KEY_str *bin_key);
PRE_REKEY* PRE_REKEY_bin2rekey(const PRE_REKEY_str *bin_rekey);

void PRE_SKstr_free(PRE_SK_str *sk);
void PRE_PKstr_free(PRE_PK_str *pk);
void PRE_KEYstr_free(PRE_KEY_str *key);
void PRE_REKEYstr_free(PRE_REKEY_str *rekey);

/************************************************************************/
/*                       Key API                                        */
/************************************************************************/
PRE_API unsigned char* PRE_KEY_AES128_Create();