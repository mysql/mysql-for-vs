/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_ASIO_OPTIONS_H_
#define _NGS_ASIO_OPTIONS_H_

#include <string>
#include <vector>
#include <boost/make_shared.hpp>


namespace ngs
{

  class IOptions_session
  {
  public:
    virtual ~IOptions_session() {};

    virtual bool supports_tls() = 0;
    virtual bool active_tls() = 0;

    virtual std::string ssl_cipher() = 0;
    virtual std::vector<std::string> ssl_cipher_list() = 0;

    virtual std::string ssl_version() = 0;

    virtual long ssl_verify_depth() = 0;
    virtual long ssl_verify_mode() = 0;
    virtual long ssl_sessions_reused() = 0;

    virtual long ssl_get_verify_result_and_cert() = 0;
    virtual std::string ssl_get_peer_certificate_issuer() = 0;
    virtual std::string ssl_get_peer_certificate_subject() = 0;
  };

  class IOptions_context
  {
  public:
    virtual ~IOptions_context() {};

    virtual long ssl_ctx_verify_depth() = 0;
    virtual long ssl_ctx_verify_mode() = 0;

    virtual std::string ssl_server_not_after() = 0;
    virtual std::string ssl_server_not_before() = 0;

    virtual long ssl_sess_accept_good() = 0;
    virtual long ssl_sess_accept() = 0;
    virtual long ssl_accept_renegotiates() = 0;

    virtual std::string ssl_session_cache_mode() = 0;

    virtual long ssl_session_cache_hits() = 0;
    virtual long ssl_session_cache_misses() = 0;
    virtual long ssl_session_cache_overflows() = 0;
    virtual long ssl_session_cache_size() = 0;
    virtual long ssl_session_cache_timeouts() = 0;
    virtual long ssl_used_session_cache_entries() = 0;
  };

  typedef boost::shared_ptr<IOptions_session> IOptions_session_ptr;
  typedef boost::shared_ptr<IOptions_context> IOptions_context_ptr;

} // namespace ngs

#endif // _NGS_ASIO_OPTIONS_H_
