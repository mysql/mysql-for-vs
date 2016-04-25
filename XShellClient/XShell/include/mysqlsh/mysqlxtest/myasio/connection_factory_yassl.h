/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_ASIO_CONNECTION_YASSL_FACTORY_H_
#define _NGS_ASIO_CONNECTION_YASSL_FACTORY_H_

#if defined(HAVE_YASSL)

#include "openssl/ssl.h"

#include "myasio/connection_factory.h"
#include "myasio/wrapper_ssl.h"
#include "myasio/connection_state_yassl.h"

namespace yaSSL
{
  class SSL_CTX;
}

namespace ngs
{

  class Connection_yassl_factory: public Connection_factory
  {
  public:
    Connection_yassl_factory(const std::string &ssl_key,
                             const std::string &ssl_cert,    const std::string &ssl_ca,
                             const std::string &ssl_ca_path, const std::string &ssl_cipher,
                             const std::string &ssl_crl,     const std::string &ssl_crl_path,
                             const bool is_client = false);

    virtual IConnection_unique_ptr create_connection(boost::asio::io_service &io_service);
    virtual IOptions_context_ptr   create_ssl_context_options();

  private:

    bool m_is_client;
    boost::shared_ptr<yaSSL::SSL_CTX> ssl_ctxt;
  };

} // namespace ngs

#endif // defined(HAVE_YASSL)

#endif // _NGS_ASIO_CONNECTION_YASSL_FACTORY_H_
