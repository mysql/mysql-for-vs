/*
   Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _SHCORE_COMMON_H_
#define _SHCORE_COMMON_H_

#include "shellcore/types_common.h"
#include "logger/logger.h"

#ifdef _WIN32
# ifdef _DLL
#  ifdef SHCORE_EXPORT
#   define SHCORE_PUBLIC __declspec(dllexport)
#  else
#   define SHCORE_PUBLIC __declspec(dllimport)
#  endif
# else
#  define SHCORE_PUBLIC
# endif
#else
# define SHCORE_PUBLIC
#endif

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

#include <boost/function.hpp>

namespace shcore
{
  extern void SHCORE_PUBLIC default_print(const std::string& text);
  extern SHCORE_PUBLIC boost::function<void(std::string)> print;
}

#endif
