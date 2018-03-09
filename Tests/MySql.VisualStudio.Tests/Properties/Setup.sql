DROP DATABASE IF EXISTS {0};
CREATE DATABASE {0};
USE {0};

DROP USER IF EXISTS test@localhost;
DROP USER IF EXISTS test@'%';

CREATE USER 'test'@'localhost' IDENTIFIED BY 'test';
GRANT ALL ON {0}.* to 'test'@'localhost';
CREATE USER 'test'@'%' IDENTIFIED BY 'test';
GRANT ALL ON {0}.* to 'test'@'%';

FLUSH PRIVILEGES;

CREATE TABLE items (
  item_id int primary key auto_increment,
  description varchar(50) not null,
  brand varchar(50),
  price float not null,
  quantity int not null
);

CREATE TABLE stores(
  store_id int primary key auto_increment,
  name varchar(50) not null,
  address varchar(100)
);

CREATE TABLE employees(
  employee_id int primary key auto_increment,
  name varchar(50) not null,
  store_id int,
  active bool
);

CREATE TABLE orders(
  order_id int primary key auto_increment,
  employee_id int not null,
  client_name varchar(50)
);

CREATE TABLE order_details(
  order_id int not null,
  item_id int not null,
  quantity int,
  price float,
  discount float
);



-- Populating data

INSERT INTO stores VALUES(null, 'Matrix', 'Beverly Hills');

INSERT INTO items VALUES(null, 'Wings', 'OwnMark', 12.99, 12);
INSERT INTO items VALUES(null, 'Cookies', 'The Bear', 1.99, 1);
INSERT INTO items VALUES(null, 'Icecream', 'Vanilla', 2.99, 1);

INSERT INTO employees VALUES(null, 'EmployeeName', 1, true);

DROP DATABASE IF EXISTS {1};
CREATE DATABASE {1};
USE {1};

DROP USER IF EXISTS test@localhost;
DROP USER IF EXISTS test@'%';

CREATE USER 'test'@'localhost' IDENTIFIED BY 'test';
GRANT ALL ON {1}.* to 'test'@'localhost';
CREATE USER 'test'@'%' IDENTIFIED BY 'test';
GRANT ALL ON {1}.* to 'test'@'%';

FLUSH PRIVILEGES;

CREATE TABLE stuff(
  item_id int primary key auto_increment,
  description varchar(50) not null,
  brand varchar(50),
  price float not null,
  quantity int not null
);

CREATE TABLE mylines(
  line_id int primary key auto_increment,
  name varchar(50) not null,
  address varchar(100)
);

CREATE TABLE products(
  product_id int primary key auto_increment,
  name varchar(50) not null,
  store_id int,
  active bool
);

-- Populating data

INSERT INTO stuff VALUES(null, 'some description', 'some brand', 0.0, 1);
INSERT INTO mylines VALUES(null, 'line name', 'OwnMark');

-- Third database


DROP DATABASE IF EXISTS {2};
CREATE DATABASE {2};
USE {2};

DROP USER IF EXISTS test@localhost;
DROP USER IF EXISTS test@'%';

CREATE USER 'test'@'localhost' IDENTIFIED BY 'test';
GRANT ALL ON {2}.* to 'test'@'localhost';
CREATE USER 'test'@'%' IDENTIFIED BY 'test';
GRANT ALL ON {2}.* to 'test'@'%';

FLUSH PRIVILEGES;

CREATE TABLE Brands(
  brand_id int primary key auto_increment,
  name varchar(50) not null,
  description varchar(50) not null
);

-- Populating data

INSERT INTO brands VALUES(null, 'some name', 'brand description');

