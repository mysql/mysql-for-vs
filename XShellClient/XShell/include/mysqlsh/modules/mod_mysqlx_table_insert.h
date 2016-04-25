/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_CRUD_TABLE_INSERT_H_
#define _MOD_CRUD_TABLE_INSERT_H_

#include "table_crud_definition.h"

namespace mysh
{
  namespace mysqlx
  {
    class Table;

    /**
    * Handler for Insert operations on Tables.
    */
    class TableInsert : public Table_crud_definition, public boost::enable_shared_from_this<TableInsert>
    {
    public:
      TableInsert(boost::shared_ptr<Table> owner);
    public:
      virtual std::string class_name() const { return "TableInsert"; }
      static boost::shared_ptr<shcore::Object_bridge> create(const shcore::Argument_list &args);
      shcore::Value insert(const shcore::Argument_list &args);
      shcore::Value values(const shcore::Argument_list &args);

      virtual shcore::Value execute(const shcore::Argument_list &args);
#ifdef DOXYGEN
      TableInsert insert();
      TableInsert insert(List columns);
      TableInsert insert(String col1, String col2, ...);
      TableInsert values(Value value, Value value, ...);
      Result execute();
#endif
    private:
      std::unique_ptr< ::mysqlx::InsertStatement> _insert_statement;
    };
  };
};

#endif
