/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


#ifndef _TYPES_COMMON_H_
#define _TYPES_COMMON_H_


#ifdef _WIN32
# ifdef _DLL
#  ifdef SHCORE_EXPORT
#   define TYPES_COMMON_PUBLIC __declspec(dllexport)
#  else
#   define TYPES_COMMON_PUBLIC __declspec(dllimport)
#  endif
# else
#  define TYPES_COMMON_PUBLIC
# endif
#else
# define TYPES_COMMON_PUBLIC
#endif


#ifdef No_mysqlshtypes
# undef TYPES_COMMON_PUBLIC
# define TYPES_COMMON_PUBLIC SHCORE_PUBLIC
#endif

#endif