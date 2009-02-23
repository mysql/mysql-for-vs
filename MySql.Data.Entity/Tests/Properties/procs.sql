
DROP PROCEDURE IF EXISTS AddAuthor$$
DROP PROCEDURE IF EXISTS DeleteAuthor$$
DROP PROCEDURE IF EXISTS UpdateAuthor$$

CREATE PROCEDURE AddAuthor(theid INT, thename VARCHAR(20), theage INT) 
BEGIN
	INSERT INTO authors VALUES (theid, thename, theage);
END $$

CREATE PROCEDURE DeleteAuthor(theid int)
BEGIN
	DELETE FROM authors WHERE id=theid;
END $$

CREATE FUNCTION UpdateAuthor(theid int, thename varchar(20), theage int) RETURNS INT
BEGIN
	UPDATE authors SET `name`=thename, age=theage WHERE id=theid;
	RETURN 0;
END $$
