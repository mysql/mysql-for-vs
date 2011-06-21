/* Provider schema block -- version 3 */

/* create our application and user tables */
create table my_aspnet_Applications(id INT PRIMARY KEY AUTO_INCREMENT, name VARCHAR(256), description VARCHAR(256));
create table my_aspnet_Users(id INT PRIMARY KEY AUTO_INCREMENT, 
		applicationId INT NOT NULL, name VARCHAR(256) NOT NULL, 
		isAnonymous TINYINT(1) NOT NULL DEFAULT 1, lastActivityDate DATETIME);
create table my_aspnet_Profiles(userId INT PRIMARY KEY, valueindex longtext, stringdata longtext, binarydata longblob, lastUpdatedDate timestamp);
create table my_aspnet_SchemaVersion(version INT);

insert into my_aspnet_SchemaVersion VALUES (3);
 
/* now we need to migrate all applications into our apps table */
insert into my_aspnet_Applications (name) select ApplicationName from mysql_Membership UNION select ApplicationName from mysql_UsersInRoles;

/* now we make our changes to the existing tables */
alter table mysql_Membership
          rename to my_aspnet_Membership,
          drop primary key,
          drop column pkid,
          drop column IsOnline,
          add column userId INT FIRST,
          add column applicationId INT AFTER userId;
          
alter table mysql_Roles
          rename to my_aspnet_Roles,
          drop key Rolename,
          add column id INT PRIMARY KEY AUTO_INCREMENT FIRST,
          add column applicationId INT NOT NULL AFTER id;
          
alter table mysql_UsersInRoles
          drop key Username,
          rename to my_aspnet_UsersInRoles,
          add column userId INT FIRST,
          add column roleId INT AFTER userId,
          add column applicationId INT AFTER roleId;

ALTER TABLE my_aspnet_Membership CONVERT TO CHARACTER SET DEFAULT;
ALTER TABLE my_aspnet_Roles CONVERT TO CHARACTER SET DEFAULT;
ALTER TABLE my_aspnet_UsersInRoles CONVERT TO CHARACTER SET DEFAULT;

/* these next lines set the application Id on our tables appropriately */          
update my_aspnet_Membership m, my_aspnet_Applications a set m.applicationId = a.id where a.name=m.ApplicationName;
update my_aspnet_Roles r, my_aspnet_Applications a set r.applicationId = a.id where a.name=r.ApplicationName;
update my_aspnet_UsersInRoles u, my_aspnet_Applications a set u.applicationId = a.id where a.name=u.ApplicationName;

/* now merge our usernames into our users table */
insert into my_aspnet_Users (applicationId, name) 
        select applicationId, Username from my_aspnet_Membership
        UNION select applicationId, Username from my_aspnet_UsersInRoles; 
          
/* now set the user ids in our tables accordingly */        
update my_aspnet_Membership m, my_aspnet_Users u set m.userId = u.id where u.name=m.Username AND u.applicationId=m.applicationId;
update my_aspnet_UsersInRoles r, my_aspnet_Users u set r.userId = u.id where u.name=r.Username AND u.applicationId=r.applicationId;

/* now update the isanonymous and last activity date fields for the users */        
update my_aspnet_Users u, my_aspnet_Membership m 
        set u.isAnonymous=0, u.lastActivityDate=m.LastActivityDate 
        where u.name = m.Username;

/* make final changes to our tables */        
alter table my_aspnet_Membership
          drop column Username,
          drop column ApplicationName,
          drop column applicationId,
          add primary key (userId);
          
/* next we set our role id values appropriately */
update my_aspnet_UsersInRoles u, my_aspnet_Roles r set u.roleId = r.id where u.Rolename = r.Rolename and r.applicationId=u.applicationId;

/* now we make the final changes to our roles tables */                    
alter table my_aspnet_Roles
          drop column ApplicationName,
          change column Rolename name VARCHAR(255) NOT NULL;
          
alter table my_aspnet_UsersInRoles
          drop column ApplicationName,
          drop column applicationId,
          drop column Username,
          drop column Rolename,
          add primary key (userId, roleId);
          
          
          
          