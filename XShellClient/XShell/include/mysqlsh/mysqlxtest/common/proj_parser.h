/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _EXPR_PARSER_PROJ_H_
#define _EXPR_PARSER_PROJ_H_

#define _EXPR_PARSER_HAS_PROJECTION_KEYWORDS_ 1

#include <boost/format.hpp>
#include "expr_parser.h"
#include "mysqlx_crud.pb.h"
#include "../compilerutils.h"

#include <memory>

namespace mysqlx
{
  class Proj_parser : public Expr_parser
  {
  public:
    Proj_parser(const std::string& expr_str, bool document_mode = false, bool allow_alias = true);

    template<typename Container>
    void parse(Container &result)
    {
      Mysqlx::Crud::Projection *colid = result.Add();
      source_expression(*colid);

      if (_tokenizer.tokens_available())
      {
        const mysqlx::Token& tok = _tokenizer.peek_token();
        throw Parser_error((boost::format("Projection parser: Expression '%s' has unexpected token '%s' at position %d") % _tokenizer.get_input() % tok.get_text() %
          tok.get_pos()).str());
      }
    }

    const std::string& id();
    void source_expression(Mysqlx::Crud::Projection &column);
  };
};
#endif
