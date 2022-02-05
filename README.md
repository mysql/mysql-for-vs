# MySQL for Visual Studio
MySQL for Visual Studio provides access to MySQL objects and data without forcing your developers to leave Visual Studio.

Designed and developed as a Visual Studio package, MySQL for Visual Studio integrates directly into Server Explorer providing a seamless experience for setting up new connections and working with database objects.

## Licensing
Please refer to files [README](README) and [LICENSE](LICENSE), available in this repository, and [Legal Notices in documentation](https://dev.mysql.com/doc/visual-studio/en/visual-studio-preface.html) for further details.

### Software requirements for debugging and building
- Visual Studio 2017 and 2019 (any flavor) with workloads:
  - ASP .NET and web development
  - .NET desktop development
  - Visual Studio extension development
- MySQL Connector/NET 8.0.24
- MySQL for Visual Studio 1.2.10
- Sandcastle Help File Builder (2019.11.17.0+)
- Wix Toolset 3.11.2
- Wix Toolset Visual Studio 2017 and 2019 Extensions
- MSBuild Community Tasks 1.5.0.214
- Nuget.exe (if not already included as part of your VS installation)

### Prerequisites

This project is configured to work with Visual Studio Community 2017 or 2019. If you are expecting to use **Professional** or **Enterprise** it is expected that the following files are updated accordingly. Any **VisualStudioFlavor** property in the following **.csproj** files or mention of **Community** should be replaced with the flavor of your choice:

- Source/MySql.Debugger/MySql.Debugger.csproj
- Source/MySql.Debugger.VisualStudio/MySql.Debugger.VisualStudio.csproj
- Source/MySql.Parser/MySql.Parser.csproj
- Source/MySql.VisualStudio/MySql.VisualStudio.csproj
- Tests/MySql.Debugger.Tests/MySql.Debugger.Tests.csproj
- Tests/MySql.Parser.Tests/MySql.Parser.Tests.csproj
- Tests/MySql.VisualStudio.Tests/MySql.VisualStudio.Tests.csproj
- Source/Mysql.VisualStudio/ImportProjects4.6.1.targets
- Source/Mysql.VisualStudio/ImportProjects4.7.2.targets
- Installer/Installer.wixproj
- Package.csproj

### Debug steps

1. Open MySqlForVisualStudio.sln file using the Visual Studio version to debug.
2. Set **MySql.VisualStudio** as the startup project in the **Solution Explorer**.
3. Right click the **MySql.VisualStudio** project and select the **Properties** option.
4. Ensure that the path to **devenv.exe** is pointing to the correct Visual Studio version in **Debug->Start action->Start external program**. E.g "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe"
5. Ensure that the value at **Debug->Start options->Command line arguments** is: **"/rootSuffix exp"**. This enables using Visual Studio's experimental instsance for debugging.
6. Compile the solution.
7. Debug.

### Package generation

1. Open a cmd window with admin privilges.
2. Navigate to the MySQL for Visual Studio repository.
3. Execute a nuget restore.
    ```sh
    nuget.exe restore "MySqlForVisualStudio.sln"
    ```
4. Use **msbuild** (included in your Visual Studio installation) to generate the packages.
    ```sh
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe" /p:Configuration=Release;NoPackage=n;VisualStudioVersion=16.0 /t:Clean,Build Package.sln
    ```
5. Generated packages, including the MySQL for Visual Studio MSI are located in the **packages** folder.

### MySql.Utility dependency
Some of the projects have a reference to the **MySql.Utility.dll** file.
If changes are required to this assembly, the code is located in the **MySql.Utility** folder of the repository.

### Additional Resources

* [MySQL](http://www.mysql.com/)
* [Documentation](https://dev.mysql.com/doc/visual-studio/en/)
* [MySQL Bugs database](https://bugs.mysql.com)
