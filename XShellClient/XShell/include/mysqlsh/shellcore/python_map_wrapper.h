/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PYTHON_MAP_WRAPPER_H_
#define _PYTHON_MAP_WRAPPER_H_

#include "shellcore/python_context.h"
#include "shellcore/types.h"

namespace shcore
{
  /*
   * Wraps an map object as a Python sequence object
   */
  struct PyShDictObject
  {
    PyObject_HEAD
    shcore::Value::Map_type_ref *map;
  };

  PyObject *wrap(boost::shared_ptr<Value::Map_type> map);
  bool unwrap(PyObject *value, boost::shared_ptr<Value::Map_type> &ret_object);

};

#endif
