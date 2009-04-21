// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework
{
    class TestFixtureAttribute : Attribute
    {
    }

    class TearDownAttribute : Attribute
    {
    }

    class TestFixtureSetUpAttribute : Attribute
    {
    }

    class TestFixtureTearDownAttribute : Attribute
    {
    }

    class SetUpAttribute : Attribute
    {
    }

    class TestAttribute : Attribute
    {
    }

    class Assert
    {
        public static void Fail(string message)
        {
            throw new Exception(message);
        }

        public static void AreEqual(object expected, object value, string msg)
        {
            try
            {
                if (expected is UInt64)
                {
                    ulong lValue = Convert.ToUInt64(value);
                    if (!expected.Equals(lValue))
                        throw new Exception(msg);
                }
                else
                {
                    long iExpected = Convert.ToInt64(expected);
                    long iValue = Convert.ToInt64(value);
                    if (iExpected != iValue)
                        throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidCastException ||
                    ex is FormatException)
                {
                    if (!expected.Equals(value))
                        throw new Exception(msg);
                }
                else
                    throw new Exception(msg);
            }

            /*            if (expected is string)
                        {
                            if (expected.ToString() != value.ToString())
                                throw new Exception(msg);
                        }
                        else if (expected is DateTime)
                        {
                            if (!expected.Equals(value))
                                throw new Exception(msg);
                        }
                        else*/
        }

        public static void AreEqual(object expected, object value)
        {
            AreEqual(expected, value, null);
        }

        public static void IsTrue(bool value, string msg)
        {
            if (!value)
                throw new Exception(msg);
        }

        public static void IsTrue(bool value)
        {
            IsTrue(value, "Value should be true");
        }

        public static void IsFalse(bool value, string msg)
        {
            if (value)
                throw new Exception(msg);
        }

        public static void IsFalse(bool value)
        {
            IsFalse(value, "Value should be false");
        }

        public static void IsNull(object value, string msg)
        {
            if (value != null)
                throw new Exception(msg);
        }

        public static void IsNull(object value)
        {
            IsNull(value, "Should be null");
        }

        public static void IsNotNull(object value, string msg)
        {
            if (value == null)
                throw new Exception(msg);
        }

        public static void IsNotNull(object value)
        {
            IsNotNull(value, "Should not be null");
        }

    }
}
