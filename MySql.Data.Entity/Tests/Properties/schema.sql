CREATE TABLE employees(
	employeeId INT PRIMARY KEY,
	lastName NVARCHAR(20), 
	firstName NVARCHAR(10),
	age INT) ENGINE=InnoDB;

INSERT INTO employees VALUES (1, 'Flintstone', 'Fred', 43);
INSERT INTO employees VALUES (2, 'Flintstone', 'Wilma', 37);
INSERT INTO employees VALUES (3, 'Rubble', 'Barney', 41);
INSERT INTO employees VALUES (4, 'Rubble', 'Betty', 35);
INSERT INTO employees VALUES (5, 'Slate', 'S', 62);
INSERT INTO employees VALUES (6, 'Doo', 'Scooby', 7);
INSERT INTO employees VALUES (7, 'Underdog', 'J', 12);

CREATE TABLE salariedEmployees(
	employeeId INT PRIMARY KEY,
	salary INT,
	CONSTRAINT FOREIGN KEY (employeeId) REFERENCES employees (employeeId)) Engine=InnoDB;
	
INSERT INTO salariedEmployees VALUES (5, 500);
INSERT INTO salariedEmployees VALUES (7, 50);

DROP TABLE IF EXISTS Companies;
CREATE TABLE Companies (
	id INT PRIMARY KEY,
	`name` VARCHAR(100) NOT NULL,
	dateBegan DATETIME,
	numEmployees INT,
	address VARCHAR(50),
	city VARCHAR(50),
	state CHAR(2),
	zipcode CHAR(9)) ENGINE=InnoDB;

INSERT INTO Companies VALUES (1, 'Hasbro', '1996-11-15 5:18:23', 200, '123 My Street', 'Nashville', 'TN', 12345);
INSERT INTO Companies VALUES (2, 'Acme', NULL, 55, '45 The Lane', 'St. Louis', 'MO', 44332);
INSERT INTO Companies VALUES (3, 'Bandai America', NULL, 376, '1 Infinite Loop', 'Cupertino', 'CA', 54321);
INSERT INTO Companies VALUES (4, 'Lego Group', NULL, 700, '222 Park Circle', 'Lexington', 'KY', 32323);
INSERT INTO Companies VALUES (5, 'Mattel', NULL, 888, '111 Parkwood Ave', 'San Jose', 'CA', 55676);
INSERT INTO Companies VALUES (6, 'K''NEX', NULL, 382, '7812 N. 51st', 'Dallas', 'TX', 11239);
INSERT INTO Companies VALUES (7, 'Playmobil', NULL, 541, '546 Main St.', 'Omaha', 'NE', 78439);

DROP TABLE IF EXISTS toys;
CREATE TABLE toys (
	id INT PRIMARY KEY,
	makerId INT,
	`name` varchar(100) NOT NULL,
	minage int NOT NULL,
	FOREIGN KEY (makerId) REFERENCES Companies(id) ) ENGINE=InnoDB;
	
INSERT INTO toys VALUES (1, 3, 'Slinky', 2);	
INSERT INTO toys VALUES (2, 2, 'Rubiks Cube', 5);	
INSERT INTO toys VALUES (3, 1, 'Lincoln Logs', 3);	
INSERT INTO toys VALUES (4, 4, 'Legos', 4);	

DROP TABLE IF EXISTS Stores;
CREATE TABLE Stores (
	id INT PRIMARY KEY,
	`name` VARCHAR(50) NOT NULL) ENGINE=InnoDB;
INSERT INTO Stores VALUES (1, 'Target');
INSERT INTO Stores VALUES (2, 'K-Mart');
INSERT INTO Stores VALUES (3, 'Wal-Mart');	
	
	
DROP TABLE IF EXISTS Orders;
CREATE TABLE Orders (
	id INT PRIMARY KEY,
	storeId INT NOT NULL,
	freight DOUBLE NOT NULL) ENGINE=InnoDB;
INSERT INTO Orders VALUES (1, 1, 65.3);
INSERT INTO Orders VALUES (2, 2, 127.8);
INSERT INTO Orders VALUES (3, 3, 254.78);	
INSERT INTO Orders VALUES (4, 1, 165.8);
INSERT INTO Orders VALUES (5, 2, 85.2);
INSERT INTO Orders VALUES (6, 3, 98.5);	
INSERT INTO Orders VALUES (7, 1, 222.3);
INSERT INTO Orders VALUES (8, 2, 125);
INSERT INTO Orders VALUES (9, 3, 126.4);	
	