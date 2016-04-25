/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_TABLE_CRUD_DEFINITION_H_
#define _MOD_TABLE_CRUD_DEFINITION_H_

#include "shellcore/types_cpp.h"
#include "shellcore/common.h"
#include "crud_definition.h"
#include "mysqlx_crud.h"
#include "mysqlxtest_utils.h"

#include <boost/weak_ptr.hpp>
#include <boost/enable_shared_from_this.hpp>

#include <set>

#ifdef __GNUC__
#define ATTR_UNUSED __attribute__((unused))
#else
#define ATTR_UNUSED
#endif

namespace mysh
{
  namespace mysqlx
  {
    class Table_crud_definition : public Crud_definition
    {
    public:
      Table_crud_definition(boost::shared_ptr<DatabaseObject> owner) :Crud_definition(owner){}

    protected:
      ::mysqlx::TableValue map_table_value(shcore::Value source);
    };
  }
}

#endif
