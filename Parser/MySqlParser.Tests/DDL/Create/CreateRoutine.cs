using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;


namespace MySql.Parser.Tests
{
  [TestFixture]
  public class CreateRoutine
  {
    [Test]
    public void SimpleProc()
    {
      string sql = @"
CREATE PROCEDURE simpleproc (OUT param1 INT)
    BEGIN
      SELECT COUNT(*) INTO param1 FROM t;
    END;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void SimpleProc2()
    {
      string sql = @"
CREATE PROCEDURE dorepeat(p1 INT)
BEGIN
  SET @x = 0;
  REPEAT SET @x = @x + 1; UNTIL @x > p1 END REPEAT;
END;
";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void CreateProcWithSec()
    {
      string sql = @"CREATE DEFINER = 'admin'@'localhost' PROCEDURE account_count()
SQL SECURITY INVOKER
BEGIN
  SELECT 'Number of accounts:', COUNT(*) FROM mysql.user;
END;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void ComplexProc()
    {
      string sql = @"
CREATE DEFINER=`root`@`localhost` PROCEDURE `add_error_log`(
`error_level` int(11),
`error_level_name` varchar(512),
`error_message` longtext,
`error_file` text,
`error_line` int(11),
`error_context` longtext,
`error_query_string` longtext,
`error_time` text ,
`user_id` int(11),
`post_data` longtext,
`user_msg` text)
BEGIN
INSERT INTO tbl_error_log(error_level, error_level_name, error_message, error_file,error_line, error_context,error_query_string,error_time, user_id, post_data, user_msg)
values(error_level, error_level_name, error_message, error_file,error_line, error_context,error_query_string, error_time, user_id, post_data, user_msg);
END;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }


    [Test]
    public void SimpleFunc()
    {
      string sql = @"CREATE FUNCTION hello (s CHAR(20))
    RETURNS CHAR(50) DETERMINISTIC
    RETURN CONCAT('Hello, ',s,'!');";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void ComplexFunc()
    {
      string sql = @"CREATE FUNCTION fnGetXMLinfoVraag4 (xmlTag varchar(30),message text) returns varchar(255)
DETERMINISTIC
READS SQL DATA
begin
declare lenField int;
declare xmlTagBegin varchar(30);
declare xmlTagEnd varchar(30);
declare fieldresult varchar(255);
set xmlTagBegin = concat('<', xmlTag, '>');
set xmlTagEnd = concat('</', xmlTag, '>');
set lenField = length(xmlTag) + 2;
set fieldresult = case when locate(xmlTagBegin,message) = 0 then ''
else substring(message,locate(xmlTagBegin,message) + lenField,locate(xmlTagEnd,message) - (locate(xmlTagBegin,message) + lenField)) end;
return fieldresult;
end";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void ComplexFunc2()
    {
      string sql = @"
CREATE FUNCTION db.fnfullname ( id smallint(5) unsigned ) RETURNS varchar(160) CHARACTER SET utf8
COMMENT 'Returns the full name of person in db.people table referenced by id where FirstName and FamilyName are not null but MiddleName may be null'
DETERMINISTIC
READS SQL DATA
BEGIN
DECLARE fulname varchar(160) CHARACTER SET utf8;
SELECT CONCAT_WS(' ', db.people.FirstName, db.people.MiddleName, db.people.FamilyName) into fulname from db.people where db.people.id=id;
RETURN fulname;
END;
";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void SimpleCompare()
    {
      string sql = @"CREATE FUNCTION SimpleCompare(n INT, m INT)
  RETURNS VARCHAR(20)

  BEGIN
    DECLARE s VARCHAR(20);

    IF n > m THEN SET s = '>';
    ELSEIF n = m THEN SET s = '=';
    ELSE SET s = '<';
    END IF;

    SET s = CONCAT(n, ' ', s, ' ', m);

    RETURN s;
  END";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Test]
    public void VerboseCompare()
    {
      string sql = @"CREATE FUNCTION VerboseCompare (n INT, m INT)
  RETURNS VARCHAR(50)

  BEGIN
    DECLARE s VARCHAR(50);

    IF n = m THEN SET s = 'equals';
    ELSE
      IF n > m THEN SET s = 'greater';
      ELSE SET s = 'less';
      END IF;

      SET s = CONCAT('is ', s, ' than');
    END IF;

    SET s = CONCAT(n, ' ', s, ' ', m, '.');

    RETURN s;
  END";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }
  }
}
