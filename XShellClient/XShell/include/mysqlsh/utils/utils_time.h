/*
   Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef __mysh__utils_time__
#define __mysh__utils_time__

#include "shellcore/types_common.h"
#include "shellcore/common.h"
#include <string>

class SHCORE_PUBLIC MySQL_timer
{
public:
  unsigned long get_time();
  unsigned long start();
  unsigned long end();
  unsigned long raw_duration() { return _end - _start; }
  static std::string format_legacy(unsigned long raw_time, int part_seconds, bool in_seconds = false);
  static void parse_duration(unsigned long raw_time, int &days, int &hours, int &minutes, float &seconds, bool in_seconds = false);

private:
  unsigned long _start;
  unsigned long _end;
};

#endif /* defined(__mysh__utils_time__) */
