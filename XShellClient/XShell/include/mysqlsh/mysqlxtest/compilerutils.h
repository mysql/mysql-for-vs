/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _COMPILERUTILS_H_
#define _COMPILERUTILS_H_

#ifdef UNUSED
#  elif defined(__GNUC__)
#    define UNUSED(x) UNUSED_ ## x __attribute__((unused))
#  elif defined(__LCLINT__)
#    define UNUSED(x) /*@unused@*/ x
#  elif defined(__cplusplus)
#    define UNUSED(x)
#  else
#    define UNUSED(x) x
#endif

#endif  // _COMPILERUTILS_H_
