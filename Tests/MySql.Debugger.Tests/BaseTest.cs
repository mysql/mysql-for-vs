using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MySql.Data.MySqlClient;

namespace MySql.Debugger.Tests
{
  public class BaseTest
  {
    [SetUp]
    public void Setup()
    {
      MySqlConnection con = new MySqlConnection(TestUtils.CONNECTION_STRING_WITHOUT_DB);
      MySqlCommand cmd = new MySqlCommand("drop database if exists test4", con);
      con.Open();
      try
      {
        cmd.ExecuteNonQuery();
        cmd.CommandText = "create database test4;";
        cmd.ExecuteNonQuery();
      }
      finally
      {
        con.Close();
      }
    }

    [TearDown]
    public void Teardown()
    {
      MySqlConnection con = new MySqlConnection(TestUtils.CONNECTION_STRING_WITHOUT_DB);
      MySqlCommand cmd = new MySqlCommand("drop database if exists test4", con);
      con.Open();
      try
      {
        cmd.ExecuteNonQuery();
      }
      finally
      {
        con.Close();
      }
    }
  }
}
