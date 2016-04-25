/*
   Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// Interactive Expression access module
// Exposed as "Expression" in the shell

#ifndef _TANTS_H_
#define _MOD_MYSQLX_CONSTANTS_H_

#include "mod_common.h"
#include "shellcore/types.h"
#include "shellcore/types_cpp.h"

namespace mysh
{
  namespace mysqlx
  {
    /**
    * Constants to represent data types con Column objects
    *
    * Supported Data Types
    *
    *  - Bit
    *  - TinyInt
    *  - SmallInt
    *  - MediumInt
    *  - Int
    *  - BigInt
    *  - Float
    *  - Decimal
    *  - Double
    *  - Json
    *  - String
    *  - Bytes
    *  - Time
    *  - Date
    *  - DateTime
    *  - Timestamp
    *  - Set
    *  - Enum
    *  - Geometry
    */
    class SHCORE_PUBLIC Type : public shcore::Cpp_object_bridge
    {
    public:
      // Virtual methods from object bridge
      virtual std::string class_name() const { return "mysqlx.Type"; };
      virtual bool operator == (const Object_bridge &other) const { return this == &other; };

      virtual shcore::Value get_member(const std::string &prop) const;

      std::vector<std::string> get_members() const;

      static boost::shared_ptr<shcore::Object_bridge> create(const shcore::Argument_list &args);
    };

    class SHCORE_PUBLIC IndexType : public shcore::Cpp_object_bridge
    {
    public:
      // Virtual methods from object bridge
      virtual std::string class_name() const { return "mysqlx.IndexType"; };
      virtual bool operator == (const Object_bridge &other) const { return this == &other; }

      virtual shcore::Value get_member(const std::string &prop) const;
      std::vector<std::string> get_members() const;

      static boost::shared_ptr<shcore::Object_bridge> create(const shcore::Argument_list &args);
    };
  };
};

#endif
