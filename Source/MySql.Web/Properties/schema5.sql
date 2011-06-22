CREATE TABLE my_aspnet_Sessions
(
  SessionId       varchar(255)  NOT NULL,
  ApplicationId   int       NOT NULL,
  Created         datetime  NOT NULL,
  Expires         datetime  NOT NULL,
  LockDate        datetime  NOT NULL,
  LockId          int       NOT NULL,
  Timeout         int       NOT NULL,
  Locked          tinyint(1)   NOT NULL,
  SessionItems    BLOB,
  Flags           int   NOT NULL,
  primary key (SessionId,ApplicationId)
)  DEFAULT CHARSET=latin1;

/*
  Cleaning up timed out sessions.
  In 5.1 events provide a support for periodic jobs.
  In older version we need a do-it-yourself event.
*/
CREATE TABLE my_aspnet_SessionCleanup
(
  LastRun   datetime NOT NULL,
  IntervalMinutes int NOT NULL
);

INSERT INTO my_aspnet_SessionCleanup(LastRun,IntervalMinutes) values(NOW(), 10);

UPDATE my_aspnet_SchemaVersion SET version=5;

