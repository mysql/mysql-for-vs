/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/


#ifndef _MYSQLD_CONTEXT_SSL_H_
#define _MYSQLD_CONTEXT_SSL_H_

#include <boost/asio/buffer.hpp>
#include <queue>


namespace mysqld
{

  void set_context(SSL_CTX* ssl_context, const bool is_client, const std::string &ssl_key,
                   const std::string &ssl_cert,    const std::string &ssl_ca,
                   const std::string &ssl_ca_path, const std::string &ssl_cipher,
                   const std::string &ssl_crl,     const std::string &ssl_crl_path);

}  // namespace mysqld


#endif // _MYSQLD_CONTEXT_SSL_H_
