/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


#ifndef _MYSQLXTEST_COMMON_H_
 #define _MYSQLXTEST_COMMON_H_


#ifdef _WIN32
 # ifdef _DLL
 #  ifdef MYSQLXTEST_EXPORTS
 #   define MYSQLXTEST_PUBLIC __declspec(dllexport)
 #  else
 #   define MYSQLXTEST_PUBLIC __declspec(dllimport)
 #  endif
 # else
 #  define MYSQLXTEST_PUBLIC
 # endif
 #else
 # define MYSQLXTEST_PUBLIC
 #endif

#ifdef NO_MYSQLXTEST
 # undef MYSQLXTEST_PUBLIC
 # define MYSQLXTEST_PUBLIC SHCORE_PUBLIC
 #endif

#endif

