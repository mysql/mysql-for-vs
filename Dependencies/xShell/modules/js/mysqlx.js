/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

exports.mysqlx = {}

// Connection functions
exports.mysqlx.getSession = function(connection_data, password)
{
  var session;

  if (typeof(password) == 'undefined')
    session = _F.mysqlx.XSession(connection_data);
  else
    session = _F.mysqlx.XSession(connection_data, password);
  
  return session;
}

exports.mysqlx.getNodeSession = function(connection_data, password)
{
  var session;
  
  if (typeof(password) == 'undefined')
    session = _F.mysqlx.NodeSession(connection_data);
  else
    session = _F.mysqlx.NodeSession(connection_data, password);
  
  return session;
}


exports.mysqlx.expr = function(expression)
{
	if (typeof(expression) == 'undefined')
		expr_obj = _F.mysqlx.Expression();
	else
		expr_obj = _F.mysqlx.Expression(expression);
	
  return expr_obj
}

exports.mysqlx.dateValue = function(year, month, day, hour, minute, second)
{
	if (typeof(year) == 'undefined')
		date_obj = _F.mysqlx.Date();
	else if (typeof(month) == 'undefined')
		date_obj = _F.mysqlx.Date(year);
	else if (typeof(day) == 'undefined')
		date_obj = _F.mysqlx.Date(year, month);
	else if (typeof(hour) == 'undefined')
		date_obj = _F.mysqlx.Date(year, month, day);
	else if (typeof(minute) == 'undefined')
		date_obj = _F.mysqlx.Date(year, month, day, hour);
	else if (typeof(second) == 'undefined')
		date_obj = _F.mysqlx.Date(year, month, day, hour, minute);
	else 
		date_obj = _F.mysqlx.Date(year, month, day, hour, minute, second);
	
  return date_obj
}

// Constants
exports.mysqlx.Type = _F.mysqlx.Type();
exports.mysqlx.IndexType = _F.mysqlx.IndexType();
