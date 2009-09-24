ALTER TABLE my_aspnet_Sessions CONVERT TO CHARACTER SET DEFAULT;
ALTER TABLE my_aspnet_Sessions MODIFY SessionItems LONGBLOB;

UPDATE my_aspnet_SchemaVersion SET version=6;