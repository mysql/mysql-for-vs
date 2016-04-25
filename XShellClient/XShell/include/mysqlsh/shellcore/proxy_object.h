/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PROXY_OBJECT_H_
#define _PROXY_OBJECT_H_

#include "shellcore/types_common.h"
#include "types_cpp.h"

namespace shcore {

class SHCORE_PUBLIC Proxy_object : public shcore::Cpp_object_bridge
{
public:
  virtual std::string class_name() const { return "Proxy_object"; }

  Proxy_object(const boost::function<Value (const std::string&)> &delegate);

  virtual Value get_member(const std::string &prop) const;

  virtual bool operator == (const Object_bridge &other) const
  {
    return this == &other;
  }

private:
  boost::function<Value (const std::string&)> _delegate;
};

};

#endif
