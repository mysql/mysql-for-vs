/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _ORDERBY_PARSER_H_
#define _ORDERBY_PARSER_H_

#include <boost/format.hpp>
#include "expr_parser.h"
#include "mysqlx_crud.pb.h"
#include "../compilerutils.h"

#include <memory>

namespace mysqlx
{
  class Orderby_parser : public Expr_parser
  {
  public:
    Orderby_parser(const std::string& expr_str, bool document_mode = false);

    template<typename Container>
    void parse(Container &result)
    {
      Mysqlx::Crud::Order *colid = result.Add();
      column_identifier(*colid);

      if (_tokenizer.tokens_available())
      {
        const mysqlx::Token& tok = _tokenizer.peek_token();
        throw Parser_error((boost::format("Orderby parser: Expected EOF, instead stopped at token '%s' at position %d") % tok.get_text()
          % tok.get_pos()).str());
      }
    }

    //const std::string& id();
    void column_identifier(Mysqlx::Crud::Order &orderby_expr);

    std::vector<Token>::const_iterator begin() const { return _tokenizer.begin(); }
    std::vector<Token>::const_iterator end() const { return _tokenizer.end(); }
  };
};
#endif
