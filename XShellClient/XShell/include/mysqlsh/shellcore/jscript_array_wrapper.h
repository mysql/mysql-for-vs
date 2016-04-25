/*
   Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


#ifndef _JSCRIPT_ARRAY_WRAPPER_H_
#define _JSCRIPT_ARRAY_WRAPPER_H_

#include "shellcore/types.h"
#include <shellcore/include_v8.h>

namespace shcore
{
class JScript_context;

class JScript_array_wrapper
{
public:
  JScript_array_wrapper(JScript_context *context);
  ~JScript_array_wrapper();

  v8::Handle<v8::Object> wrap(boost::shared_ptr<Value::Array_type> array);

  static bool unwrap(v8::Handle<v8::Object> value, boost::shared_ptr<Value::Array_type> &ret_array);

private:
  struct Collectable;
  static void handler_igetter(uint32_t index, const v8::PropertyCallbackInfo<v8::Value>& info);
  static void handler_ienumerator(const v8::PropertyCallbackInfo<v8::Array>& info);
  static void handler_getter(v8::Local<v8::String> prop, const v8::PropertyCallbackInfo<v8::Value>& info);


  static void wrapper_deleted(const v8::WeakCallbackData<v8::Object, Collectable>& data);

private:
  JScript_context *_context;
  v8::Persistent<v8::ObjectTemplate> _array_template;
};

};


#endif
