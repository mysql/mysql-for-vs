using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using Microsoft.Win32;
using System.Reflection;


namespace MySql.Debugger.VisualStudio
{
  [RunInstaller(true)]
  public partial class Installer : System.Configuration.Install.Installer
  {
    private string enginePath = @"SOFTWARE\Microsoft\VisualStudio\10.0\AD7Metrics\Engine\";
    private string clsidPath = @"SOFTWARE\Microsoft\VisualStudio\10.0\CLSID\";

    public Installer()
    {
      InitializeComponent();
    }

    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);
      RegisterDebugEngine();
    }

    public override void Uninstall(IDictionary savedState)
    {
      base.Uninstall(savedState);
      UnregisterDebugEngine();
    }

    protected void RegisterDebugEngine()
    {
      RegistryKey engineKey = Registry.LocalMachine.CreateSubKey(enginePath + AD7Guids.EngineGuid.ToString("B").ToUpper());
      engineKey.SetValue(null, "guidMySqlStoredProcedureDebugEngine");
      engineKey.SetValue("CLSID", AD7Guids.CLSIDGuid.ToString("B").ToUpper());
      engineKey.SetValue("ProgramProvider", AD7Guids.ProgramProviderGuid.ToString("B").ToUpper());
      engineKey.SetValue("Attach", 1, RegistryValueKind.DWord);
      engineKey.SetValue("AddressBP", 0, RegistryValueKind.DWord);
      engineKey.SetValue("AutoSelectPriority", 4, RegistryValueKind.DWord);
      engineKey.SetValue("CallstackBP", 1, RegistryValueKind.DWord);
      engineKey.SetValue("Name", AD7Guids.EngineName);
      engineKey.SetValue("PortSupplier", AD7Guids.PortSupplierGuid.ToString("B").ToUpper());
      engineKey.SetValue("AlwaysLoadLocal", 0, RegistryValueKind.DWord);

      RegistryKey clsidKey = Registry.LocalMachine.CreateSubKey(clsidPath + AD7Guids.CLSIDGuid.ToString("B").ToUpper());
      clsidKey.SetValue("Assembly", Assembly.GetExecutingAssembly().GetName().Name);
      clsidKey.SetValue("Class", typeof(AD7Engine).FullName);
      clsidKey.SetValue("InprocServer32", @"c:\windows\system32\mscoree.dll");
      clsidKey.SetValue("CodeBase", @"file:///" + Assembly.GetExecutingAssembly().Location);

      RegistryKey programProviderKey = Registry.LocalMachine.CreateSubKey(clsidPath + AD7Guids.ProgramProviderGuid.ToString("B").ToUpper());
      programProviderKey.SetValue("Assembly", Assembly.GetExecutingAssembly().GetName().Name);
      programProviderKey.SetValue("Class", typeof(AD7ProgramProvider).FullName);
      programProviderKey.SetValue("InprocServer32", @"c:\windows\system32\mscoree.dll");
      programProviderKey.SetValue("CodeBase", @"file:///" + Assembly.GetExecutingAssembly().Location);
    }

    protected void UnregisterDebugEngine()
    {
      Registry.LocalMachine.DeleteSubKeyTree(enginePath + AD7Guids.EngineGuid.ToString("B").ToUpper());
      Registry.LocalMachine.DeleteSubKeyTree(clsidPath + AD7Guids.CLSIDGuid.ToString("B").ToUpper());
      Registry.LocalMachine.DeleteSubKeyTree(clsidPath + AD7Guids.ProgramProviderGuid.ToString("B").ToUpper());
    }

  }
}
