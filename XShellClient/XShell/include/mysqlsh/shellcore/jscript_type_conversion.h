/*
   Copyright (c) 2014, 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _JSCRIPT_TYPE_CONVERSION_H_
#define _JSCRIPT_TYPE_CONVERSION_H_

#include "shellcore/types.h"
#include "shellcore/include_v8.h"

namespace shcore {
  class JScript_context;

  struct JScript_type_bridger
  {
    JScript_type_bridger(JScript_context *context);
    ~JScript_type_bridger();

    void init();
    void dispose();

    Value v8_value_to_shcore_value(const v8::Handle<v8::Value> &value);
    v8::Handle<v8::Value> shcore_value_to_v8_value(const Value &value);

    v8::Handle<v8::String> type_info(v8::Handle<v8::Value> value);

    double call_num_method(v8::Handle<v8::Object> object, const char *method);

    v8::Handle<v8::Value> native_object_to_js(Object_bridge_ref object);
    Object_bridge_ref js_object_to_native(v8::Handle<v8::Object> object);

    JScript_context *owner;

    class JScript_object_wrapper *object_wrapper;
    class JScript_object_wrapper *indexed_object_wrapper;
    class JScript_function_wrapper *function_wrapper;
    class JScript_map_wrapper *map_wrapper;
    class JScript_array_wrapper *array_wrapper;
  };
};

#endif
