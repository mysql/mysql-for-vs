
drop database if exists ServerSideDebugger;

create database ServerSideDebugger;

use ServerSideDebugger;

create table DebugSessions(
    DebugSessionId int auto_increment primary key
);

CREATE TABLE `debugscope` (
  `DebugSessionId` int(11) NOT NULL DEFAULT '0',
  `DebugScopeLevel` int(11) NOT NULL DEFAULT '0',
  `VarName` varchar(30) NOT NULL DEFAULT '',
  `VarValue` varbinary(50000) DEFAULT NULL,
  `Stamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`DebugSessionId`,`DebugScopeLevel`,`VarName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

create table DebugData( Id int auto_increment primary key, `Name` varchar( 50 ), Val varchar( 50 ) ) engine=MyIsam;

insert into DebugData( `Name`, Val ) values ( 'ScopeLevel', 0 );
insert into DebugData( `Name`, Val ) values ( 'last_insert_id', 0 );
insert into DebugData( `Name`, Val ) values ( 'row_count', 0 );

create table DebugCallstack (
    Id int auto_increment primary key,
    DebugSessionId int,
    RoutineName varchar( 50 )
) engine=MyIsam;


CREATE TABLE `debugtbl` (
  `Id` int(11) NOT NULL,
  `Val` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyIsam;

insert into `debugtbl`( Id, Val ) values ( 1, 1 );