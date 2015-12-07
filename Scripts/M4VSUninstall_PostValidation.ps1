function Test-RegistryValue
{
    param(
        [Alias("PSPath")]
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [String]$Path
        ,
        [Parameter(Position = 1, Mandatory = $true)]
        [String]$Name
        ,
        [Switch]$PassThru
    )

    process
	{
        if (Test-Path $Path)
		{
            $Key = Get-Item -LiteralPath $Path
            if ($Key.GetValue($Name, $null) -ne $null)
			{
                if ($PassThru)
				{
                    Get-ItemProperty $Path $Name
                }
				else
				{
                    $true
                }
            }
			else
			{
                $false
            }
        }
		else
		{
            $false
        }
    }
}

function DeleteRegistryAndExtensionsFile($vsVersion, $vsRegistryPath, $vsRegistryName, $extensionsFilePath)
{
	### Delete the created registry entry, if applies
	if (Test-RegistryValue -Path "hkcu:\Console" -Name $("M4VSUninstall_RegKey_" + $vsVersion))
	{
		if ((Test-RegistryValue -Path $vsRegistryPath -Name $vsRegistryName))
		{
			echo $("`n`n -- Deleting the VS registry path for VS" + $vsVersion + ".")
			Remove-ItemProperty -Path $vsRegistryPath -Name $vsRegistryName -Force
		}
		Remove-ItemProperty -Path "hkcu:\Console" -Name $("M4VSUninstall_RegKey_" + $vsVersion) -Force
	}

	### Delete the created extensions file, if applies
	if (Test-RegistryValue -Path "hkcu:\Console" -Name $("M4VSUninstall_ExtFile_" + $vsVersion))
	{
		if ((Test-Path $extensionsFilePath))
		{
			echo $("`n`n -- Deleting the extensions file for VS" + $vsVersion + ".")
			Remove-Item $extensionsFilePath -force
			Remove-ItemProperty -Path "hkcu:\Console" -Name $("M4VSUninstall_ExtFile_" + $vsVersion)
		}
	}
}

function Is64BitsOS()
{
	$version = (Get-WMIObject win32_operatingsystem).OSArchitecture
	if ($version.Contains("64"))
	{
		return $true;
	}
	return $false;
}

###### VS 2012 ######
$vsRegistryPath = if (!(Is64BitsOS)) {"HKLM:\Software\Microsoft\VisualStudio\11.0\Setup\VS"} else {"HKLM:\Software\Wow6432Node\Microsoft\VisualStudio\11.0\Setup\VS"}
$vsRegistryName = "EnvironmentDirectory"
$extensionsFilePath = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\extensions.configurationchanged"} else {"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\extensions.configurationchanged"}

DeleteRegistryAndExtensionsFile -vsVersion "2012" `
	-vsRegistryPath $vsRegistryPath `
	-vsRegistryName $vsRegistryName `
	-extensionsFilePath $extensionsFilePath

###### VS 2013 ######
$vsRegistryPath = if (!(Is64BitsOS)) {"HKLM:\Software\Microsoft\VisualStudio\12.0\"} else {"HKLM:\Software\Wow6432Node\Microsoft\VisualStudio\12.0\"}
$vsRegistryName = "ShellFolder"
$extensionsFilePath = if (!(Is64BitsOS)) {"C:\Program Files \Microsoft Visual Studio 12.0\Common7\IDE\Extensions\extensions.configurationchanged"} else {"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\extensions.configurationchanged"}

DeleteRegistryAndExtensionsFile -vsVersion "2013" `
	-vsRegistryPath $vsRegistryPath `
	-vsRegistryName $vsRegistryName `
	-extensionsFilePath $extensionsFilePath

###### VS 2015 ######
$vsRegistryPath = if (!(Is64BitsOS)) {"HKLM:\Software\Microsoft\VisualStudio\14.0\"} else {"HKLM:\Software\Wow6432Node\Microsoft\VisualStudio\14.0\"}
$vsRegistryName = "ShellFolder"
$extensionsFilePath = if (!(Is64BitsOS)) {"C:\Program Files \Microsoft Visual Studio 14.0\Common7\IDE\Extensions\extensions.configurationchanged"} else {"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\extensions.configurationchanged"}

DeleteRegistryAndExtensionsFile -vsVersion "2015" `
	-vsRegistryPath $vsRegistryPath `
	-vsRegistryName $vsRegistryName `
	-extensionsFilePath $extensionsFilePath