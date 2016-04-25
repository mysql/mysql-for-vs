/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/
#ifndef _UTILS_MYSQL_PARSING_H_
#define _UTILS_MYSQL_PARSING_H_

#define SPACES " \t\r\n"

#include <string>
#include <vector>
#include <stack>

namespace shcore
{
  namespace mysql
  {
    namespace splitter
    {
      // String SQL parsing functions (from WB)
      const unsigned char* skip_leading_whitespace(const unsigned char *head, const unsigned char *tail);
      bool is_line_break(const unsigned char *head, const unsigned char *line_break);
      size_t determineStatementRanges(const char *sql, size_t length, std::string &delimiter,
                                      std::vector<std::pair<size_t, size_t> > &ranges,
                                      const std::string &line_break, std::stack<std::string> &input_context_stack);
    }
  }
}

#endif