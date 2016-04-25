/*
   Copyright (c) 2015, 2016, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _SHELLCORE_PYTHON_H_
#define _SHELLCORE_PYTHON_H_

#include "shellcore/shell_core.h"

namespace shcore {
  class Python_context;

  class Shell_python : public Shell_language
  {
  public:
    Shell_python(Shell_core *shcore);
    virtual ~Shell_python();

    virtual void set_global(const std::string &name, const Value &value);

    virtual std::string preprocess_input_line(const std::string &s);
    virtual void handle_input(std::string &code, Interactive_input_state &state, boost::function<void(shcore::Value)> result_processor);

    virtual std::string prompt();
    virtual void abort();
  private:
    boost::shared_ptr<Python_context> _py;
  };
};

#endif
