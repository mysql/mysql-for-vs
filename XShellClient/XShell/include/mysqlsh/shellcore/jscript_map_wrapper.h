/*
   Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


#ifndef _JSCRIPT_MAP_WRAPPER_H_
#define _JSCRIPT_MAP_WRAPPER_H_

#include "shellcore/types.h"
#include "shellcore/include_v8.h"

namespace shcore
{
class JScript_context;

class JScript_map_wrapper
{
public:
  JScript_map_wrapper(JScript_context *context);
  ~JScript_map_wrapper();

  v8::Handle<v8::Object> wrap(boost::shared_ptr<Value::Map_type> map);

  static bool unwrap(v8::Handle<v8::Object> value, boost::shared_ptr<Value::Map_type> &ret_map);

private:
  struct Collectable;

  static void handler_getter(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info);
  static void handler_setter(v8::Local<v8::String> property, v8::Local<v8::Value> value, const v8::PropertyCallbackInfo<v8::Value>& info);
  static void handler_enumerator(const v8::PropertyCallbackInfo<v8::Array>& info);

  static void wrapper_deleted(const v8::WeakCallbackData<v8::Object, Collectable>& data);

private:
  JScript_context *_context;
  v8::Persistent<v8::ObjectTemplate> _map_template;
};

};


#endif
