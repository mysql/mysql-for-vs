// Copyright (c) 2012, 2018, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MySql.Utility.Classes
{
  public class IniFile
  {
    public const uint MAX_BUFFER = 32767;

    public IniFile(string fileName)
    {
      FileName = fileName;
    }

    public string FileName { get; set; }

    public string ReadValue(string section, string key, string defaultValue)
    {
      StringBuilder value = new StringBuilder(255);
      GetPrivateProfileString(section, key, defaultValue, value, 255, FileName);
      return value.ToString();
    }

    public void WriteValue(string section, string key, string value)
    {
      WritePrivateProfileString(section, key, value, FileName);
    }

    public bool HasKey(string section, string keyToFind)
    {
      string[] keys = GetKeyNames(section);
      return keys != null && keys.Any(key => string.Compare(key, keyToFind, StringComparison.OrdinalIgnoreCase) == 0);
    }

    public string[] GetKeyNames(string section)
    {
      IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
      int bytes = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, FileName);
      if ((bytes == MAX_BUFFER - 2) || (bytes == 0))
      {
        Marshal.FreeCoTaskMem(pReturnedString);
        return null;
      }

      string returnedString = Marshal.PtrToStringAnsi(pReturnedString, bytes - 1);
      string[] sections = returnedString.Split('\0');
      Marshal.FreeCoTaskMem(pReturnedString);
      return sections;
    }

    [DllImport(DllImportConstants.KERNEL32)]
    private static extern int GetPrivateProfileSection(string section, IntPtr lpReturnedString, uint nSize, string fileName);

    [DllImport(DllImportConstants.KERNEL32)]
    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

    [DllImport(DllImportConstants.KERNEL32)]
    private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);
  }
}
