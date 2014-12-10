/* 建立表格並制定格式 */
create Table Customers
(
	CustomerId varchar(10) Primary key,
	FirstName varchar(50),
	LastName varchar(10)
)
/*  */
Create Table Orders
(
	OrderId varchar(10) primary key,
	OrderDate varchar(8),
	CustomerId varchar(10)
)

alter Table Orders ADD Foreign key
(
	CustomerId
)
References Customers

Alter Table Orders ADD Verssion varchar(10)

Create Table Product
(
	ProductId varchar(20) Primary key,
	Verssion varchar(10),
	Name varchar(20),
	cost varchar(20)
)

Create Table OrderProduct
(
	OrderId varchar(10),
	ProductId varchar(20)
)

Alter Table OrderProduct ADD Foreign key
(OrderId) References Orders
Alter Table OrderProduct ADD Foreign key
(ProductId) References Product