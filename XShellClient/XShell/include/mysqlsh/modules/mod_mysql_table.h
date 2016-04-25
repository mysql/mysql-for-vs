/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// Interactive ClassicTable access module
// (the one exposed as the table members of the db object in the shell)

#ifndef _MOD_MYSQL_TABLE_H_
#define _MOD_MYSQL_TABLE_H_

#include "base_database_object.h"
#include "shellcore/types.h"
#include "shellcore/types_cpp.h"

namespace mysh
{
  namespace mysql
  {
    class ClassicSchema;

    /**
    * Represents a ClassicTable on an ClassicSchema, retrieved with a session created using the MySQL Protocol.
    */
    class ClassicTable : public DatabaseObject
    {
    public:
      ClassicTable(boost::shared_ptr<ClassicSchema> owner, const std::string &name);
      ClassicTable(boost::shared_ptr<const ClassicSchema> owner, const std::string &name);
      virtual ~ClassicTable();

      virtual std::string class_name() const { return "ClassicTable"; }
    };
  }
}

#endif
