#pragma once
#include "PRE.h"
#include "KeyGen.h"
#include "Encrypt_Decrypt.h"

/************************************************************************/
/* APIs export for dll                                                    */
/************************************************************************/

PRE_API void PRE_Setup();
PRE_API void PRE_UnSetup();

//Key crypto
PRE_API PRE_KEY_str* PRE_KEYstr_Create();
PRE_API CipherStr_REKEY* PRE_Encrypt(PRE_KEY_str *key_str,PRE_PK_str* pkj_str, unsigned char* m);
PRE_API unsigned char* PRE_Decrypt(PRE_KEY_str *key_str,CipherStr_REKEY *strC);

PRE_API int blockSize = BLOCK_SIZE;
