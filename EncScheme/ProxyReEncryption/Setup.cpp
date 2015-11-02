/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	5/7/2014   19:25

	* @filename	: 	C:\Users\Administrator\Desktop\Cryptology\ProxyReEncryption\ProxyReEncryption\Setup.cpp
	* @file path:	C:\Users\Administrator\Desktop\Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	Setup
	* @file ext	:	cpp

	* @author	:	JianYe Huang
	
	* @brief	:	Initialize the security parameter(G,q,g,H1,H2)
	*/
#include"PRE.h"
#include"Setup.h"
#include<stdio.h>
#include<string.h>

#include <openssl/evp.h>
#include <openssl/crypto.h>

#define HASH_1 1
#define HASH_2 2

//Security paramter(Globel)
SecParams *params;

//The size of a elliptic point stores in memory(byte)
#define POINT_BIN_SIZE 43
///Hash（Elliptic point => Zp）
BIGNUM* Hash(const EC_GROUP *G,const BIGNUM *q,const EC_POINT* point,int HashFlag)
{
	int ret;
	BN_CTX *ctx = BN_CTX_new();
	BIGNUM *x_bn = BN_new();
	BIGNUM *y_bn = BN_new();
	BIGNUM* hashNum  = BN_new();
	BIGNUM* resultNum = BN_new();
	//Elliptic point binary array
	int Len_point =sizeof(char)*POINT_BIN_SIZE;
	unsigned char *point_xy_bin = (unsigned char*)OPENSSL_malloc(Len_point);
	memset(point_xy_bin,0,Len_point);

	ret = EC_POINT_get_affine_coordinates_GF2m(G,point,x_bn,y_bn,ctx);
	CHECK_IF_ERROR(ret);

	//1、point_x && point_y => bin(42byte)
	if(HashFlag == HASH_1)
	{
		ret = BN_bn2bin(x_bn,point_xy_bin);
		CHECK_IF_ERROR(ret);
		ret = BN_bn2bin(y_bn,point_xy_bin+POINT_BIN_SIZE/2);
		CHECK_IF_ERROR(ret);
	}
	else
	{
		ret = BN_bn2bin(y_bn,point_xy_bin);
		CHECK_IF_ERROR(ret);
		ret = BN_bn2bin(x_bn,point_xy_bin+POINT_BIN_SIZE/2);
		CHECK_IF_ERROR(ret);
	}

	//2、bin(42Bytes) => hash => bin(32Bytes)
	EVP_MD_CTX hashCtx;
	unsigned char hashHex[EVP_MAX_MD_SIZE];
	unsigned int hashLen;
	//use SHA-256 alogrithm
	const EVP_MD* hashType = EVP_sha256();
	CHECK_IF_ERROR(hashType);
	OpenSSL_add_all_digests();
	EVP_MD_CTX_init(&hashCtx);
	EVP_DigestInit_ex(&hashCtx,hashType,NULL);
	EVP_DigestUpdate(&hashCtx,point_xy_bin,POINT_BIN_SIZE);
	EVP_DigestFinal_ex(&hashCtx,hashHex,&hashLen);
	EVP_MD_CTX_cleanup(&hashCtx);

	//3、bin(32Bytes) => BN
	hashNum = BN_bin2bn(hashHex,hashLen,hashNum);
	CHECK_IF_ERROR(hashNum);
	//4、Compute resultNum = hashNum (mod q)
	resultNum = BN_mod_inverse(resultNum,hashNum,q,ctx);
	CHECK_IF_ERROR(resultNum);

	BN_free(hashNum);
	BN_free(x_bn);
	BN_free(y_bn);
	BN_CTX_free(ctx);
	OPENSSL_free(point_xy_bin);
	EVP_cleanup();
	return resultNum;
}
/**
	 * @name      Hash1
	 * @brief	  hash(ellipic =>Zp)
	 * modifier   public 
	 * qualifier  (EC_GROUP *G,BIGNUM *q,const EC_POINT *point)
	 * @param	  const EC_GROUP*	G		:Ellipic
	 * @param	  const BIGNUM*		q		:Order
	 * @param	  const  EC_POINT*	point	:Ellipic point
	 * @return	  Success:Bignum map from the ellipic point
	 *            Failed:NULL
	 */
BIGNUM* Hash1(EC_GROUP *G,BIGNUM *q,const EC_POINT *point)
{
	return Hash(G,q,point,HASH_1);
}
/**
	 * @name      Hash2
	 * @brief	  hash(ellipic =>Zp). 
	 *            It has the same effect as hash1 but with different method.
	 * @see       Hash1
	 */
BIGNUM* Hash2(EC_GROUP *G,BIGNUM *q,const EC_POINT *point)
{
	return Hash(G,q,point,HASH_2);
}
/**
 * @name      Hash_point2bin
 * @brief	  hash(Ellipic point => binary string)
 * modifier   public 
 * @param	  const EC_POINT * point	:
 * @return	  Success:	Hash value(unsigned char*)
 *            Failed:	NULL
 */
unsigned char* Hash_point2bin( const EC_POINT *point )
{
	BN_CTX *ctx = BN_CTX_new();
	char* point_bin = EC_POINT_point2hex(params->G,point,POINT_CONVERSION_UNCOMPRESSED,ctx);
	CHECK_IF_ERROR(point_bin);

	unsigned char *hashHex = (unsigned char *)OPENSSL_malloc(sizeof(char)*EVP_MAX_MD_SIZE);
	unsigned int hashLen;
	//use SHA-512 alogrithm
	EVP_MD_CTX hashCtx;
	const EVP_MD* hashType = EVP_sha512();
	CHECK_IF_ERROR(hashType);
	OpenSSL_add_all_digests();
	EVP_MD_CTX_init(&hashCtx);
	EVP_DigestInit_ex(&hashCtx,hashType,NULL);
	EVP_DigestUpdate(&hashCtx,point_bin,strlen(point_bin));
	EVP_DigestFinal_ex(&hashCtx,hashHex,&hashLen);
	EVP_MD_CTX_cleanup(&hashCtx);

	OPENSSL_free(point_bin);
	BN_CTX_free(ctx);
	EVP_cleanup();
	return hashHex;
}

/**
 * @name      PRE_Setup
 * @brief	  Initialize the security parameter
 * modifier  public 
 * @return	  Success:Security parameter (SecParams *)
 *            Failed:NULL
 * @retval	  
 */
PRE_API void PRE_Setup()
{
	PRE_err_init();
	int ret;
	params = new SecParams();

	/*Get built-in ellipic curves' list and its count.*/
	size_t crv_len;
	crv_len = EC_get_builtin_curves(NULL,0);
	EC_builtin_curve  *curves = 
		(EC_builtin_curve *)OPENSSL_malloc(sizeof(EC_builtin_curve)* crv_len);

	ret = EC_get_builtin_curves(curves,crv_len);

	
	/* Generate group with selected curve  */
	int nid=curves[CURVE_INDEX].nid;
	params->G=EC_GROUP_new_by_curve_name(nid);

	//Get the order q.
	params->q = BN_new();
	ret = EC_GROUP_get_order(params->G, params->q, NULL);
	
	//Get the generator g.
	params->g = EC_GROUP_get0_generator(params->G);

	//Set the hash function.
	params->Hash1 = Hash1;
	params->Hash2 = Hash2;
	OPENSSL_free(curves);
}
/**
 * @name      PRE_UnSetup
 * @brief	  Free the memory of Security parameter
 * modifier   public 
 * @param	  SecParams * params Security parameter
 * @return	  Success:(void)
 *            Failed:
 * @retval	  
 */
static int alreadyUnSetuped = 0;
PRE_API void PRE_UnSetup()
{
	if( alreadyUnSetuped == 0)
	{
		if(NULL == params)return;
		if(NULL != params)EC_GROUP_clear_free(params->G);
		params->g = NULL;
		if(NULL != params->q)BN_free(params->q);
		params->Hash1 = NULL;
		params->Hash2 = NULL;
		delete(params);

		alreadyUnSetuped++;
		PRE_err_cleanup();
		
	}
}
