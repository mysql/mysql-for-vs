using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger.VisualStudio
{
  public class AD7Guids
  {
    public const string EngineString = "EEEE0740-10F7-4e5f-8BC4-1CC0AC9ED5B0";
    public static readonly Guid EngineGuid = new Guid(EngineString);

    public const string CLSIDString = "EEEE066A-1103-451f-BC7A-6AEF76558AE2";
    public static readonly Guid CLSIDGuid = new Guid(CLSIDString);

    public const string ProgramProviderString = "EEEE9AB0-511C-4bf0-BBE8-F763A73DA5EF";
    public static readonly Guid ProgramProviderGuid = new Guid(ProgramProviderString);

    public const string PortSupplierString = "EEEE547D-6B37-4F46-9567-F4AC7ACAFCBE";
    public static readonly Guid PortSupplierGuid = new Guid(ProgramProviderString);

    public const string EngineName = "MySql Stored Procedure Debug Engine";

    public const string LanguageName = "MySql language";
  }
}
