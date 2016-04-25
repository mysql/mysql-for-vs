/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PYTHON_FUNCTION_WRAPPER_H_
#define _PYTHON_FUNCTION_WRAPPER_H_

#include "shellcore/python_context.h"
#include "shellcore/types.h"

namespace shcore
{
class Python_context;

/*
 * Wraps a native/bridged C++ function reference as a Python sequence object
 */
struct PyShFuncObject
{
  PyObject_HEAD
  shcore::Function_base_ref *func;
};

PyObject *wrap(boost::shared_ptr<Function_base> func);
bool unwrap(PyObject *value, boost::shared_ptr<Function_base> &ret_func);

};

#endif  // _PYTHON_FUNCTION_WRAPPER_H_
