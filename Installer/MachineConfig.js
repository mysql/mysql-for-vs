// Launched during installation
function InstallMachineConfig()
{
    DoAllJob("Install");
}

// Launched during deinstalation
function UninstallMachineConfig()
{
    DoAllJob("Uninstall");
}

// Does all job
function DoAllJob(action)
{
    // Init log
    var log = InitLog();
    WriteLineToLog(log, "Initializing installation.");
    WriteLineToLog(log, "Action is " + action);
    

    // Get framework root           
    var frameworkRoot = GetFrameworkRoot(log);
    if( frameworkRoot == null )
    {
        WriteLineToLog("Failed to get framework root!");
        return;
    }    
    WriteLineToLog(log, "Framework root: " + frameworkRoot);

    // Get 2.0 path
    var machineConfig = GetMachineConfigPath(log, frameworkRoot);
    if( machineConfig == null )
    {
        WriteLineToLog("Failed to get machine.config path!");
        return;
    }    
    WriteLineToLog(log, "machine.config path: " + machineConfig);
    
    // Perform main task
    MainOperation(log, machineConfig, action);
    
    // Close log
    CloseLog(log);
}

// Returns machine.config path
function GetMachineConfigPath(log, root)
{
    try
    {
        // Create fso
        var fso, f, fc, s;
        fso = new ActiveXObject("Scripting.FileSystemObject");
        if(fso == null)
        {
            WriteLineToLog(log, "Failed to get FSO!");
            return null;
        }
        
        // Get root information
        f = fso.GetFolder(root);
        if(fso == null)
        {
            WriteLineToLog(log, "Failed to get information about framework root!");
            return null;
        }
        
        // Enumerate subfolders
        fc = new Enumerator(f.SubFolders);
        if(fc == null)
        {
            WriteLineToLog(log, "Failed to enumerate framework root subfolders!");
            return null;
        }
        
        // Search for v2.0.... subfolder
        for (; !fc.atEnd(); fc.moveNext())
        {
            if( fc.item().Name.indexOf("v2.0.") == 0)
            {   
                // Extract config name and check for existence
                var result = root + fc.item().Name + "\\CONFIG\\machine.config";
                if( fso.FileExists(result) )
                    return result; 
                else
                    WriteLineToLog(log, "File " + result + " doesn't exists.");                
            }
        }
        
        WriteLineToLog(log, "v2.0 subfolder not founded!");
        return null;
    }
    catch(e)
    {
        WriteLineToLog(log, "Failed to get framework root because of exception!");
        return null;
    }
}

// Returns .NET frameworks root directory
function GetFrameworkRoot(log)
{
    try
    {
        var WshShell = new ActiveXObject("WScript.Shell");
        if( WshShell == null )
        {
            WriteLineToLog(log, "Failed to create WScript.Shell!");
            return null;
        }
        WriteLineToLog("reading framework root from registry");
        return WshShell.RegRead ("HKLM\\Software\\Microsoft\\.NETFramework\\InstallRoot");        
    }
    catch(e)
    {
        WriteLineToLog(log, "Failed to get framework root because of exception!");
        return null;
    }
}

// Initializes log file on disk C
function InitLog()
{
    try
    {
        var fso = new ActiveXObject("Scripting.FileSystemObject");
        return fso.CreateTextFile("c:\\MySql.Data.log", true);
    }
    catch(e)
    {
        return null;
    }
}

// Writes text to log
function WriteToLog(log, line)
{
    try
    {
        if( log != null )
            log.Write(line);
    }
    catch(e)
    {
    }
}

// Writes line to log
function WriteLineToLog(log, line)
{
    try
    {
        if( log != null )
            log.WriteLine(line);
    }
    catch(e)
    {
    }
}

// Closes log file on disk C
function CloseLog(log)
{
    try
    {
        if( log != null )
            log.Close();
    }
    catch(e)
    {
        return null;
    }
}

// Performs main operation - alters machine.config
function MainOperation(log, machineConfigPath, action)
{
    var invariantName = "MySql.Data.MySqlClient";
    
    try
    {
        // Create DOM object
        var config = new ActiveXObject("Msxml2.DOMDocument");
        if( config == null )
        {
            WriteLineToLog(log, "Failed to create XML document!");
            return;
        }
        WriteLineToLog(log, "XML document created.");

        // Load machine config
        config.load(machineConfigPath);
        WriteLineToLog(log, "XML document loaded.");

        // Locate list of factpries
        var factoriesList = config.getElementsByTagName("DbProviderFactories");
        if( factoriesList == null || factoriesList.length <= 0 && factoriesList[0] == null )
        {
            WriteLineToLog(log, "Factories list is empty!");
            return;
        }
        WriteLineToLog(log, "Factories read.");

        // Modify list if it is founded
        var existEntry = GetExistsEntry(factoriesList[0], invariantName);
        WriteLineToLog(log, "Search for existed entry completed.");
        
        // Uninstall if installed
        if( action == "Uninstall" && existEntry != null )
        {
            WriteLineToLog(log, "Removing exists entry...");
            factoriesList[0].removeChild(existEntry);
            WriteLineToLog(log, "Existing entry removed.");
        }
        
        // Install if not installed
        if( action == "Install" && existEntry == null )
        {
            WriteLineToLog(log, "Creating new entry...");
            var newEntry = config.createElement("add");
            if(newEntry == null)
            {
                WriteLineToLog(log, "Factories to create new entry!");
                return;
            }
            WriteLineToLog(log, "New entry created.");
            
            WriteLineToLog(log, "Filling attributes...");
            newEntry.setAttribute("name", "MySQL Data Provider");
            newEntry.setAttribute("invariant", invariantName);
            newEntry.setAttribute("description", ".Net Framework Data Provider for MySQL");
            newEntry.setAttribute("type", "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data");
            WriteLineToLog(log, "Attributes are filled.");
            
            WriteLineToLog(log, "Appending entry...");
            factoriesList[0].appendChild(newEntry);
            WriteLineToLog(log, "New entry appended.");
        }
        
        // Save changes    
        WriteLineToLog(log, "Saving changes...");
        config.save(machineConfigPath);
        WriteLineToLog(log, "machine.config succesfully saved.");

    }
    catch(e)
    {
        WriteLineToLog(log, "Failed to perform main task because of exception!");
    }
}

// Searches for existing provider entry
function GetExistsEntry(factoriesList, invariantName)
{
    for(i = 0; i < factoriesList.childNodes.length; i++ )
    {
        if( factoriesList.childNodes[i].getAttribute("invariant") == invariantName )
            return factoriesList.childNodes[i];
    }
    return null;
}