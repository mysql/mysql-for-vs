/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#include "mysqlx/mysql.h"

#include ""
#include "mysqld_error.h"

#include <boost/asio.hpp>
#include <google/protobuf/message.h>

class Mysqlx_test_connector;

struct st_mysql_priv
{
  Mysqlx_test_connector *impl;
};

typedef st_mysql_priv MYSQL;


typedef struct st_mysql
{
  struct st_mysql_priv *impl;
  int last_error_no;
  char *last_error;

  my_bool free_me;		/* If free in mysql_close */
#if 0
  NET		net;			/* Communication parameters */
  unsigned char	*connector_fd;		/* ConnectorFd for SSL */
  char		*host,*user,*passwd,*unix_socket,*server_version,*host_info;
  char          *info, *db;
  struct charset_info_st *charset;
  MYSQL_FIELD	*fields;
  MEM_ROOT	field_alloc;
  my_ulonglong affected_rows;
  my_ulonglong insert_id;		/* id if insert on table with NEXTNR */
  my_ulonglong extra_info;		/* Not used */
  unsigned long thread_id;		/* Id for connection in server */
  unsigned long packet_length;
  unsigned int	port;
  unsigned long client_flag,server_capabilities;
  unsigned int	protocol_version;
  unsigned int	field_count;
  unsigned int 	server_status;
  unsigned int  server_language;
  unsigned int	warning_count;
  struct st_mysql_options options;
  enum mysql_status status;

  my_bool	reconnect;		/* set to 1 if automatic reconnect */


  LIST  *stmts;                     /* list of all statements */
  const struct st_mysql_methods *methods;
  void *thd;
  /*
   Points to boolean flag in MYSQL_RES  or MYSQL_STMT. We set this flag
   from mysql_stmt_close if close had to cancel result set of this object.
   */
  my_bool *unbuffered_fetch_owner;
  /* needed for embedded server - no net buffer to store the 'info' */
  char *info_buffer;
  void *extension;
#endif
} MYSQL;




