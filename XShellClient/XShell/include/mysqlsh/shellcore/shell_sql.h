/*
   Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _SHELL_SQL_H_
#define _SHELL_SQL_H_

#include "shellcore/shell_core.h"
#include "shellcore/ishell_core.h"
#include "shellcore/common.h"
#include <boost/system/error_code.hpp>
#include <stack>

namespace shcore
{
  class SHCORE_PUBLIC Shell_sql : public Shell_language
  {
  public:
    Shell_sql(IShell_core *owner);
    virtual ~Shell_sql() {};

    virtual void set_global(const std::string &, const Value &) {}

    virtual void handle_input(std::string &code, Interactive_input_state &state, boost::function<void(shcore::Value)> result_processor);

    virtual std::string prompt();

    virtual bool print_help(const std::string& topic);
    void print_exception(const shcore::Exception &e);
    virtual void abort();

  private:
    std::string _sql_cache;
    std::string _delimiter;
    std::stack<std::string> _parsing_context_stack;

    void cmd_process_file(const std::vector<std::string>& params);
  };
};

#endif
