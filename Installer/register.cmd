echo off
gacutil /u mysql.data
gacutil /u mysql.web
gacutil /u mysql.data.entity

gacutil /i MySql.Data\provider\bin\debug\mysql.data.dll
installutil mysql.data\provider\bin\debug\mysql.data.dll

gacutil /i MySql.Data.Entity\provider\bin\debug\mysql.data.entity.dll

gacutil /i MySql.Web\providers\bin\debug\mysql.web.dll
installutil mysql.web\providers\bin\debug\mysql.web.dll

installer\binary\globalinstaller mysql.visualstudio\bin\debug\mysql.visualstudio.dll version=VS2008 debug=true
