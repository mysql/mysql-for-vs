// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Reflection;
using NUnit.Framework;
using System.Collections;

namespace MySql.Data.MySqlClient.Tests
{
    public class TestRunner
    {
        private ArrayList tests = new ArrayList();

        public event EventHandler FixtureStarted;
        public event EventHandler FixtureDone;
        public event EventHandler TestStarted;
        public event EventHandler TestDone;

        public  ArrayList LoadTests()
        {
            Assembly me = Assembly.GetExecutingAssembly();

            Type[] types = me.GetTypes();

            foreach (Type t in types)
            {
                object[] o = t.GetCustomAttributes(typeof(TestFixtureAttribute), false);
                if (o == null || o.Length == 0) continue;

                TestCollection tc = new TestCollection();
                tc.name = t.Name;
                tc.fixtureType = t;

                FindMethods(t, tc);
                if (tc.testMethods.Count > 0)
                    tests.Add(tc);
            }
            return tests;
        }

        private void FindMethods(Type t, TestCollection tc)
        {
            // now get all the methods on the fixture
            MethodInfo[] methods = t.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);

            // if there is a fixture setup routine, invoke it
            foreach (MethodInfo mi in methods)
            {
                if (mi.IsPrivate) continue;

                object[] attr = mi.GetCustomAttributes(true);

                foreach (Attribute a in attr)
                {
                    if (a is TestFixtureSetUpAttribute)
                        tc.classSetup = mi;
                    else if (a is TestFixtureTearDownAttribute)
                        tc.classTeardown = mi;
                    else if (a is SetUpAttribute)
                        tc.setup = mi;
                    else if (a is TearDownAttribute)
                        tc.tearDown = mi;
                    else if (a is TestAttribute)
                    {
                        TestMethod tm = new TestMethod();
                        tm.member = mi;
                        tc.testMethods.Add(tm);
                    }
                }
            }
        }

        public void StartFixture(TestCollection tc)
        {
            try
            {
                if (tc.fixture == null)
                    tc.fixture = Activator.CreateInstance(tc.fixtureType);

                if (tc.classSetup != null)
                    tc.classSetup.Invoke(tc.fixture, null);
            }
            catch (Exception ex)
            {
                tc.message = ex.Message;
                tc.stack = ex.StackTrace;
                throw;
            }
        }

        public void EndFixture(TestCollection tc)
        {
            try
            {
                if (tc.classTeardown != null)
                    tc.classTeardown.Invoke(tc.fixture, null);
            }
            catch (Exception ex)
            {
                tc.message = ex.Message;
                tc.stack = ex.StackTrace;
                throw;
            }
        }

        public bool RunTest(int fixtureIndex, int methodIndex)
        {
            TestCollection tc = (TestCollection)tests[fixtureIndex];
            TestMethod tm = (TestMethod)tc.testMethods[methodIndex];
            try
            {
                if (tc.setup != null)
                    tc.setup.Invoke(tc.fixture, null);

                tm.member.Invoke(tc.fixture, null);

                if (tc.tearDown != null)
                    tc.tearDown.Invoke(tc.fixture, null);

                return true;
            }
            catch (Exception ex)
            {
                tm.message = ex.Message;
                tm.stack = ex.StackTrace;
                return false;
            }
        }

        private void OnTestDone()
        {
            if (TestDone != null)
                TestDone(this, null);
        }

        private void OnTestStarted()
        {
            if (TestStarted != null)
                TestStarted(this, null);
        }

        private void OnFixtureStated()
        {
            if (FixtureStarted != null)
                FixtureStarted(this, null);
        }

        private void OnFixtureDone()
        {
            if (FixtureDone != null)
                FixtureDone(this, null);
        }
    }

    public class TestCollection
    {
        public string name;
        public ArrayList testMethods;
        public MethodInfo classSetup;
        public MethodInfo classTeardown;
        public MethodInfo setup;
        public MethodInfo tearDown;
        public object fixture;
        public Type fixtureType;
        public string message;
        public string stack;

        public TestCollection()
        {
            testMethods = new ArrayList();
        }
    }

    public class TestMethod
    {
        public MethodInfo member;
        public string message;
        public string stack;
    }
}
