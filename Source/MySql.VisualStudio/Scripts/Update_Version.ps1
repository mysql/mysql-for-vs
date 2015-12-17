function IsVersionChanged($filePath, $regEx, $regExReplacement, $version)
{
	$updated = $false
	#Get the file
	((Get-Content $filePath) |
		Foreach-Object {
			$line = $_
			#If the versions are different
			if (($line -match $regEx) -And !($line.Contains($version)))
			{
				$updated = $true
			}
		}
	)
	return $updated
}

function UpdateFile($filePath, $regEx, $regExReplacement, $version)
{
	$updated = IsVersionChanged -filePath $filePath `
		-regEx $regEx `
		-regExReplacement $regExReplacement `
		-version $version

	if ($updated)
	{
		((Get-Content $filePath) |
		Foreach-Object {
			$line = $_
			#If the versions are different, update the version
			if (($line -match $regEx) -And !($line.Contains($version)))
			{
				$line = $line -replace $regEx, $regExReplacement
			}
			$line
		} |
		Set-Content ($filePath)
		)
	}
}

#Get the version from the assembly
$version = Get-Content "..\..\Source\MySql.VisualStudio\Properties\VersionInfo.cs" |
 Where-Object { $_.Contains("AssemblyVersion") }
$version = $version.Substring(28,5)

#Update the version in the specified files
UpdateFile -filePath "..\..\Source\MySql.VisualStudio\source.extension.vsixmanifest" `
	-regEx "<Version>[0-9\.]+</Version>" `
	-regExReplacement ("<Version>" + $version + "</Version>") `
	-version $version

UpdateFile -filePath "..\..\Source\MySql.VisualStudio\ItemTemplates\CSharp\MySQL\MySQL_Web.zip\MySQL_Web_ItemTemplate.vstemplate" `
	-regEx "Version=[0-9\.]+" `
	-regExReplacement ("Version=" + $version + ".0") `
	-version $version

UpdateFile -filePath "..\..\Source\MySql.VisualStudio\ItemTemplates\CSharp\MySQL\MySQL_WinForm.zip\MySQL_WinForm_ItemTemplate.vstemplate" `
	-regEx "Version=[0-9\.]+" `
	-regExReplacement ("Version=" + $version + ".0") `
	-version $version

UpdateFile -filePath "..\..\Source\MySql.VisualStudio\ItemTemplates\VisualBasic\MySQL\MySQL_VB_Web.zip\MySQL_Web_VB_ItemTemplate.vstemplate" `
	-regEx "Version=[0-9\.]+" `
	-regExReplacement ("Version=" + $version + ".0") `
	-version $version

UpdateFile -filePath "..\..\Source\MySql.VisualStudio\ItemTemplates\VisualBasic\MySQL\MySQL_VB_WinForm.zip\MySQL_WinForm_VB_ItemTemplate.vstemplate" `
	-regEx "Version=[0-9\.]+" `
	-regExReplacement ("Version=" + $version + ".0") `
	-version $version


