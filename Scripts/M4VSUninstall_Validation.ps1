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

function ValidateRegistryAndExtensionsFile($vsVersion, $pluginRegistryPath, $pluginRegistryName, $vsRegistryPath, $vsRegistryName, $vsRegistryValue, $extensionsFilePath)
{
	echo $("`n`n -- Check whether the M4VS plugin is installed for VS" + $vsVersion + ".")
	if (Test-RegistryValue -Path $pluginRegistryPath -Name $pluginRegistryName)
	{
		echo $("> The script detected that the M4VS plugin is installed for VS" + $vsVersion + ". Checking if registry key needs to be created.")
		if (!(Test-RegistryValue -Path $vsRegistryPath -Name $vsRegistryName))
		{
			echo $("> The registry key doesn't exists for VS" + $vsVersion + ". The script will create it.")
			New-Item -Path $vsRegistryPath -Force
			New-ItemProperty -Path $vsRegistryPath -Name $vsRegistryName -Value $vsRegistryValue -PropertyType String -Force

			### Write in the registry that we have created the registry key
			New-ItemProperty -Path "hkcu:\Console" -Name $("M4VSUninstall_RegKey_" + $vsVersion) -Value "1" -PropertyType String -Force

			echo $("-- Check whether the extensions file exists for VS" + $vsVersion + ".")
			if (!(Test-Path $extensionsFilePath))
			{
				echo $("> The extensions file doesn't exists for VS" + $vsVersion + ". The script will create it.")
				New-Item $extensionsFilePath -type file -force

				### Write in the registry that we have created the file
				New-ItemProperty -Path "hkcu:\Console" -Name $("M4VSUninstall_ExtFile_" + $vsVersion) -Value "1" -PropertyType String -Force
			}
			else
			{
				echo $("> The extensions file already exists for VS" + $vsVersion + ".")
			}
		}
		else
		{
			echo $("> The registry key for VS" + $vsVersion + " already exists.")
		}
	}
	else
	{
		echo $("> The script detected that the M4VS plugin is NOT installed for VS" + $vsVersion + ".")
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

$pluginRegistryPath = "hklm:\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders"

###### VS 2012 ######
$pluginRegistryName = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\"} else {"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\"}
$vsRegistryPath = if (!(Is64BitsOS)) {"HKLM:\Software\Microsoft\VisualStudio\11.0\Setup\VS"} else {"HKLM:\Software\Wow6432Node\Microsoft\VisualStudio\11.0\Setup\VS"}
$vsRegistryName = "EnvironmentDirectory"
$vsRegistryValue = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\"} else { "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\"}
$extensionsFilePath = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\extensions.configurationchanged"} else {"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\Extensions\extensions.configurationchanged"}

ValidateRegistryAndExtensionsFile -vsVersion "2012" `
	-pluginRegistryPath $pluginRegistryPath `
	-pluginRegistryName $pluginRegistryName `
	-vsRegistryPath $vsRegistryPath `
	-vsRegistryName $vsRegistryName `
	-vsRegistryValue $vsRegistryValue `
	-extensionsFilePath $extensionsFilePath

###### VS 2013 ######
$pluginRegistryName = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\"} else {"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\"}
$vsRegistryPath = if (!(Is64BitsOS)) {"HKLM:\Software\Microsoft\VisualStudio\12.0\"} else {"HKLM:\Software\Wow6432Node\Microsoft\VisualStudio\12.0\"}
$vsRegistryName = "ShellFolder"
$vsRegistryValue = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 12.0\"} else { "C:\Program Files (x86)\Microsoft Visual Studio 12.0\"}
$extensionsFilePath = if (!(Is64BitsOS)) {"C:\Program Files \Microsoft Visual Studio 12.0\Common7\IDE\Extensions\extensions.configurationchanged"} else {"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\extensions.configurationchanged"}

ValidateRegistryAndExtensionsFile -vsVersion "2013" `
	-pluginRegistryPath $pluginRegistryPath `
	-pluginRegistryName $pluginRegistryName `
	-vsRegistryPath $vsRegistryPath `
	-vsRegistryName $vsRegistryName `
	-vsRegistryValue $vsRegistryValue `
	-extensionsFilePath $extensionsFilePath

###### VS 2015 ######
$pluginRegistryName = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\"} else {"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\"}
$vsRegistryPath = if (!(Is64BitsOS)) {"HKLM:\Software\Microsoft\VisualStudio\14.0\"} else {"HKLM:\Software\Wow6432Node\Microsoft\VisualStudio\14.0\"}
$vsRegistryName = "ShellFolder"
$vsRegistryValue = if (!(Is64BitsOS)) {"C:\Program Files\Microsoft Visual Studio 14.0\"} else { "C:\Program Files (x86)\Microsoft Visual Studio 14.0\"}
$extensionsFilePath = if (!(Is64BitsOS)) {"C:\Program Files \Microsoft Visual Studio 14.0\Common7\IDE\Extensions\extensions.configurationchanged"} else {"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions\extensions.configurationchanged"}

ValidateRegistryAndExtensionsFile -vsVersion "2015" `
	-pluginRegistryPath $pluginRegistryPath `
	-pluginRegistryName $pluginRegistryName `
	-vsRegistryPath $vsRegistryPath `
	-vsRegistryName $vsRegistryName `
	-vsRegistryValue $vsRegistryValue `
	-extensionsFilePath $extensionsFilePath