@echo off

REM Make sure we are called with a version
IF [%1] == [] GOTO Usage

REM Make sure our files are ready
IF NOT EXIST MySql.Data\provider\bin\debug\mysql.data.dll GOTO NOTREADY
IF NOT EXIST MySql.Web\providers\bin\debug\mysql.web.dll GOTO NOTREADY
IF NOT EXIST mysql.visualstudio\bin\debug\mysql.visualstudio.dll GOTO NOTREADY
IF NOT %1 == 2005 AND NOT EXIST MySql.Data.Entity\provider\bin\debug\mysql.data.entity.dll GOTO NOTREADY

REM Unregister our assemblies (this will work if they are not registered)
gacutil /u mysql.data
gacutil /u mysql.web
gacutil /u mysql.data.entity

REM Now register the core assembly
gacutil /i MySql.Data\provider\bin\debug\mysql.data.dll
installutil mysql.data\provider\bin\debug\mysql.data.dll

REM Register web assembly
gacutil /i MySql.Web\providers\bin\debug\mysql.web.dll
installutil mysql.web\providers\bin\debug\mysql.web.dll

REM If we are not on 2005 then register the entity assembly
if NOT %1 == 2005 gacutil /i MySql.Data.Entity\provider\bin\debug\mysql.data.entity.dll

REm Now register the visual studio bits
set cmd=version=VS%1 debug=true ranu=true
if %1 == 2005 SET cmd=version=VS2005 debug=true
installer\binary\globalinstaller mysql.visualstudio\bin\debug\mysql.visualstudio.dll %cmd%
EXIT /B 0

:NOTREADY
ECHO some files are not ready
EXIT /B 1

:USAGE
ECHO version missing