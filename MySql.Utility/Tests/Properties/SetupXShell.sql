# First database
CREATE SCHEMA IF NOT EXISTS `{0}`;
USE `{0}`;

/* Grant privileges to test user to ensure we can do whatever we need with the database */
GRANT ALL ON `{0}`.* to 'test'@'localhost' IDENTIFIED BY 'test';
GRANT ALL ON `{0}`.* to 'test'@'%' IDENTIFIED BY 'test';
FLUSH PRIVILEGES;

# Creating table
DROP TABLE IF EXISTS `character`;
CREATE TABLE `character` ( /* in-line comment for testing of the tokenizer */
  `character_id` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(30) NOT NULL,
  `age` smallint(4) unsigned NOT NULL,
  `gender` enum('male', 'female') NOT NULL,
  `from` varchar(30) DEFAULT '' NOT NULL,
  `universe` varchar(30) NOT NULL,
  `base` bool DEFAULT false NOT NULL,
  PRIMARY KEY (`character_id`),
  UNIQUE KEY `idx_name` (`name`),
  KEY `idx_base` (`base`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Populating data
INSERT INTO `character` (`name`, `age`, `gender`, `universe`, `base`) VALUES
('Sarah Palmer', 31, 'female', 'Halo 4', TRUE),
('Avery Johnson', 78, 'male', 'Halo', TRUE),
('Miranda Keys', 27, 'female', 'Halo 2', TRUE),
('Catherine Halsey', 66, 'female', 'Halo: Reach', TRUE),
('Catherine-B320', 22, 'female', 'Halo: Reach', TRUE),
('Jun-A266', 34, 'male', 'Halo: Reach', TRUE),
('Jorge-052', 41, 'male', 'Halo: Reach', TRUE),
('Emile-A239', 29, 'male', 'Halo: Reach', TRUE),
('Carter-A259', 32, 'male', 'Halo: Reach', TRUE),
('John-117', 47, 'male', 'Halo', TRUE),
('Linda-058', 47, 'female', 'Halo 5', TRUE),
('Jameson Locke', 29, 'male', 'Halo 5', TRUE),
('Kelly-087', 48, 'female', 'Halo 5', TRUE),
('Frederic-104', 47, 'male', 'Halo 5', TRUE),
('Edward Buck', 48, 'male', 'Halo 5', TRUE),
('Holly Tanaka', 28, 'female', 'Halo 5', TRUE),
('Olympia Vale', 22, 'female', 'Halo 5', TRUE),
('Thane Krios 2', 39, 'male', 'Mass Effect 2', TRUE),
('Jeff Moreau', 30, 'male', 'Mass Effect', TRUE),
('Jacob Taylor', 28, 'male', 'Mass Effect 2', TRUE),
('Kasumi Goto', 25, 'female', 'Mass Effect 2', TRUE),
('Jacqueline Nought', 24, 'female', 'Mass Effect 2', TRUE),
('Saren Arterius', 44, 'male', 'Mass Effect', TRUE),
('Mordin Solus', 50, 'male', 'Mass Effect 2', TRUE),
('David Anderson', 48, 'male', 'Mass Effect', TRUE),
('Zaeed Massani', 55, 'male', 'Mass Effect 2', TRUE),
('Kaidan Alenko', 35, 'male', 'Mass Effect', TRUE);

# Create a stored procedure just for the sake of testing the tokenizer handling of the delimiter override
DELIMITER $$
DROP PROCEDURE IF EXISTS test_procedure$$
CREATE PROCEDURE test_procedure()
BEGIN
  DECLARE v_charcount INTEGER; 
  SELECT name, gender, age FROM `character` ORDER BY age;
  SELECT COUNT(*) INTO v_charcount FROM `character`;
  SET @x = 0;
  # This repeat does not do anything, but we add it to include an END that does not close the BEGIN block
  REPEAT
	SET @x = @x + 1;
	UNTIL @x > v_charcount
  END REPEAT;
END$$
CALL test_procedure()$$
DELIMITER ;

# Creating views
CREATE OR REPLACE VIEW `halo_characters` AS SELECT name, age, gender FROM `character` WHERE universe LIKE 'Halo%';
CREATE OR REPLACE VIEW `mass_effect_characters` AS SELECT name, age, gender FROM `character` WHERE universe LIKE 'Mass%';
CREATE OR REPLACE VIEW `spartan_characters` AS SELECT name, age, gender FROM `character` WHERE universe LIKE 'Halo%' AND name LIKE '%-%';