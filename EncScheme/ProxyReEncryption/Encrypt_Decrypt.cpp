/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	10/7/2014   15:18

	* @filename	: 	Encrypt_Decrypt.cpp
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	Encrypt_Decrypt
	* @file ext	:	cpp

	* @author	:	JianYe Huang
	
	* @brief	:	Encryption & Decryption.
	*/
#include <openssl\crypto.h>

#include "PRE.h"
#include"Setup.h"
#include"Encrypt_Decrypt.h"

char* Hex = "0123456789ABCDEF123456789ABCDEF";
unsigned char *XOR(unsigned char *m,unsigned char* gr)
{
	int i;
	int size = sizeof(char)*BLOCK_SIZE;
	unsigned char *result = (unsigned char*)OPENSSL_malloc(size+1);
	memset(result,0,size+1);
	for( i= 0; i !=  size ;i++)
	{
		result[i] = m[i]^gr[i];
	}
	return result;
}
unsigned char *CipherBytes2HexString(unsigned char* E)
{
	int size = sizeof(char)*BLOCK_SIZE;
	unsigned char *HexString = (unsigned char*)OPENSSL_malloc( 2 * size + 1);

	int i;
	for( i = 0 ; i != size ; i++ )
	{
		unsigned char b = E[i];
		HexString[2*i] = Hex[b >> 4];
		HexString[2*i+1] = Hex[ b & 0x0F];
	}
	HexString[2 * size] = '\0';
	return HexString;
}
unsigned char *HexString2CipherBytes(unsigned char* HexString)
{
	int size = sizeof(char)*BLOCK_SIZE;
	unsigned char *E = (unsigned char*)OPENSSL_malloc(size+1);

	int i;
	for( i = 0 ; i != size ; i++ )
	{
		int high = HexString[2*i] - '0' < 10?
			(HexString[2*i] - '0'):
			(HexString[2*i] - 'A'+10);
		int low = HexString[2*i+1] - '0'< 10?
			(HexString[2*i+1] - '0'):
			(HexString[2*i+1] - 'A'+10);
		E[i] = (high&0x0F)<<4 | (low & 0x0F);
	}
	E[size] = '\0';
	return E;
}

/**
 * @name      Encrypt
 * @brief	  Encrypt the message with user's pk
 * modifier  public 
 * @param	  PRE_PK * pki : user's public key
 * @param	  char * m : plaintext
 * @return	  Success: ciphertext structure(Cipher*)
 *            Failed: NULL
 * @retval	  
 */
Cipher* Encrypt( PRE_PK *pki,unsigned char *m )
{
	int ret;
	BN_CTX *ctx = BN_CTX_new();

	/**
	 *1、Pick r randomly from Zp
	 */
	BIGNUM *r = BN_new();
	ret = BN_rand_range(r,params->q);
	CHECK_IF_ERROR(ret);
	/**
	 *2、Compute E = m oxr Hash(g^r)
	 */
	Cipher* C = new Cipher();
	EC_POINT *gr_point = EC_POINT_new(params->G);
	ret = EC_POINT_mul(params->G,gr_point,r,NULL,NULL,NULL);//compute  g^r
	unsigned char *gr = Hash_point2bin( gr_point);//Get Hash(g^r)
	C->E = XOR(m,gr);//Compute m oxr (g^r)
	CHECK_IF_ERROR(C->E);

	/**
	 *3、Compute F = (pk1^H2(pk2)*pk2)^r
	 */
	BIGNUM *H2_pk2 = params->Hash2(params->G,params->q,pki->pk2);//compute a = H2(pk2)
	ret = EC_POINT_mul(params->G,C->F,NULL,pki->pk1,H2_pk2,NULL);//compute b = pk1^a
	CHECK_IF_ERROR(ret);
	ret = EC_POINT_add(params->G,C->F,C->F,pki->pk2,NULL);//compute c = b*pk2
	CHECK_IF_ERROR(ret);
	ret = EC_POINT_mul(params->G,C->F,NULL,C->F,r,NULL);//compute C->F = c^r
	CHECK_IF_ERROR(ret);

	BN_free(r);
	EC_POINT_free(gr_point);
	OPENSSL_free(gr);
	BN_free(H2_pk2);

	BN_CTX_free(ctx);
	return C;
}
Cipher* Encrypt( PRE_PK_str *pki_str,unsigned char* m)
{
	PRE_PK *pki = PRE_PK_bin2pk(pki_str);
	Cipher* C =  Encrypt(pki,m);
	pki->~pre_pk_st();
	return C;
}
/**
 * @name      ReEncrypt
 * @brief	  ReEncrypt
 * modifier  public 
 * @param	  PRE_REKEY * reKey
 * @param	  Cipher * C : Ciphertext structure
 * @return	  Success:ReEncrypt ciphertext(Cipher_REKEY *)
 *            Failed:NULL
 * @retval	  
 */
Cipher_REKEY* ReEncrypt( PRE_REKEY *reKey,Cipher *C )
{
	int ret;
	BN_CTX *ctx = BN_CTX_new();

	//Cipher_REKEY *C_REKEY = PRE_Init_Cipher_ReEncrypt(params->G);
	Cipher_REKEY *C_REKEY = new Cipher_REKEY();
	//C_REKEY->E
	C_REKEY->E = (unsigned char*)OPENSSL_malloc(sizeof(char)*BLOCK_SIZE);
	memset(C_REKEY->E,0,BLOCK_SIZE);
	memcpy_s(C_REKEY->E,BLOCK_SIZE,C->E,BLOCK_SIZE);
	//C_REKEY->F
	C_REKEY->F = EC_POINT_new(params->G);
	ret = EC_POINT_mul(params->G,C_REKEY->F,NULL,C->F,reKey->v,ctx);
	CHECK_IF_ERROR(ret);
	//C_REKEY->U&&W
	C_REKEY->U = EC_POINT_dup(reKey->U,params->G);
	CHECK_IF_ERROR(C_REKEY->U);
	C_REKEY->W = EC_POINT_dup(reKey->W,params->G);
	CHECK_IF_ERROR(C_REKEY->W);

	BN_CTX_free(ctx);
	return C_REKEY;
}
Cipher_REKEY* ReEncrypt( PRE_REKEY_str *reKey_str,Cipher *C)
{
	PRE_REKEY *reKey = PRE_REKEY_bin2rekey(reKey_str);
	Cipher_REKEY* C_r = ReEncrypt(reKey,C);
	reKey->~PRE_REKEY();
	return C_r;
}
/**
 * @name      Decrypt
 * @brief	  Decrypt
 * modifier  public 
 * @param	  PRE_KEY * key : User's key
 * @param	  Cipher * C : Ciphertext structure
 * @return	  Success:Plaintext(char*)
 *            Failed:NULL
 * @retval	  
 */
unsigned char* Decrypt(PRE_KEY *key,Cipher* C)
{
	BN_CTX *ctx = BN_CTX_new();
	int ret;
	//Compute H2(pki2)
	BIGNUM* m_num = params->Hash2(params->G,params->q,key->pk->pk2);
	CHECK_IF_ERROR(m_num);
	//Compute x1*H2(pki2)
	ret = BN_mul(m_num,key->sk->x1,m_num,ctx);
	CHECK_IF_ERROR(ret);
	//compute  t = x1*H2(pki2)+x2
	ret = BN_add(m_num,m_num,key->sk->x2);
	CHECK_IF_ERROR(ret);
	//Compute m_num = (1/t)mod q
	m_num = BN_mod_inverse(m_num,m_num,params->q,ctx);
	CHECK_IF_ERROR(m_num);
	//Compute F^(1/t)
	EC_POINT* temp = EC_POINT_new(params->G);
	CHECK_IF_ERROR(temp);

	ret = EC_POINT_mul(params->G,temp,NULL,C->F,m_num,NULL);
	CHECK_IF_ERROR(ret);

	//compute Hash(F^1/t)
	unsigned char *hash_gr = Hash_point2bin(temp);
	CHECK_IF_ERROR(hash_gr);


	//compute E oxr Hash(F^1/t)
	unsigned char* m =XOR(C->E,hash_gr);
	CHECK_IF_ERROR(m);

	if(hash_gr != NULL)OPENSSL_free(hash_gr);
	EC_POINT_free(temp);
	BN_free(m_num);
	BN_CTX_free(ctx);
	return m;
}
unsigned char* Decrypt(PRE_KEY_str *key_str,Cipher* C)
{
	PRE_KEY *key = PRE_KEY_bin2key(key_str);
	unsigned char* D = Decrypt(key,C);
	key->~PRE_KEY();
	return D;
}
/**
 * @name      Decrypt
 * @brief	  Decrypt corresponding to ReEncrypt
 * modifier  public 
 * @param	  PRE_KEY * key : User's key
 * @param	  Cipher_REKEY * C : Cipher_reEncrypt structure
 * @return	  Success:Plaintext(char*)
 *            Failed:NULL
 * @retval	  
 */
unsigned char* Decrypt(PRE_KEY *key,Cipher_REKEY* C)
{
	int ret;
	EC_POINT *m_point = EC_POINT_new(params->G);
	BN_CTX *ctx = BN_CTX_new();
	/**
	 *1、Compute V = U + (-W^((x2)^-1(mod q)))
	 */
	BIGNUM *x2_inverse = BN_dup(key->sk->x2);
	x2_inverse = BN_mod_inverse(x2_inverse,x2_inverse,params->q,ctx);//(x2)^-1(mod q)
	EC_POINT *V  = EC_POINT_new(params->G);
	CHECK_IF_ERROR(V);
	ret = EC_POINT_mul(params->G,V,NULL,C->W,x2_inverse,ctx);//W^((x2)^-1(mod q))
	CHECK_IF_ERROR(ret);
	ret = EC_POINT_invert(params->G,V,ctx);//-W^((x2)^-1(mod q))
	CHECK_IF_ERROR(ret);
	ret = EC_POINT_add(params->G,V,C->U,V,ctx);//U + (-W^((x2)^-1(mod q)))
	CHECK_IF_ERROR(ret);
	/**
	 *2、m = E oxr (F^(H1(V)^-1))
	 */
	BIGNUM* H1_V = params->Hash1(params->G,params->q,V);//H1(V)
	H1_V = BN_mod_inverse(H1_V,H1_V,params->q,ctx);//H1(V)^-1
	CHECK_IF_ERROR(ret);
	ret = EC_POINT_mul(params->G,m_point,NULL,C->F,H1_V,ctx);//F^(H1(V)^-1)
	CHECK_IF_ERROR(ret);
	
	//compute Hash(F^(H1(V)^-1))
	unsigned char *hash_gr = Hash_point2bin(m_point);
	CHECK_IF_ERROR(hash_gr);

	unsigned char* m =XOR(C->E,hash_gr);
	CHECK_IF_ERROR(m);

	EC_POINT_free(V);
	EC_POINT_free(m_point);
	BN_free(x2_inverse);
	BN_free(H1_V);
	OPENSSL_free(hash_gr);
	BN_CTX_free(ctx);
	return m;
}
unsigned char* Decrypt(PRE_KEY_str *key_str,Cipher_REKEY *C)
{
	PRE_KEY *key = PRE_KEY_bin2key(key_str);
	unsigned char *D = Decrypt(key,C);
	key->~PRE_KEY();
	return D;
}

/************************************************************************/
/*                       Cipher <==> Cipher-string               */
/************************************************************************/
CipherStr_REKEY* PRE_Cipher2bin(Cipher_REKEY* C)
{
	if(!C)return NULL;
	CipherStr_REKEY* strC = new CipherStr_REKEY();
	strC->E = CipherBytes2HexString(C->E);//(unsigned char*)OPENSSL_malloc(sizeof(char)*BLOCK_SIZE);memcpy(strC->E,C->E,BLOCK_SIZE);
	strC->F = (unsigned char*)EC_POINT_point2hex(params->G,C->F,PRE_POINT_CONVERSION,NULL);
	strC->U = (unsigned char*)EC_POINT_point2hex(params->G,C->U,PRE_POINT_CONVERSION,NULL);
	strC->W = (unsigned char*)EC_POINT_point2hex(params->G,C->W,PRE_POINT_CONVERSION,NULL);

	return strC;
}
Cipher_REKEY* PRE_bin2Cipher(CipherStr_REKEY* strC)
{
	if(!strC) return NULL;
	Cipher_REKEY* C = new Cipher_REKEY();
	C->E = HexString2CipherBytes(strC->E);//(unsigned char*)OPENSSL_malloc(sizeof(char)*BLOCK_SIZE);memcpy(C->E,strC->E,BLOCK_SIZE);
	C->F = EC_POINT_hex2point(params->G,(const char*)strC->F,C->F,NULL);
	C->U = EC_POINT_hex2point(params->G,(const char*)strC->U,C->U,NULL);
	C->W = EC_POINT_hex2point(params->G,(const char*)strC->W,C->W,NULL);

	return C;
}



/************************************************************************/
/*                       Encrypt/Decrypt API           */
/************************************************************************/
PRE_API CipherStr_REKEY* PRE_Encrypt(PRE_KEY_str *key_str,PRE_PK_str* pkj_str, unsigned char* m)
{
	PRE_KEY* key = PRE_KEY_bin2key(key_str);
	PRE_PK* pkj = PRE_PK_bin2pk(pkj_str);

	PRE_REKEY *rekey = new PRE_REKEY(key,pkj);
	Cipher* C = Encrypt(key->pk,m);

	Cipher_REKEY* C_REKEY = ReEncrypt(rekey,C);

	CipherStr_REKEY *strC_REKEY  = PRE_Cipher2bin(C_REKEY);

	return strC_REKEY;
}
PRE_API unsigned char* PRE_Decrypt(PRE_KEY_str *key_str,CipherStr_REKEY *strC)
{
	Cipher_REKEY* C = PRE_bin2Cipher(strC);
	return Decrypt(key_str,C);
}