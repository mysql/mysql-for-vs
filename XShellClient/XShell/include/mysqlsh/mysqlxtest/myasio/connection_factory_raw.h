/*
   Copyright (c) 2015, 2016 Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _NGS_ASIO_CONNECTION_RAW_FACTORY_H_
#define _NGS_ASIO_CONNECTION_RAW_FACTORY_H_

#include "myasio/connection_factory.h"


namespace ngs
{

  class Connection_raw_factory: public Connection_factory
  {
  public:
    virtual IConnection_unique_ptr create_connection(boost::asio::io_service &io_service);

    virtual IOptions_context_ptr create_ssl_context_options();
  };

} // namespace ngs

#endif // _NGS_ASIO_CONNECTION_RAW_FACTORY_H_
