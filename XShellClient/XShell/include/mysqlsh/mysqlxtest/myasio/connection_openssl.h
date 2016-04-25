/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_ASIO_CONNECTION_OPENSSL_H_
#define _NGS_ASIO_CONNECTION_OPENSSL_H_

#if !defined(HAVE_YASSL)

#include <boost/asio/ssl.hpp>

#include "myasio/connection.h"


namespace ngs
{

class Connection_openssl : public IConnection
{
public:
  Connection_openssl(boost::asio::io_service &socket, boost::asio::ssl::context &context, const bool is_client = false);
  virtual ~Connection_openssl();

  virtual Endpoint    get_remote_endpoint() const;
  virtual int         get_socket_id();
  virtual IOptions_session_ptr options();

  virtual void async_connect(const Endpoint &endpoint, const On_asio_status_callback &on_connect_callback, const On_asio_status_callback &on_ready_callback);
  virtual void async_accept(boost::asio::ip::tcp::acceptor &, const On_asio_status_callback &on_accept_callback, const On_asio_status_callback &on_read_callback);
  virtual void async_write(const Const_buffer_sequence &data, const On_asio_data_callback &on_write_callback);
  virtual void async_read(const Mutable_buffer_sequence &data, const On_asio_data_callback &on_read_callback);
  virtual void async_activate_tls(const On_asio_status_callback on_status);

  virtual IConnection_ptr get_lowest_layer();

  virtual void post(const boost::function<void ()> &calee);
  virtual bool thread_in_connection_strand();

  virtual void shutdown(boost::asio::socket_base::shutdown_type how_to_shutdown, boost::system::error_code &ec);
  virtual void cancel();
  virtual void close();

private:
  enum State {State_handshake, State_running, State_stop};

  typedef boost::asio::ssl::stream<boost::asio::ip::tcp::socket> stream;
  typedef boost::asio::ssl::stream_base::handshake_type handshake_type;
  typedef boost::asio::strand strand;

  void on_connect_try_handshake(const boost::system::error_code &ec);
  void on_accept_try_handshake(boost::asio::io_service &acceptor, const boost::system::error_code &ec);
  void on_handshake(const boost::system::error_code &ec);

  handshake_type m_handshake_type;
  stream         m_asio_socket;
  strand         m_asio_strand;

  On_asio_status_callback m_accept_callback;
  On_asio_status_callback m_ready_callback;

  State m_state;
};

}  // namespace ngs


#endif // !defined(HAVE_YASSL)

#endif // _NGS_ASIO_CONNECTION_OPENSSL_H_
