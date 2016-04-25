/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PYTHON_TYPE_CONVERSION_H_
#define _PYTHON_TYPE_CONVERSION_H_

#include <Python.h>
#include "shellcore/types.h"

namespace shcore {

class Python_context;

struct Python_type_bridger
{
  Python_type_bridger(Python_context *context);
  ~Python_type_bridger();

  void init();

  Value pyobj_to_shcore_value(PyObject *value) const;
  PyObject *shcore_value_to_pyobj(const Value &value);

  PyObject *native_object_to_py(Object_bridge_ref object);

  Python_context *_owner;
};

}

#endif
