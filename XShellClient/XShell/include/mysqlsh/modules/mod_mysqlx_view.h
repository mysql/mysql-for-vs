/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// Interactive Table access module
// (the one exposed as the table members of the db object in the shell)

#ifndef _MOD_MYSQLX_VIEW_H_
#define _MOD_MYSQLX_VIEW_H_

#include "base_database_object.h"
#include "shellcore/types.h"
#include "shellcore/types_cpp.h"

namespace mysh
{
  namespace mysqlx
  {
    class Schema;

    /**
    * Represents a View on an Schema, retrieved with a session created using the X Protocol.
    * \todo Implement and document select()
    */
    class View : public DatabaseObject
    {
    public:
      View(boost::shared_ptr<Schema> owner, const std::string &name);
      View(boost::shared_ptr<const Schema> owner, const std::string &name);
      virtual ~View();

      virtual std::string class_name() const { return "View"; }
    };
  }
}

#endif
