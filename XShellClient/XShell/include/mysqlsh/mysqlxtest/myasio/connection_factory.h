/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_ASIO_CONNECTION_FACTORY_H_
#define _NGS_ASIO_CONNECTION_FACTORY_H_

#include "myasio/connection.h"


namespace ngs
{

class Connection_factory
{
public:
  virtual ~Connection_factory() {}

  virtual IConnection_unique_ptr create_connection(boost::asio::io_service &io_service) = 0;
  virtual IOptions_context_ptr   create_ssl_context_options() = 0;
};

typedef boost::shared_ptr<Connection_factory> Connection_factory_ptr;

} // namespace ngs

#endif // _NGS_ASIO_CONNECTION_FACTORY_H_
