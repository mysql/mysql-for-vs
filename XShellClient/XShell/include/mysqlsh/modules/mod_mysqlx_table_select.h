/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_CRUD_TABLE_SELECT_H_
#define _MOD_CRUD_TABLE_SELECT_H_

#include "table_crud_definition.h"

namespace mysh
{
  namespace mysqlx
  {
    class Table;

    /**
    * Handler for record selection on a Table.
    *
    * This object provides the necessary functions to allow selecting record data from a table.
    *
    * This object should only be created by calling the select function on the table object from which the record data will be retrieved.
    *
    * \sa Table
    */
    class TableSelect : public Table_crud_definition, public boost::enable_shared_from_this<TableSelect>
    {
    public:
      TableSelect(boost::shared_ptr<Table> owner);
    public:
      virtual std::string class_name() const { return "TableSelect"; }
      shcore::Value select(const shcore::Argument_list &args);
      shcore::Value where(const shcore::Argument_list &args);
      shcore::Value group_by(const shcore::Argument_list &args);
      shcore::Value having(const shcore::Argument_list &args);
      shcore::Value order_by(const shcore::Argument_list &args);
      shcore::Value limit(const shcore::Argument_list &args);
      shcore::Value offset(const shcore::Argument_list &args);
      shcore::Value bind(const shcore::Argument_list &args);

      virtual shcore::Value execute(const shcore::Argument_list &args);
#ifdef DOXYGEN
      TableSelect select(List searchExprStr);
      TableSelect where(String searchCondition);
      TableSelect groupBy(List searchExprStr);
      TableSelect having(String searchCondition);
      TableSelect orderBy(List sortExprStr);
      TableSelect limit(Integer numberOfRows);
      TableSelect offset(Integer limitOffset);
      TableSelect bind(String name, Value value);
      RowResult execute();
#endif
    private:
      std::unique_ptr< ::mysqlx::SelectStatement> _select_statement;
    };
  };
};

#endif
