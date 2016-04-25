/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

exports.mysql = {}

// Connection functions
exports.mysql.getClassicSession = function(connection_data, password)
{
  var session;

  if (typeof(password) == 'undefined')
    session = _F.mysql.ClassicSession(connection_data);
  else
    session = _F.mysql.ClassicSession(connection_data, password);
  
  return session;
}