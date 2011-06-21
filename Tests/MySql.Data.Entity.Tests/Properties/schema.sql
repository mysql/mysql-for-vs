DROP TABLE IF EXISTS SalariedEmployees;
DROP TABLE IF EXISTS Employees;
DROP TABLE IF EXISTS EmployeeChildren;
DROP TABLE IF EXISTS Toys;
DROP TABLE IF EXISTS Companies;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Stores;
DROP TABLE IF EXISTS Books;
DROP TABLE IF EXISTS Authors;
DROP TABLE IF EXISTS Publishers;
DROP TABLE IF EXISTS DataTypeTests;
DROP TABLE IF EXISTS DesktopComputers;
DROP TABLE IF EXISTS LaptopComputers;
DROP TABLE IF EXISTS TabletComputers;
DROP TABLE IF EXISTS Computers;

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

CREATE TABLE EmployeeChildren(
	Id INT UNSIGNED NOT NULL PRIMARY KEY,
	EmployeeId INT NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	FirstName NVARCHAR(10) NOT NULL,
	BirthTime TIME,
	Weight DOUBLE,
	LastModified TIMESTAMP NOT NULL);

INSERT INTO EmployeeChildren VALUES (1, 1, 'Flintstone', 'Pebbles', NULL, NULL, NULL);

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

CREATE TABLE Computers (
	`Id` INT NOT NULL AUTO_INCREMENT,
	`Brand` varchar(100) NOT NULL,
	CONSTRAINT PK_Computers PRIMARY KEY (Id)) ENGINE=InnoDB;

INSERT INTO Computers VALUES (1, 'Dell');		
INSERT INTO Computers VALUES (2, 'Acer');
INSERT INTO Computers VALUES (3, 'Toshiba');		
INSERT INTO Computers VALUES (4, 'Sony');
INSERT INTO Computers VALUES (5, 'Apple');		
INSERT INTO Computers VALUES (6, 'HP');

CREATE TABLE DesktopComputers (
  `IdDesktopComputer` INT NOT NULL ,
  `Color` VARCHAR(15) NULL DEFAULT NULL ,
  PRIMARY KEY (`IdDesktopComputer`) ,
  CONSTRAINT FK_DesktopComputer_Computer
    FOREIGN KEY (IdDesktopComputer)
    REFERENCES Computers (Id)) ENGINE = InnoDB;
    
INSERT INTO DesktopComputers VALUES (1, 'White');
INSERT INTO DesktopComputers VALUES (2, 'Black');

CREATE TABLE LaptopComputers (
  `IdLaptopComputer` INT NOT NULL ,
  `Size` VARCHAR(45) NULL DEFAULT NULL ,
  `IsCertified` BIT(1) NULL DEFAULT NULL ,
  PRIMARY KEY (IdLaptopComputer) ,
  CONSTRAINT FK_LaptopComputer_Computer
    FOREIGN KEY (IdLaptopComputer)
    REFERENCES Computers(Id)) ENGINE = InnoDB;

INSERT INTO LaptopComputers VALUES (3, '13.2 x 9.4', 1);
INSERT INTO LaptopComputers VALUES (4, '19.5 x 13', 0);

CREATE TABLE TabletComputers (
  `IdTabletComputer` INT NOT NULL ,
  `IsAvailable` BIT(1) NULL DEFAULT NULL ,
  `ReleaseDate` DATETIME NULL DEFAULT NULL ,
  PRIMARY KEY (IdTabletComputer) ,
  CONSTRAINT FK_TabletComputer_Computer
    FOREIGN KEY (IdTabletComputer)
    REFERENCES Computers(Id)) ENGINE = InnoDB;

INSERT INTO TabletComputers VALUES (5, 1, '2011-05-04');
INSERT INTO TabletComputers VALUES (6, 1, '2010-06-09');

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
INSERT INTO Orders VALUES (10, 3, 350.54721);	


CREATE TABLE Authors(
	id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
	`name` VARCHAR(20) NOT NULL,
	age INT) ENGINE=InnoDB;
INSERT INTO Authors VALUES (1, 'Tom Clancy', 65);
INSERT INTO Authors VALUES (2, 'Stephen King', 57);
INSERT INTO Authors VALUES (3, 'John Grisham', 49);
INSERT INTO Authors VALUES (4, 'Dean Koontz', 52);
INSERT INTO Authors VALUES (5, 'Don Box', 44);


CREATE TABLE Publishers(
	id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
	`name` VARCHAR(20) NOT NULL) ENGINE=InnoDB;
INSERT INTO Publishers VALUES (1, 'Acme Publishing');
	
CREATE TABLE Books (
	id INT AUTO_INCREMENT NOT NULL PRIMARY KEY,
	`name` VARCHAR(20) NOT NULL,
	pages int,
	author_id int NOT NULL,
	publisher_id int NOT NULL,
	FOREIGN KEY (author_id) REFERENCES Authors(id),
	FOREIGN KEY (publisher_id) REFERENCES Publishers(id)) ENGINE=InnoDB;
INSERT INTO Books VALUES (1, 'Debt of Honor', 200, 1, 1);
INSERT INTO Books VALUES (2, 'Insomnia', 350, 2, 1);
INSERT INTO Books VALUES (3, 'Rainmaker', 475, 3, 1);

SET @guid=UUID();
CREATE TABLE DataTypeTests(
	id CHAR(36) CHARACTER SET utf8 NOT NULL PRIMARY KEY,
	id2 CHAR(36) BINARY NOT NULL,
	idAsChar VARCHAR(36));
INSERT INTO DataTypeTests VALUES (@guid, @guid, @guid);	
INSERT INTO DataTypeTests VALUES ('481A6506-03A3-4ef9-A05A-B247E75A2FB4',
	'481A6506-03A3-4ef9-A05A-B247E75A2FB4', '481A6506-03A3-4ef9-A05A-B247E75A2FB4');

