/*
   Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _SHELLCORE_JS_H_
#define _SHELLCORE_JS_H_

#include "shellcore/shell_core.h"

namespace shcore {
  class JScript_context;

  class Shell_javascript : public Shell_language
  {
  public:
    Shell_javascript(Shell_core *shcore);

    virtual void set_global(const std::string &name, const Value &value);

    virtual void handle_input(std::string &code, Interactive_input_state &state, boost::function<void(shcore::Value)> result_processor);

    virtual std::string prompt();
    virtual void abort();
  private:
    boost::shared_ptr<JScript_context> _js;
  };
};

#endif
