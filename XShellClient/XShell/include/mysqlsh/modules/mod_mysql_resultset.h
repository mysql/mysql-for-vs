/*
   Copyright (c) 2015 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_RESULT_H_
#define _MOD_RESULT_H_

#include "shellcore/types.h"
#include "shellcore/types_cpp.h"
#include "base_resultset.h"
#include <list>

namespace mysh
{
  namespace mysql
  {
    class Result;

    /**
    * Allows browsing through the result information after performing an operation on the database through the MySQL Protocol.
    * This class allows access to the result set from the classic MySQL data model to be retrieved from Dev API queries.
    */
    class SHCORE_PUBLIC ClassicResult : public ShellBaseResult
    {
    public:
      ClassicResult(boost::shared_ptr<Result> result);

      virtual std::string class_name() const { return "ClassicResult"; }
      virtual std::vector<std::string> get_members() const;
      virtual shcore::Value get_member(const std::string &prop) const;
      virtual void append_json(shcore::JSON_dumper& dumper) const;

      shcore::Value has_data(const shcore::Argument_list &args) const;
      virtual shcore::Value fetch_one(const shcore::Argument_list &args) const;
      virtual shcore::Value fetch_all(const shcore::Argument_list &args) const;
      virtual shcore::Value next_data_set(const shcore::Argument_list &args);

    protected:
      boost::shared_ptr<Result> _result;

#ifdef DOXYGEN
      Integer affectedRowCount; //!< Same as getAffectedItemCount()
      Integer columnCount; //!< Same as getcolumnCount()
      List columnNames; //!< Same as getColumnNames()
      List columns; //!< Same as getColumns()
      String executionTime; //!< Same as getExecutionTime()
      String info; //!< Same as getInfo()
      Integer lastInsertId; //!< Same as getLastInsertId()
      List warnings; //!< Same as getWarnings()
      Integer warningCount; //!< Same as getWarningCount()

      Row fetchOne();
      List fetchAll();
      Integer getAffectedRowCount();
      Integer getColumnCount();
      List getColumnNames();
      List getColumns();
      String getExecutionTime();
      Bool hasData();
      String getInfo();
      Integer getLastInsertId();
      Integer getWarningCount();
      List getWarnings();
      Bool nextDataSet();
#endif
    };
  }
};

#endif
