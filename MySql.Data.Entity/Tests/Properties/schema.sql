CREATE TABLE Employees(
	Id INT NOT NULL PRIMARY KEY,
	LastName NVARCHAR(20) NOT NULL, 
	FirstName NVARCHAR(10) NOT NULL,
	Age INT) ENGINE=InnoDB;

INSERT INTO Employees VALUES (1, 'Flintstone', 'Fred', 43);
INSERT INTO Employees VALUES (2, 'Flintstone', 'Wilma', 37);
INSERT INTO Employees VALUES (3, 'Rubble', 'Barney', 41);
INSERT INTO Employees VALUES (4, 'Rubble', 'Betty', 35);
INSERT INTO Employees VALUES (5, 'Slate', 'S', 62);
INSERT INTO Employees VALUES (6, 'Doo', 'Scooby', 7);
INSERT INTO Employees VALUES (7, 'Underdog', 'J', 12);

CREATE TABLE SalariedEmployees(
	EmployeeId INT NOT NULL PRIMARY KEY,
	Salary INT NOT NULL,
	CONSTRAINT FOREIGN KEY (EmployeeId) REFERENCES Employees (Id)) Engine=InnoDB;
	
INSERT INTO salariedEmployees VALUES (5, 500);
INSERT INTO salariedEmployees VALUES (7, 50);

DROP TABLE IF EXISTS Companies;
CREATE TABLE Companies (
	`Id` INT NOT NULL AUTO_INCREMENT,
	`Name` VARCHAR(100) NOT NULL,
	`DateBegan` DATETIME,
	`NumEmployees` INT,
	`Address` VARCHAR(50),
	`City` VARCHAR(50),
	`State` CHAR(2),
	`ZipCode` CHAR(9),
	CONSTRAINT PK_Companies PRIMARY KEY (Id)) ENGINE=InnoDB;

INSERT INTO Companies VALUES (1, 'Hasbro', '1996-11-15 5:18:23', 200, '123 My Street', 'Nashville', 'TN', 12345);
INSERT INTO Companies VALUES (2, 'Acme', NULL, 55, '45 The Lane', 'St. Louis', 'MO', 44332);
INSERT INTO Companies VALUES (3, 'Bandai America', NULL, 376, '1 Infinite Loop', 'Cupertino', 'CA', 54321);
INSERT INTO Companies VALUES (4, 'Lego Group', NULL, 700, '222 Park Circle', 'Lexington', 'KY', 32323);
INSERT INTO Companies VALUES (5, 'Mattel', NULL, 888, '111 Parkwood Ave', 'San Jose', 'CA', 55676);
INSERT INTO Companies VALUES (6, 'K''NEX', NULL, 382, '7812 N. 51st', 'Dallas', 'TX', 11239);
INSERT INTO Companies VALUES (7, 'Playmobil', NULL, 541, '546 Main St.', 'Omaha', 'NE', 78439);

DROP TABLE IF EXISTS Toys;
CREATE TABLE Toys (
	`Id` INT NOT NULL AUTO_INCREMENT,
	`SupplierId` INT NOT NULL,
	`Name` varchar(100) NOT NULL,
	`MinAge` int NOT NULL,
	CONSTRAINT PK_Toys PRIMARY KEY (Id),
	KEY `SupplierId` (`SupplierId`),
	FOREIGN KEY (SupplierId) REFERENCES Companies(Id) ) ENGINE=InnoDB;
	
INSERT INTO Toys VALUES (1, 3, 'Slinky', 2);	
INSERT INTO Toys VALUES (2, 2, 'Rubiks Cube', 5);	
INSERT INTO Toys VALUES (3, 1, 'Lincoln Logs', 3);	
INSERT INTO Toys VALUES (4, 4, 'Legos', 4);	

DROP TABLE IF EXISTS Stores;
CREATE TABLE Stores (
	id INT PRIMARY KEY,
	`name` VARCHAR(50) NOT NULL,
	address VARCHAR(50),
	city VARCHAR(50),
	state CHAR(2),
	zipcode CHAR(9)	
	) ENGINE=InnoDB;
INSERT INTO Stores VALUES (1, 'Target', '2417 N. Haskell Ave', 'Dallas', 'TX', '75204');
INSERT INTO Stores VALUES (2, 'K-Mart', '4225 W. Indian School Rd.', 'Phoenix', 'AZ', '85019');
INSERT INTO Stores VALUES (3, 'Wal-Mart', '1238 Putty Hill Ave', 'Towson', 'MD', '21286');	
	
	
DROP TABLE IF EXISTS Orders;
CREATE TABLE Orders (
	id INT PRIMARY KEY,
	storeId INT NOT NULL,
	freight DOUBLE NOT NULL,
	FOREIGN KEY (storeId) REFERENCES Stores(id)) ENGINE=InnoDB;
INSERT INTO Orders VALUES (1, 1, 65.3);
INSERT INTO Orders VALUES (2, 2, 127.8);
INSERT INTO Orders VALUES (3, 3, 254.78);	
INSERT INTO Orders VALUES (4, 1, 165.8);
INSERT INTO Orders VALUES (5, 2, 85.2);
INSERT INTO Orders VALUES (6, 3, 98.5);	
INSERT INTO Orders VALUES (7, 1, 222.3);
INSERT INTO Orders VALUES (8, 2, 125);
INSERT INTO Orders VALUES (9, 3, 126.4);	
	