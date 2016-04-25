/*
   Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _MOD_COMMON_H_
#define _MOD_COMMON_H_

#ifdef _WIN32
# ifdef _DLL
#  ifdef mysqlshmods_EXPORTS
#   define MOD_PUBLIC __declspec(dllexport)
#  else
#   define MOD_PUBLIC __declspec(dllimport)
#  endif
# else
#  define MOD_PUBLIC
# endif
#else
# define MOD_PUBLIC
#endif

#ifdef DOXYGEN
// These data types are needed for documentation of the Dev-API
#define String std::string
#define Integer int
#define Bool bool
#define Map void
#define List void
#define Undefined void
#define Resultset void
#define Null void
#define CollectionFindRef void
#define ExecuteOptions int
#endif

#endif