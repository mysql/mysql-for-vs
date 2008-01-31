/* Provider schema block -- version 3 */

/* create our application and user tables */
create table my_aspnet_Applications(id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(256), description VARCHAR(256));
create table my_aspnet_Users(id INT PRIMARY KEY AUTO_INCREMENT, applicationId INT, name VARCHAR(256), isAnonymous TINYINT(1), lastActivityDate DATETIME);
create table my_aspnet_Profiles(userId INT PRIMARY KEY, valueindex longtext, stringdata longtext, binarydata longblob, lastUpdatedDate timestamp);
create table my_aspnet_SchemaVersion(version INT);
 
insert into my_aspnet_SchemaVersion VALUES (3);
 
/* now we need to migrate all applications into our apps table */
insert into my_aspnet_Applications (name) select ApplicationName from mysql_membership UNION select ApplicationName from mysql_UsersInRoles;

/* now we make our changes to the existing tables */
alter table mysql_membership
          rename to my_aspnet_Membership,
          drop primary key,
          drop column pkid,
          drop column isonline,
          add column userId INT FIRST,
          add column applicationId INT AFTER userId;
          
alter table mysql_roles
          rename to my_aspnet_Roles,
          drop key rolename,
          add column id INT PRIMARY KEY AUTO_INCREMENT FIRST,
          add column applicationId INT AFTER id;
          
alter table mysql_UsersInRoles
          drop key username,
          rename to my_aspnet_UsersInRoles,
          add column userId INT FIRST,
          add column roleId INT AFTER userId,
          add column applicationId INT AFTER roleId;

/* these next lines set the application Id on our tables appropriately */          
update my_aspnet_Membership m, my_aspnet_Applications a set m.applicationId = a.id where a.name=m.ApplicationName;
update my_aspnet_Roles r, my_aspnet_Applications a set r.applicationId = a.id where a.name=r.ApplicationName;
update my_aspnet_UsersInRoles u, my_aspnet_Applications a set u.applicationId = a.id where a.name=u.ApplicationName;

/* now merge our usernames into our users table */
insert into my_aspnet_Users (applicationId, name) 
        select applicationId, username from my_aspnet_membership
        UNION select applicationId, username from my_aspnet_UsersInRoles; 
          
/* now set the user ids in our tables accordingly */        
update my_aspnet_Membership m, my_aspnet_Users u set m.userId = u.id where u.name=m.UserName AND u.applicationId=m.ApplicationId;
update my_aspnet_UsersInRoles r, my_aspnet_Users u set r.userId = u.id where u.name=r.UserName AND u.applicationId=r.applicationId;

/* now update the isanonymous and last activity date fields for the users */        
update my_aspnet_users u, my_aspnet_membership m 
        set u.isAnonymous=0, u.lastActivityDate=m.lastActivityDate 
        where u.name = m.username;

/* make final changes to our tables */        
alter table my_aspnet_Membership
          drop column username,
          drop column applicationName,
          drop column applicationId,
          add primary key (userId);
          
/* next we set our role id values appropriately */
update my_aspnet_UsersInRoles u, my_aspnet_Roles r set u.roleId = r.id where u.rolename = r.rolename and r.applicationId=u.applicationId;

/* now we make the final changes to our roles tables */                    
alter table my_aspnet_Roles
          drop column applicationName,
          change column rolename name VARCHAR(255) NOT NULL;
          
alter table my_aspnet_UsersInRoles
          drop column applicationName,
          drop column applicationId,
          drop column username,
          drop column rolename,
          add primary key (userId, roleId);
          
          
          