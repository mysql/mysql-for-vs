using NUnit.Framework;
using System;
using MySql.Web.SessionState;
using MySql.Data.MySqlClient;
using System.Web.SessionState;
using System.Threading;
using System.Collections.Specialized;
using System.IO;

namespace MySql.Web.Tests
{
  [TestFixture]
  class SessionTests : BaseWebTest
  {           
    string strSessionID;
    string calledId;

    [SetUp]
    public override void Setup()
    {
      base.Setup();          
    }

    private byte[] Serialize(SessionStateItemCollection items)
    {
      MemoryStream ms = new MemoryStream();
      BinaryWriter writer = new BinaryWriter(ms);
      if (items != null)
      {
        items.Serialize(writer);
      }
      writer.Close();
      return ms.ToArray();
    }


    private void CreateSessionData(int AppId, DateTime timeCreated)
    {
      MySqlCommand cmd = new MySqlCommand();        
      strSessionID = System.Guid.NewGuid().ToString();
     
      //DateTime now = DateTime.Now;
      //DateTime lastHour = now.Subtract(new TimeSpan(1, 0, 0));
           
      SessionStateItemCollection collection = new SessionStateItemCollection();
      collection["FirstName"] = "Some";
      collection["LastName"] = "Name";
      byte[] items = Serialize(collection);

      string sql = @"INSERT INTO my_aspnet_sessions VALUES (
            @sessionId, @appId, @created, @expires, @lockdate, @lockid, @timeout,
            @locked, @items, @flags)";
 
      cmd = new MySqlCommand(sql, conn);
      cmd.Parameters.AddWithValue("@sessionId", strSessionID);
      cmd.Parameters.AddWithValue("@appId", AppId);
      cmd.Parameters.AddWithValue("@created", timeCreated);
      cmd.Parameters.AddWithValue("@expires", timeCreated);
      cmd.Parameters.AddWithValue("@lockdate", timeCreated);
      cmd.Parameters.AddWithValue("@lockid", 1);
      cmd.Parameters.AddWithValue("@timeout", 1);
      cmd.Parameters.AddWithValue("@locked", 0);
      cmd.Parameters.AddWithValue("@items", items);
      cmd.Parameters.AddWithValue("@flags", 0);
      cmd.ExecuteNonQuery();

      //create new row on sessioncleanup table
      cmd.CommandText = "INSERT IGNORE INTO my_aspnet_sessioncleanup SET" +
              " ApplicationId = @ApplicationId, " +
              " LastRun = NOW(), " +
              " IntervalMinutes = 10";
      cmd.Parameters.Clear();
      cmd.Parameters.AddWithValue("@ApplicationId", AppId);
      cmd.ExecuteNonQuery();

      // set our last run table to 1 hour ago
      cmd.CommandText = "UPDATE my_aspnet_sessioncleanup SET LastRun=@lastHour WHERE ApplicationId = @ApplicationId";
      cmd.Parameters.Clear();
      cmd.Parameters.AddWithValue("@lastHour", timeCreated);
      cmd.Parameters.AddWithValue("@ApplicationId", AppId);
      cmd.ExecuteNonQuery();
    }


    private void SetSessionItemExpiredCallback(bool includeCallback)
    {
      calledId = null;
            
      CreateSessionData(1, DateTime.Now.Subtract(new TimeSpan(1, 0, 0)));

      MySqlSessionStateStore session = new MySqlSessionStateStore();

      NameValueCollection config = new NameValueCollection();
      config.Add("connectionStringName", "LocalMySqlServer");
      config.Add("applicationName", "/");
      config.Add("enableExpireCallback", includeCallback ? "true" : "false");      
      session.Initialize("SessionProvTest", config);
      if (includeCallback) session.SetItemExpireCallback(expireCallback);
      Thread.Sleep(1000);
      session.Dispose();
    }

    private long CountSessions()
    {
      return (long)MySqlHelper.ExecuteScalar(conn, "SELECT COUNT(*) FROM my_aspnet_sessions");
    }

    public void expireCallback(string id, SessionStateStoreData item)
    {
      calledId = id;
    }


    [Test]
    public void SessionItemWithExpireCallback()
    {
      SetSessionItemExpiredCallback(true);
      Assert.AreEqual(strSessionID, calledId);
      Assert.AreEqual(0, CountSessions());
    }


    [Test]
    public void SessionItemWithoutExpireCallback()
    {
      SetSessionItemExpiredCallback(false);
      Assert.AreNotEqual(strSessionID, calledId);
      Assert.AreEqual(0, CountSessions());
    }

    [Test]
    public void DeleteSessionAppSpecific()
    {
      // create two sessions of different appId
      // it should delete only 1
      CreateSessionData(1, DateTime.Now.Subtract(new TimeSpan(1, 0, 0)));      
      CreateSessionData(2, DateTime.Now.Subtract(new TimeSpan(1, 0, 0)));      

      MySqlSessionStateStore session = new MySqlSessionStateStore();

      NameValueCollection config = new NameValueCollection();
      config.Add("connectionStringName", "LocalMySqlServer");
      config.Add("applicationName", "/");
      config.Add("enableExpireCallback", "false" );
      session.Initialize("SessionTests", config);
      Thread.Sleep(1000);
      session.Dispose();

      Assert.AreEqual(1, (long)MySqlHelper.ExecuteScalar(conn, "SELECT Count(*) FROM my_aspnet_sessions;"));

    }
  }
}
