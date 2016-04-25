/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_SQL_EXECUTE_H_
#define _MOD_SQL_EXECUTE_H_

#include "dynamic_object.h"

namespace mysh
{
  namespace mysqlx
  {
    class NodeSession;
    /**
    * Handler for execution SQL statements, supports parameter binding.
    *
    * This object should only be created by calling the sql function a NodeSession instance.
    * \sa NodeSession
    */
    class SqlExecute : public Dynamic_object, public boost::enable_shared_from_this<SqlExecute>
    {
    public:
      SqlExecute(boost::shared_ptr<NodeSession> owner);
      virtual std::string class_name() const { return "SqlExecute"; }
      shcore::Value sql(const shcore::Argument_list &args);
      shcore::Value bind(const shcore::Argument_list &args);
      virtual shcore::Value execute(const shcore::Argument_list &args);
#ifdef DOXYGEN
      SqlExecute sql(String statement);
      SqlExecute bind(Value value);
      SqlExecute bind(List values);
      SqlResult execute();
#endif
    private:
      boost::weak_ptr<NodeSession> _session;
      std::string _sql;
      shcore::Argument_list _parameters;
    };
  };
};

#endif
