/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _PYTHON_UTILS_H_
#define _PYTHON_UTILS_H_

#include <Python.h>

// Must be placed when Python code will be called
struct WillEnterPython
{
  PyGILState_STATE state;
  bool locked;

  WillEnterPython()
  : state(PyGILState_Ensure()), locked(true)
  {
  }

  ~WillEnterPython()
  {
    if (locked)
      PyGILState_Release(state);
  }

  void release()
  {
    if (locked)
      PyGILState_Release(state);
    locked = false;
  }
};


// Must be placed when non-python code will be called from a Python handler/callback
struct WillLeavePython
{
  PyThreadState *save;

  WillLeavePython()
  : save(PyEval_SaveThread())
  {
  }

  ~WillLeavePython()
  {
    PyEval_RestoreThread(save);
  }
};

#endif
