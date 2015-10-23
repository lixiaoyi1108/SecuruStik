/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	5/7/2014   08:23

	* @filename	: 	KeyGen.cpp
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	KeyGen
	* @file ext	:	cpp

	* @author	:	JianYe Huang
	
	* @brief	:	Operations for keys(pk,sk,rekey)
	*/
#include"Setup.h"
#include"KeyGen.h"
#include <string.h>
#include<openssl/rand.h>

/************************************************************************/
/*                       key <==> key-string               */
/************************************************************************/
PRE_REKEY_str* PRE_REKEY_Create(PRE_KEY_str *keyi_str,PRE_PK_str *pkj_str)
{
	PRE_KEY* keyi = PRE_KEY_bin2key(keyi_str);
	PRE_PK* pkj = PRE_PK_bin2pk(pkj_str);

	PRE_REKEY* rekey = new PRE_REKEY(keyi,pkj);
	PRE_REKEY_str *strRekey = PRE_REKEY_rekey2bin(rekey);

	keyi->~PRE_KEY();
	pkj->~PRE_PK();
	rekey->~PRE_REKEY();

	return strRekey;
}
PRE_SK_str* PRE_SK_sk2bin(const PRE_SK *sk)
{
	if(!sk)return NULL;
	PRE_SK_str* strSK = new PRE_SK_str();
	strSK->x1 = BN_bn2hex(sk->x1);
	strSK->x2 = BN_bn2hex(sk->x2);
	return strSK;
}
PRE_PK_str* PRE_PK_pk2bin(const PRE_PK *pk)
{
	if(!pk)return NULL;
	PRE_PK_str* strPK = new PRE_PK_str();
	strPK->pk1 = EC_POINT_point2hex(params->G,pk->pk1,PRE_POINT_CONVERSION,NULL);
	strPK->pk2 = EC_POINT_point2hex(params->G,pk->pk2,PRE_POINT_CONVERSION,NULL);
	return strPK;
}
PRE_KEY_str* PRE_KEY_key2bin(const PRE_KEY *key)
{
	if(!key)return NULL;
	PRE_KEY_str* strKey = new PRE_KEY_str();
	strKey->pk = new PRE_PK_str();
	strKey->sk = new PRE_SK_str();
	strKey->pk->pk1 = EC_POINT_point2hex(params->G,key->pk->pk1,PRE_POINT_CONVERSION,NULL);CHECK_IF_ERROR(strKey->pk->pk1);
	strKey->pk->pk2 = EC_POINT_point2hex(params->G,key->pk->pk2,PRE_POINT_CONVERSION,NULL);CHECK_IF_ERROR(strKey->pk->pk2);
	strKey->sk->x1 = BN_bn2hex(key->sk->x1);
	strKey->sk->x2 = BN_bn2hex(key->sk->x2);
	return strKey;
}
PRE_REKEY_str* PRE_REKEY_rekey2bin(const PRE_REKEY *rekey)
{
	if(!rekey)return NULL;
	PRE_REKEY_str* strReKey = new PRE_REKEY_str();
	strReKey->v = BN_bn2hex(rekey->v);
	strReKey->U = EC_POINT_point2hex(params->G,rekey->U,PRE_POINT_CONVERSION,NULL);
	strReKey->W = EC_POINT_point2hex(params->G,rekey->W,PRE_POINT_CONVERSION,NULL);
	return strReKey;
}

PRE_SK* PRE_SK_bin2sk( const PRE_SK_str *bin_sk )
{
	PRE_SK* sk = new PRE_SK();
	BN_hex2bn(&sk->x1,bin_sk->x1);
	BN_hex2bn(&sk->x2,bin_sk->x2);
	return sk;
}
PRE_PK* PRE_PK_bin2pk( const PRE_PK_str *bin_pk )
{
	PRE_PK *pk = new PRE_PK();
	EC_POINT_hex2point(params->G,bin_pk->pk1,pk->pk1,NULL);
	EC_POINT_hex2point(params->G,bin_pk->pk2,pk->pk2,NULL);
	return pk;
}
PRE_KEY* PRE_KEY_bin2key( const PRE_KEY_str *bin_key )
{
	PRE_KEY *key = new PRE_KEY();
	BN_hex2bn(&key->sk->x1,bin_key->sk->x1);
	BN_hex2bn(&key->sk->x2,bin_key->sk->x2);
	EC_POINT_hex2point(params->G,bin_key->pk->pk1,key->pk->pk1,NULL);
	EC_POINT_hex2point(params->G,bin_key->pk->pk2,key->pk->pk2,NULL);
	return key;
	
}
PRE_REKEY* PRE_REKEY_bin2rekey( const PRE_REKEY_str *bin_rekey )
{
	PRE_REKEY *rekey = new PRE_REKEY();
	BN_hex2bn(&rekey->v,bin_rekey->v);
	EC_POINT_hex2point(params->G,bin_rekey->U,rekey->U,NULL);
	EC_POINT_hex2point(params->G,bin_rekey->W,rekey->W,NULL);
	return rekey;
}

void PRE_SKstr_free(PRE_SK_str *sk)
{
	OPENSSL_free(sk->x1);
	OPENSSL_free(sk->x2);
	delete(sk);
}
void PRE_PKstr_free(PRE_PK_str *pk)
{
	OPENSSL_free(pk->pk1);
	OPENSSL_free(pk->pk2);
	delete(pk);
}
void PRE_KEYstr_free(PRE_KEY_str *key)
{
	OPENSSL_free(key->pk->pk1);
	OPENSSL_free(key->pk->pk2);
	OPENSSL_free(key->sk->x1);
	OPENSSL_free(key->sk->x2);
	delete(key);
}
void PRE_REKEYstr_free(PRE_REKEY_str *rekey)
{
	OPENSSL_free(rekey->v);
	OPENSSL_free(rekey->U);
	OPENSSL_free(rekey->W);
	delete(rekey);
}

/************************************************************************/
/*                       Key API                                        */
/************************************************************************/
PRE_API PRE_KEY_str* PRE_KEYstr_Create()
{
	PRE_KEY *key = new PRE_KEY();
	PRE_KEY_str *keyStr =  PRE_KEY_key2bin(key);CHECK_IF_ERROR(key->pk->pk1);
	key->~PRE_KEY();

	return keyStr;
}