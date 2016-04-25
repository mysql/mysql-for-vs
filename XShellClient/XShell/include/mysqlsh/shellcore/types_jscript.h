/*
   Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _TYPES_JSCRIPT_H_
#define _TYPES_JSCRIPT_H_

#include "shellcore/jscript_context.h"

#include "shellcore/types.h"

#include "shellcore/include_v8.h"

namespace shcore {

class SHCORE_PUBLIC JScript_function : public Function_base
{
public:
  JScript_function(boost::shared_ptr<JScript_context> context);
  virtual ~JScript_function() {}

  virtual std::string name();

  virtual std::vector<std::pair<std::string, Value_type> > signature();

  virtual std::pair<std::string, Value_type> return_type();

  virtual bool operator == (const Function_base &other) const;

  virtual bool operator != (const Function_base &other) const;

  virtual Value invoke(const Argument_list &args);

private:
  boost::shared_ptr<JScript_context> _js;
  v8::Handle<v8::Function> _jsfunc;
};


};

#endif
