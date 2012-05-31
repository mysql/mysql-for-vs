
use ServerSideDebugger;

drop procedure if exists DumpScope;

/*
delimiter //
create procedure `DumpScope`( pDebugSessionId int, pCallstackDeepness int, pVars text )
begin
  
  declare v int;
  replace DebugScope( DebugSessionId, CallstackDeepness, Vars ) values ( pDebugSessionId, pCallstackDeepness, pVars );  
  do release_lock( 'lock1' );
  do exists( select * from debugtbl limit 0 );
  do get_lock( 'lock1', 999999 );
  
end;
//
*/

delimiter ;


delimiter //
drop procedure if exists `ExitEnterCriticalSection`;

create procedure `ExitEnterCriticalSection`()
begin

  do release_lock( 'lock1' );
  do exists( select * from debugtbl limit 0 );
  do get_lock( 'lock1', 999999 );

end;
//


delimiter //
create function `ExitEnterCriticalSectionFunction`( returnValue bit ) returns bit
begin

  do release_lock( 'lock1' );
  do exists( select * from debugtbl limit 0 );
  do get_lock( 'lock1', 999999 );
  return returnValue;

end;
//


delimiter ;
drop procedure if exists `DumpScopeVar`;

delimiter //

create procedure `DumpScopeVar`( pDebugSessionId int, pDebugScopeLevel int, pVarName varchar( 30 ), pVarValue binary )
begin
  
  replace DebugScope( DebugSessionId, DebugScopeLevel, VarName, VarValue ) values ( pDebugSessionId, pDebugScopeLevel, pVarName, pVarValue );
  
end;
//

delimiter ;

drop procedure if exists CleanupScope;

delimiter //
create procedure CleanupScope( pDebugSessionId int )
begin
  
  delete from `ServerSideDebugger`.`DebugScope` where ( DebugSessionId = pDebugSessionId ) and ( DebugScopeLevel = @dbg_scopeLevel );
  update `ServerSideDebugger`.`DebugData` set Val = Val - 1 where Id = 1;

end;
//


delimiter //
create function Peek( pDebugSessionId int ) returns varchar( 50 )
begin

    declare nextId int;
    declare returnValue varchar( 50 );
    
    set nextId = ( select max( Id ) from `ServerSideDebugger`.`DebugCallStack` where DebugSessionId = pDebugSessionId );
    set returnValue = ( select RoutineName from `ServerSideDebugger`.`DebugCallStack` 
        where ( DebugSessionId = pDebugSessionId ) and ( Id = nextId ));
    return returnValue;

end;
//


delimiter //
create procedure Push( pDebugSessionId int, pRoutineName varchar( 50 ) )
begin

    insert into `ServerSideDebugger`.`DebugCallStack`( DebugSessionId, RoutineName ) values ( pDebugSessionId, pRoutineName );

end;
//


delimiter //
create procedure Pop( pDebugSessionId int )
begin

    declare nextId int;
    set nextId = ( select max( Id ) from `ServerSideDebugger`.`DebugCallStack` where DebugSessionId = pDebugSessionId );
    delete from `ServerSideDebugger`.`DebugCallStack` where ( DebugSessionId = pDebugSessionId ) and ( Id = nextId );

end
//
