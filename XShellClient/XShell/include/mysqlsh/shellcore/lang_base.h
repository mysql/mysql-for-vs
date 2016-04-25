/*
   Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _LANG_BASE_H_
#define _LANG_BASE_H_

#include <boost/system/error_code.hpp>
#include <string>

#include "shellcore/types_common.h"
#include "common.h"

namespace shcore {
  struct TYPES_COMMON_PUBLIC Interpreter_delegate
  {
    void *user_data;
    void(*print)(void *user_data, const char *text);
    bool(*prompt)(void *user_data, const char *prompt, std::string &ret_input);
    bool(*password)(void *user_data, const char *prompt, std::string &ret_password);
    void(*source)(void *user_data, const char *module);

    void(*print_error)(void *user_data, const char *text);
    void(*print_error_code)(void *user_data, const char *message, const boost::system::error_code &error);
  };
};

#endif
