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

