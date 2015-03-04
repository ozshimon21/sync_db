
USE Master

IF Exists(Select * From sys.databases Where [name] = 'Moked')
	Drop database Moked
go

Create database Moked
go

USE Moked

CREATE TABLE Stations(Id int Identity(1,1) primary key,Name varchar (255), Number int NULL)

go


