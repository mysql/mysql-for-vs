/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_MYSQLXTEST_UTILS_H_
#define _MOD_MYSQLXTEST_UTILS_H_

#include "shellcore/types_cpp.h"
#include "shellcore/common.h"
#include "mysqlx.h"

#ifdef __GNUC__
#define ATTR_UNUSED __attribute__((unused))
#else
#define ATTR_UNUSED
#endif

/*
* Helper function to ensure the exceptions generated on the mysqlx_connector
* are properly translated to the corresponding shcore::Exception type
*/
static void ATTR_UNUSED translate_crud_exception(const std::string& operation)
{
  try
  {
    throw;
  }
  catch (shcore::Exception &e)
  {
    throw shcore::Exception::argument_error(operation + ": " + e.what());
  }
  catch (::mysqlx::Error &e)
  {
    throw shcore::Exception::mysql_error_with_code(e.what(), e.error());
  }
  catch (std::runtime_error &e)
  {
    throw shcore::Exception::runtime_error(operation + ": " + e.what());
  }
  catch (std::logic_error &e)
  {
    throw shcore::Exception::logic_error(operation + ": " + e.what());
  }
  catch (...)
  {
    throw;
  }
}

#define CATCH_AND_TRANSLATE_CRUD_EXCEPTION(operation)   \
  catch (...)                   \
{ translate_crud_exception(operation); }

/*
* Helper function to ensure the exceptions generated on the mysqlx_connector
* are properly translated to the corresponding shcore::Exception type
*/
static void ATTR_UNUSED translate_exception()
{
  try
  {
    throw;
  }
  catch (::mysqlx::Error &e)
  {
    throw shcore::Exception::mysql_error_with_code(e.what(), e.error());
  }
  catch (std::runtime_error &e)
  {
    throw shcore::Exception::runtime_error(e.what());
  }
  catch (std::logic_error &e)
  {
    throw shcore::Exception::logic_error(e.what());
  }
  catch (...)
  {
    throw;
  }
}

#define CATCH_AND_TRANSLATE()   \
  catch (...)                   \
{ translate_exception(); }

#endif
