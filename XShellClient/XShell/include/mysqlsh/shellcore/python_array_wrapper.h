/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PYTHON_ARRAY_WRAPPER_H_
#define _PYTHON_ARRAY_WRAPPER_H_

#include "shellcore/python_context.h"
#include "shellcore/types.h"

namespace shcore
{
class Python_context;

/*
 * Wraps an array object as a Python sequence object
 */
struct PyShListObject
{
  PyObject_HEAD
  shcore::Value::Array_type_ref *array;
};


PyObject *wrap(boost::shared_ptr<Value::Array_type> array);
bool unwrap(PyObject *value, boost::shared_ptr<Value::Array_type> &ret_array);

};

#endif
