/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// Interactive Expression access module
// Exposed as "Expression" in the shell

#ifndef _MOD_MYSQLX_EXPRESSION_H_
#define _MOD_MYSQLX_EXPRESSION_H_

#include "mod_common.h"
#include "shellcore/types.h"
#include "shellcore/types_cpp.h"
#include "shellcore/ishell_core.h"

namespace mysh
{
  namespace mysqlx
  {
    class SHCORE_PUBLIC Expression : public shcore::Cpp_object_bridge
    {
    public:
      Expression(const std::string &expression) { _data = expression; }
      virtual ~Expression() {};

      // Virtual methods from object bridge
      virtual std::string class_name() const { return "Expression"; };
      virtual bool operator == (const Object_bridge &other) const;

      virtual shcore::Value get_member(const std::string &prop) const;
      std::vector<std::string> get_members() const;

      static boost::shared_ptr<shcore::Object_bridge> create(const shcore::Argument_list &args);

      std::string get_data() { return _data; };

    private:
      std::string _data;
    };
  };
};

#endif
