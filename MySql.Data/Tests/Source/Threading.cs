// Copyright (C) 2004-2007 MySQL AB
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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
		private void MultipleThreadsWorker(object ev)
		{
			(ev as ManualResetEvent).WaitOne();

            using (MySqlConnection c = new MySqlConnection(GetConnectionString(true)))
            {
                c.Open();
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

			for (int i = 0; i < 20; i++)
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
		}
	}

}
