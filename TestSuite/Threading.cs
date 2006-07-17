// Copyright (C) 2004-2006 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using MySql.Data.MySqlClient;
using System.Data;
using NUnit.Framework;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace MySql.Data.MySqlClient.Tests
{
    class GenericListener : TraceListener
    {
        System.Collections.Specialized.StringCollection strings;
        StringBuilder partial;

        public GenericListener()
        {
            strings = new System.Collections.Specialized.StringCollection();
            partial = new StringBuilder();
        }

        public int Find(string sToFind)
        {
            int count = 0;
            foreach (string s in strings)
                if (s.IndexOf(sToFind) != -1)
                    count++;
            return count;
        }

        public void Clear()
        {
            partial.Remove(0, partial.Length);
            strings.Clear();
        }

        public override void Write(string message)
        {
            partial.Append(message);
        }

        public override void WriteLine(string message)
        {
            Write(message);
            strings.Add(partial.ToString());
            partial.Remove(0, partial.Length);
        }
    }

	/// <summary>
	/// Summary description for ConnectionTests.
	/// </summary>
	[TestFixture] 
	public class ThreadingTests : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
        }

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
        }

        [SetUp]
        protected override void Setup()
        {
        }

        [TearDown]
        protected override void Teardown()
        {
        }

        private void MultipleThreadsWorker(object ev)
        {
            (ev as ManualResetEvent).WaitOne();

            try
            {
                MySqlConnection c = new MySqlConnection(GetConnectionString(true));
                c.Open();
                c.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Bug #17106 MySql.Data.MySqlClient.CharSetMap.GetEncoding thread synchronization issue
        /// </summary>
        [Test]
        public void MultipleThreads()
        {
            GenericListener myListener = new GenericListener();
            ManualResetEvent ev = new ManualResetEvent(false);
            ArrayList threads = new ArrayList();
            System.Diagnostics.Trace.Listeners.Add(myListener);

            for (int i=0; i < 20; i++)
            {
                ParameterizedThreadStart ts = new ParameterizedThreadStart(MultipleThreadsWorker);
                Thread t = new Thread(ts);
                threads.Add(t);
                t.Start(ev);
            }
            // now let the threads go
            ev.Set();

            // wait for the threads to end
            int x = 0;
            while (x < threads.Count)
            {
                while ((threads[x] as Thread).IsAlive)
                    Thread.Sleep(50);
                x++;
            }
            Assert.AreEqual(1, myListener.Find("Initializing character set mapping array"));
        }
    }

}
