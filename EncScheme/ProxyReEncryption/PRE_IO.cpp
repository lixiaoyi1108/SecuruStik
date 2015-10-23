/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	16/7/2014   11:20

	* @filename	: 	PRE_IO.cpp
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	PRE_IO
	* @file ext	:	cpp

	* @author	:	JianYe Huang
	
	* @brief	:	
	*/
#include"PRE_IO.h"
#include"PRE.h"
#include <direct.h>
#include <string.h>

int sizeOfInt = sizeof(int);
/**
 * @name      PRE_printBlock
 * @brief	  print the block to console
 * modifier  public 
 * qualifier
 * @param	  char * m 
 * @return	  Success:1(int)
 *            Failed:IO erer code
 * @retval	  
 */
int PRE_IO_printBlock(char *m)
{
	if( NULL == m )return 0;
	int i;
	for( i = 0 ; BLOCK_SIZE != i ; i++ )
	{
		printf("%c",m[i]);
	}
	return 1;
}
int PRE_IO_printBlock_ln(char *m)
{
	if( NULL == PRE_IO_printBlock(m))
		return 0;
	printf("\n");
	return 1;
}

int PRE_IO_printBlock_hex(char *m,size_t size)
{
	if ( NULL == m )return 0;
	BIGNUM* bn = BN_new();
	bn = BN_bin2bn((const unsigned char*)m,size,bn);
	CHECK_IF_ERROR(bn);
	printf("%s",BN_bn2hex(bn));
	BN_free(bn);
	return 1;
}

int PRE_IO_printBlock_hex_ln(char *m,size_t size)
{
	if( NULL == PRE_IO_printBlock_hex(m,size))
		return 0;
	printf("\n");
	return 1;
}

void PRE_IO_prekey2file(PRE_KEY *key,EC_GROUP *G,char *file)
{
	FILE *fp;
	if ( NULL == (fp = fopen( file,"wb")) )exit(1);

	BN_CTX *ctx = BN_CTX_new();
	char *pk1 = EC_POINT_point2hex(G,key->pk->pk1,POINT_CONVERSION_UNCOMPRESSED,ctx);
	char *pk2 = EC_POINT_point2hex(G,key->pk->pk2,POINT_CONVERSION_UNCOMPRESSED,ctx);
	char *sk1 = BN_bn2hex(key->sk->x1);
	char *sk2 = BN_bn2hex(key->sk->x2);
	int sizeOfPk1 = sizeof(pk1);
	int sizeOfPk2 = sizeof(pk2);
	int sizeOfSk1 = sizeof(sk1);
	int sizeOfSk2 = sizeof(sk2);
	fwrite(&sizeOfPk1,sizeOfInt,1,fp);fwrite(pk1,sizeof(pk1),1,fp);
	fwrite(&sizeOfPk2,sizeOfInt,1,fp);fwrite(pk2,sizeof(pk2),1,fp);
	fwrite(&sizeOfSk1,sizeOfInt,1,fp);fwrite(sk1,sizeof(sk1),1,fp);
	fwrite(&sizeOfSk2,sizeOfInt,1,fp);fwrite(sk2,sizeof(sk2),1,fp);
	BN_CTX_free(ctx);

	fclose(fp);
}

void PRE_IO_rekey2file( char *file,PRE_REKEY *rekey,EC_GROUP *G )
{
	FILE *fp;
	if( NULL == (fp = fopen( file,"wb")) )exit(1);
	/*Write order：
	 * v->U->W
	 */
	BN_CTX *ctx = BN_CTX_new();
	char *v = BN_bn2hex(rekey->v);
	char *U = EC_POINT_point2hex(G,rekey->U,PRE_POINT_CONVERSION,ctx);
	char *W = EC_POINT_point2hex(G,rekey->W,PRE_POINT_CONVERSION,ctx);
	int strlenOfV = strlen(v);
	int strlenOfU = strlen(U);
	int strlenOfW = strlen(W);
	fwrite(&strlenOfV,sizeOfInt,1,fp);
	fwrite(&strlenOfU,sizeOfInt,1,fp);
	fwrite(&strlenOfW,sizeOfInt,1,fp);
	fwrite(v,strlenOfV,1,fp);
	fwrite(U,strlenOfU,1,fp);
	fwrite(W,strlenOfW,1,fp);
	BN_CTX_free(ctx);

	fclose(fp);

}

void PRE_IO_file2rekey(PRE_REKEY *rekey,EC_GROUP *G,char *file)
{
	FILE *fp;
	if( NULL == (fp = fopen( file,"rb")) )exit(1);

	BN_CTX *ctx = BN_CTX_new();
	/*Read order
	 * v->U->W
	 */
	int strlenOfV = 0;
	int strlenOfU = 0;
	int strlenOfW = 0;
	fread(&strlenOfV,sizeOfInt,1,fp);
	fread(&strlenOfU,sizeOfInt,1,fp);
	fread(&strlenOfW,sizeOfInt,1,fp);
	char *v = (char*)OPENSSL_malloc(strlenOfV);
	char *U = (char*)OPENSSL_malloc(strlenOfU);
	char *W = (char*)OPENSSL_malloc(strlenOfW);
	fread(v,strlenOfV,1,fp);
	fread(U,strlenOfU,1,fp);
	fread(W,strlenOfW,1,fp);

	BN_hex2bn(&rekey->v,v);
	EC_POINT_hex2point(G,U,rekey->U,ctx);
	EC_POINT_hex2point(G,W,rekey->W,ctx);

	BN_CTX_free(ctx);
	fclose(fp);
}

void PRE_IO_key2file(char* fullPath,unsigned char *key,PRE_KEY *key1,PRE_KEY *key2)
{
	FILE *fp;
	if( NULL == ( fp = fopen(fullPath,"wb+") ) )
		exit(1);

	PRE_REKEY *reKey = new PRE_REKEY(key1,key2->pk);
	Cipher *C = Encrypt(key1->pk,key);
	Cipher_REKEY *C_r = ReEncrypt(reKey,C);
	
	/*Write order：
	 * v->U->W
	 */
	BN_CTX *ctx = BN_CTX_new();
	char *F = EC_POINT_point2hex(params->G,C_r->F,PRE_POINT_CONVERSION,ctx);
	char *U = EC_POINT_point2hex(params->G,C_r->U,PRE_POINT_CONVERSION,ctx);
	char *W = EC_POINT_point2hex(params->G,C_r->W,PRE_POINT_CONVERSION,ctx);
	int strlenOfE = 128;//strlen(C_r->E);
	int strlenOfF = strlen(F);
	int strlenOfU = strlen(U);
	int strlenOfW = strlen(W);
	fwrite(&strlenOfE,sizeOfInt,1,fp);
	fwrite(&strlenOfF,sizeOfInt,1,fp);
	fwrite(&strlenOfU,sizeOfInt,1,fp);
	fwrite(&strlenOfW,sizeOfInt,1,fp);
	fwrite(C_r->E,strlenOfE,1,fp);
	fwrite(     F,strlenOfF,     1,fp);
	fwrite(     U,strlenOfU,     1,fp);
	fwrite(     W,strlenOfW,     1,fp);
	BN_CTX_free(ctx);
	
	fclose(fp);
}

void PRE_IO_key2file(char* dir,char* fileName,unsigned char *key,PRE_KEY *key1,PRE_KEY *key2)
{
	FILE *fp;
	if( 0 == mkdir(dir) ||
		NULL == ( fp = fopen(fileName,"wb+") ))
		exit(1);

	/*Write order：
	 * E->F->U->W
	 */
	PRE_REKEY *reKey = new PRE_REKEY(key1,key2->pk);
	Cipher *C = Encrypt(key1->pk,key);
	Cipher_REKEY *C_r = ReEncrypt(reKey,C);

	BN_CTX *ctx = BN_CTX_new();
	char *F = EC_POINT_point2hex(params->G,C_r->F,PRE_POINT_CONVERSION,ctx);
	char *U = EC_POINT_point2hex(params->G,C_r->U,PRE_POINT_CONVERSION,ctx);
	char *W = EC_POINT_point2hex(params->G,C_r->W,PRE_POINT_CONVERSION,ctx);
	int strlenOfE = 128;//strlen(C_r->E);
	int strlenOfF = strlen(F);
	int strlenOfU = strlen(U);
	int strlenOfW = strlen(W);
	fwrite(&strlenOfE,sizeOfInt,1,fp);
	fwrite(&strlenOfF,sizeOfInt,1,fp);
	fwrite(&strlenOfU,sizeOfInt,1,fp);
	fwrite(&strlenOfW,sizeOfInt,1,fp);
	fwrite(C_r->E,strlenOfE,1,fp);
	fwrite(     F,strlenOfF,     1,fp);
	fwrite(     U,strlenOfU,     1,fp);
	fwrite(     W,strlenOfW,     1,fp);
	fclose(fp);
}

void PRE_IO_file2key(char*filePath,unsigned char **key,PRE_KEY *key2)
{
	FILE *fp;
	if( NULL == ( fp = fopen(filePath,"rb") ))
		exit(1);

	Cipher_REKEY *C_r = new Cipher_REKEY();
	BN_CTX *ctx = BN_CTX_new();
	/*Read Order：
	 * E->F->U->W
	 */
	int strlenOfE = 0;
	int strlenOfF = 0;
	int strlenOfU = 0;
	int strlenOfW = 0;
	fread(&strlenOfE,sizeOfInt,1,fp);
	fread(&strlenOfF,sizeOfInt,1,fp);
	fread(&strlenOfU,sizeOfInt,1,fp);
	fread(&strlenOfW,sizeOfInt,1,fp);
	C_r->E = (unsigned char*)OPENSSL_malloc(strlenOfE);
	char *F = (char*)OPENSSL_malloc(strlenOfF);
	char *U = (char*)OPENSSL_malloc(strlenOfU);
	char *W = (char*)OPENSSL_malloc(strlenOfW);
	fread(C_r->E,strlenOfE,1,fp);
	fread(F,strlenOfF,1,fp);
	fread(U,strlenOfU,1,fp);
	fread(W,strlenOfW,1,fp);

	EC_POINT_hex2point(params->G,F,C_r->F,ctx);
	EC_POINT_hex2point(params->G,U,C_r->U,ctx);
	EC_POINT_hex2point(params->G,W,C_r->W,ctx);
	*key = Decrypt(key2,C_r);
	BN_CTX_free(ctx);

	fclose(fp);
}

void PRE_IO_Cipher2file(Cipher_REKEY *C,EC_GROUP *G,char *file)
{
	FILE *fp;
	if ( NULL == (fp = fopen( file,"wb")) )exit(1);

	BN_CTX *ctx = BN_CTX_new();
	char *F = EC_POINT_point2hex(G,C->F,PRE_POINT_CONVERSION,ctx);
	char *U = EC_POINT_point2hex(G,C->U,PRE_POINT_CONVERSION,ctx);
	char *W = EC_POINT_point2hex(G,C->W,PRE_POINT_CONVERSION,ctx);
	fwrite(C->E,sizeof(C->E),1,fp);
	fwrite(   F,sizeof(F),   1,fp);
	fwrite(   U,sizeof(U),   1,fp);
	fwrite(   W,sizeof(W),   1,fp);
	
	BN_CTX_free(ctx);

	fclose(fp);
}