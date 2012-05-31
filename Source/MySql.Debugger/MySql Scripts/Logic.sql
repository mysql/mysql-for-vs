CREATE DATABASE  IF NOT EXISTS `serversidedebugger` /*!40100 DEFAULT CHARACTER SET latin1 */ //

USE `serversidedebugger` //



CREATE FUNCTION `Peek`( pDebugSessionId int ) RETURNS varchar(50) CHARSET latin1
begin

    declare nextId int;
    declare returnValue varchar( 50 );
    
    set nextId = ( select max( Id ) from `ServerSideDebugger`.`DebugCallStack` where DebugSessionId = pDebugSessionId );
    set returnValue = ( select RoutineName from `ServerSideDebugger`.`DebugCallStack` 
        where ( DebugSessionId = pDebugSessionId ) and ( Id = nextId ));
    return returnValue;

end //

CREATE PROCEDURE `CleanupScope`( pDebugSessionId int )
begin
  
  delete from `ServerSideDebugger`.`DebugScope` where ( DebugSessionId = pDebugSessionId ) and ( DebugScopeLevel = @dbg_scopeLevel );
  update `ServerSideDebugger`.`DebugData` set Val = Val - 1 where Id = 1;

end //


CREATE PROCEDURE `DumpScopeVar`( pDebugSessionId int, pDebugScopeLevel int, pVarName varchar( 30 ), pVarValue binary )
begin
  
  replace DebugScope( DebugSessionId, DebugScopeLevel, VarName, VarValue ) values ( pDebugSessionId, pDebugScopeLevel, pVarName, pVarValue );
  
end //


CREATE PROCEDURE `ExitEnterCriticalSection`()
begin

  do release_lock( 'lock1' );
  do exists( select * from debugtbl limit 0 );
  do get_lock( 'lock1', 999999 );

end //


CREATE PROCEDURE `Pop`( pDebugSessionId int )
begin

    declare nextId int;
    set nextId = ( select max( Id ) from `ServerSideDebugger`.`DebugCallStack` where DebugSessionId = pDebugSessionId );
    delete from `ServerSideDebugger`.`DebugCallStack` where ( DebugSessionId = pDebugSessionId ) and ( Id = nextId );

end //


CREATE PROCEDURE `Push`( pDebugSessionId int, pRoutineName varchar( 50 ) )
begin

    insert into `ServerSideDebugger`.`DebugCallStack`( DebugSessionId, RoutineName ) values ( pDebugSessionId, pRoutineName );

end //

