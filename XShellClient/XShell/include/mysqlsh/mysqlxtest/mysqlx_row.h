/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _MYSQLX_ROW_H_
#define _MYSQLX_ROW_H_

#include <string>
#include <set>
#include <stdint.h>
#include <google/protobuf/io/coded_stream.h>

namespace mysqlx
{
  class DateTime;
  class Time;
  class Decimal;

  class Row_decoder
  {
  public:
    /* static methods to decode from protobuf format to specific types */
    static uint64_t u64_from_buffer(const std::string& buffer);
    static int64_t s64_from_buffer(const std::string& buffer);
    static const char *string_from_buffer(const std::string& buffer, size_t &rlength);
    static float float_from_buffer(const std::string& buffer);
    static double double_from_buffer(const std::string& buffer);
    static DateTime datetime_from_buffer(const std::string& buffer);
    static Time time_from_buffer(const std::string& buffer);
    static Decimal decimal_from_buffer(const std::string& buffer);
    static void set_from_buffer(const std::string& buffer, std::set<std::string>& result);
    static std::string set_from_buffer_as_str(const std::string& buffer);

  private:

    static void read_required_uint64(
      google::protobuf::io::CodedInputStream& input_buffer,
      google::protobuf::uint64& result
      );
  };
};

#endif
