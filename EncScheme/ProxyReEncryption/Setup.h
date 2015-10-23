/** 
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	5/7/2014   19:26

	* @filename	: 	Setup.h
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	Setup
	* @file ext	:	h

	* @author	:	JianYe Huang
	
	* @brief	:	
	*/
#pragma once
#include "PRE.h"
#include <openssl/ec.h>
// Hash(Ellipic point => binary string)
unsigned char* Hash_point2bin(const EC_POINT *point);

// Get a curve nid(0~66) defaultly
// There 67 built-in curves in the openssl library.
const int CURVE_INDEX = 57;

//Initialize & free the security parameter
PRE_API void PRE_Setup();
PRE_API void PRE_UnSetup();