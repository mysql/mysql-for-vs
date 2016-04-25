/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// Interactive DB access module
// (the one exposed as the db variable in the shell)

#ifndef _MOD_DB_OBJECT_H_
#define _MOD_DB_OBJECT_H_

#include "mod_common.h"
#include "shellcore/types.h"
#include "shellcore/types_cpp.h"

#include <boost/enable_shared_from_this.hpp>
#include <boost/weak_ptr.hpp>

namespace shcore
{
  class Proxy_object;
};

namespace mysh
{
  class ShellBaseSession;
  class CoreSchema;
  /**
  * Provides base functionality for database objects.
  */
  class SHCORE_PUBLIC DatabaseObject : public shcore::Cpp_object_bridge
  {
  public:
    DatabaseObject(boost::shared_ptr<ShellBaseSession> session, boost::shared_ptr<DatabaseObject> schema, const std::string &name);
    ~DatabaseObject();

    virtual std::string &append_descr(std::string &s_out, int indent = -1, int quote_strings = 0) const;
    virtual std::string &append_repr(std::string &s_out) const;
    virtual void append_json(shcore::JSON_dumper& dumper) const;

    virtual bool has_member(const std::string &prop) const;
    virtual std::vector<std::string> get_members() const;
    virtual shcore::Value get_member(const std::string &prop) const;

    virtual bool operator == (const Object_bridge &other) const;

    shcore::Value get_member_method(const shcore::Argument_list &args, const std::string& method, const std::string& prop);

    shcore::Value existsInDatabase(const shcore::Argument_list &args);

#ifdef DOXYGEN

    String name; //!< Same as getName()
    Object session; //!< Same as getSession()
    Object schema; //!< Same as getSchema()

    String getName();
    Object getSession();
    Object getSchema();

#endif

  protected:
    boost::weak_ptr<ShellBaseSession> _session;
    boost::weak_ptr<DatabaseObject> _schema;
    std::string _name;
  };
};

#endif
