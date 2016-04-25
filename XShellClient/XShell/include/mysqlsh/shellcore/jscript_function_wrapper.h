/*
   Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


// Provides a generic wrapper for shcore::Function objects so that they
// can be used from JavaScript

#ifndef _JSCRIPT_FUNCTION_WRAPPER_H_
#define _JSCRIPT_FUNCTION_WRAPPER_H_

#include "shellcore/types.h"
#include "shellcore/include_v8.h"

namespace shcore
{
class JScript_context;

class JScript_function_wrapper
{
public:
  JScript_function_wrapper(JScript_context *context);
  ~JScript_function_wrapper();

  v8::Handle<v8::Object> wrap(boost::shared_ptr<Function_base> object);

  static bool unwrap(v8::Handle<v8::Object> value, boost::shared_ptr<Function_base> &ret_function);

private:
  struct Collectable;
  static void call(const v8::FunctionCallbackInfo<v8::Value>& args);

  static void wrapper_deleted(const v8::WeakCallbackData<v8::Object, Collectable>& data);

private:
  JScript_context *_context;
  v8::Persistent<v8::ObjectTemplate> _object_template;
};

};


#endif
