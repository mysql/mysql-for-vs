/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_ASIO_OPTIONS_SSL_H_
#define _NGS_ASIO_OPTIONS_SSL_H_

#include "myasio/options.h"


namespace ngs
{

  class Options_session_ssl : public IOptions_session
  {
  public:
    Options_session_ssl(SSL *ssl)
    :m_ssl(ssl)
    {
    }

    bool supports_tls() { return true; };
    bool active_tls() { return true; };

    std::string ssl_cipher();
    std::string ssl_version();
    std::vector<std::string> ssl_cipher_list();

    long ssl_verify_depth();
    long ssl_verify_mode();

    long ssl_sessions_reused();
    long ssl_get_verify_result_and_cert();

    std::string ssl_get_peer_certificate_issuer();

    std::string ssl_get_peer_certificate_subject();

  private:
    SSL     *m_ssl;
  };

  class Options_context_ssl : public IOptions_context
  {
  public:
    Options_context_ssl(SSL_CTX *ctx)
    :m_ctx(ctx)
    {
    }

    long ssl_ctx_verify_depth();
    long ssl_ctx_verify_mode();

    std::string ssl_server_not_after();
    std::string ssl_server_not_before();

    long ssl_sess_accept_good();
    long ssl_sess_accept();
    long ssl_accept_renegotiates();

    std::string ssl_session_cache_mode();

    long ssl_session_cache_hits();
    long ssl_session_cache_misses();
    long ssl_session_cache_overflows();
    long ssl_session_cache_size();
    long ssl_session_cache_timeouts();
    long ssl_used_session_cache_entries();
  private:
    SSL_CTX *m_ctx;
  };

} // namespace ngs

#endif // _NGS_ASIO_OPTIONS_SSL_H_
