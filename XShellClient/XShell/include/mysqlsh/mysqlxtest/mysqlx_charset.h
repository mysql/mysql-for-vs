/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _MYSQLX_CHARSET_H_
#define _MYSQLX_CHARSET_H_

#include <string>
#include <stdint.h>

namespace mysqlx
{
  class Charset
  {
  public:
    static std::string charset_name_from_id(uint32_t id);
    static std::string collation_name_from_id(uint32_t id);
    static uint32_t id_from_collation_name(const std::string& collation_name);

  private:

    typedef struct {
      uint32_t id;
      std::string name;
      std::string collation;
    } Charset_entry;

    static const Charset_entry  m_charsets_info[];

    static std::string field_from_id(uint32_t id, std::string Charset_entry::*field);
  };
}


#endif
