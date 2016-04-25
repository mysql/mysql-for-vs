/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PYTHON_OBJECT_WRAPPER_H_
#define _PYTHON_OBJECT_WRAPPER_H_

#include "shellcore/python_context.h"
#include "shellcore/types.h"

namespace shcore
{
  class Python_context;

  struct PyMemberCache
  {
    std::map<std::string, AutoPyObject> members;
  };

  /*
   * Wraps a native/bridged C++ object reference as a Python sequence object
   */
  struct PyShObjObject
  {
    PyObject_HEAD
    shcore::Object_bridge_ref *object;
    PyMemberCache *cache;
  };

  struct PyShObjIndexedObject
  {
    PyObject_HEAD
    shcore::Object_bridge_ref *object;
    PyMemberCache *cache;
  };

  PyObject *wrap(boost::shared_ptr<Object_bridge> object);
  bool unwrap(PyObject *value, boost::shared_ptr<Object_bridge> &ret_object);
};

#endif  // _PYTHON_OBJECT_WRAPPER_H_
