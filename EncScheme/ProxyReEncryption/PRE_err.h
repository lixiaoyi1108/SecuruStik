/**
	* @Copyright (c) 2014,SecuruStik Technology Limited,All rights reserved.
	* @date		:	5/7/2014   19:29

	* @filename	: 	PRE_ERR.h
	* @file path:	Cryptology\ProxyReEncryption\ProxyReEncryption
	* @file base:	PRE_ERR
	* @file ext	:	h

	* @author	:	JianYe Huang
	
	* @brief	:	
*/
#pragma once
#include<stdio.h>
#ifndef DEBUG//Debug mode
#define CHECK_IF_ERROR(ret) if( 0 == ret )\
							{\
								PRE_ERR_GetLastError(__FILE__,__FUNCTION__,__LINE__);\
								return NULL;\
							}
#else
#define CHECK_IF_ERROR(ret) ;
#endif

/************************************************************************/
/* Err lib init & cleanup                                                  */
/************************************************************************/
void PRE_err_init();
void PRE_err_cleanup();

/**
 * @name      GetLastError
 * @brief	  获取上一次失败代码
 * modifier  public 
 * qualifier
 * @return	  Success:失败代码(unsigned long)
 *            Failed:0
 * @retval	  
 */
unsigned long PRE_ERR_GetLastError();

/**
 * @name      GetLastErrorString
 * @brief	  获取上一次失败信息(不含调试信息)
 * modifier  public 
 * qualifier
 * @return	  Success:失败信息(char*)
 *            Failed:NULL
 *			  字符样式：Error : ErrorCode headerFile.h codeFile.cpp
 *						@FileName(line:%d)
 *					    @FunctionName
 * @retval	  
 */
char* GetLastErrorString();

/**
 * @name      GetLastError
 * @brief	  获取上一次失败信息(含调试信息)
 * modifier  public 
 * qualifier
 * @param	  char * filePath 文件路径
 * @param	  char * function 函数名称
 * @param	  int line 错误所在行
 * @return	  Success:失败信息(unsigned long)
 *            Failed:NULL
 * @retval	  
 */
unsigned long PRE_ERR_GetLastError(char* filePath,char* function,int line);

/**
 * @name      GetErrorString
 * @brief	  根据错误代码获取失败信息
 * modifier  public 
 * qualifier
 * @param	  unsigned long errCode
 * @return	  Success:(char*)
 *            Failed:
 * @retval	  
 */
char* GetErrorString(unsigned long errCode);

/**
 * @name      SetLastError
 * @brief	  设置错误码
 * modifier  public 
 * qualifier
 * @param	  int ec 错误代码
 * @return	  Success:1(unsigned long)
 *            Failed:0
 * @retval	  
 */
unsigned long SetLastError(int ec);