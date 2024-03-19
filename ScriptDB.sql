CREATE DATABASE Law_v2

USE Law_v2;

Create Table Artical (
	ID INT Primary key IDENTITY(1,1),
	Number INT,
	Name NVARCHAR(250),
	Title nvarchar(100),
	Content NVARCHAR(MAX),
	DateActive DATETIME,
	DateDeactive DATETIME,
	Status INT,
	Chapter_ID INT,
	Section_ID INT,
	Law_ID INT
)

Create Table ArticalVector (
	Artical_ID int Primary Key,
	Vector nvarchar(max),
	Law_ID int
)

Create Table Chapter (
	ID int Identity(1,1) Primary Key,
	Number int,
	Name nvarchar(250),
	Title nvarchar(500),
	DateActive DateTime,
	DateDeactive DateTime,
	Status int,
	Law_ID int
)

Create Table Section (
	ID int Identity(1,1) Primary Key,
	Number int,
	Name nvarchar(500),
	Title nvarchar(100),
	DateActive DateTime,
	DateDeactive DateTime,
	Status int,
	Chapter_ID int,
	Law_ID int
)

Create Table Clause (
	ID int Identity(1,1) Primary Key,
	Number int,
	Content nvarchar(max),
	DateActive DateTime,
	DateDeactive DateTime,
	Status int,
	Artical_ID int
)

Create Table Concept (
	ID int Identity(1,1) Primary Key,
	Name nvarchar(255),
	Description nvarchar(255)
)

Create Table ConceptKeyPhrase (
	ID int Identity(1,1) Primary Key,
	Concept_ID int,
	KeyPhrase_ID int,
	Count int,
	Law_ID int
)

Create Table ConceptMapping (
	ID int Identity(1,1) Primary key,
	Concept_ID int,
	KeyPhrase_ID int,
	Law_ID int,
	Chapter_ID int,
	Section_ID int,
	Artical_ID int,
	Clause_ID int,
	Point_ID int
)

Create Table KeyPhrase (
	ID int Identity(1,1) Primary key,
	KeyPhrase nvarchar(255),
	KeyNorm nvarchar(255),
	Artical_ID int ,
	Concept_ID int,
	Artical_Number int
) 

Create Table KeyPhraseMapping(
	ID int Identity(1,1) Primary key,
	KeyPhrase_ID int,
	Law_ID int,
	Section_ID int,
	Chapter_ID int,
	Clause_ID int,
	Point_ID int,
	Weight float,
	NumCount int
)

Create Table Law (
	ID int Identity(1,1) Primary Key,
	Name nvarchar(255),
	DateActive DateTime,
	DateDeactive DateTime,
	Status int,
	PublicationTime DateTime,
	SignName nvarchar(255),
	Code nvarchar(100)
)

Create Table LawHTML (
	Law_ID int identity(1,1) Primary Key,
	ContextHTML nvarchar(max),
	ContentText nvarchar(max)
)

Create Table Point (
	ID int identity(1,1) Primary key,
	Clause_ID int,
	char char(3),
	Name nvarchar(100),
	Content nvarchar(Max),
	DateActive DateTime,
	DateDeactive DateTime,
	Status int
)

Create Table Query (
	ID int Identity(1,1) Primary Key,
	Question nvarchar(max),
	Answer_ID int,
	DateCreate DateTime,
	User_ID int
)

Create Table Answer (
	ID int Identity(1,1) Primary Key,
	Question_ID int,
	Artical_ID int
)

Create Table QuestionContain(
	ID int Identity(1,1) Primary Key,
	Contain nvarchar(255),
	QuestionType_ID int
)

Create Table QuestionType (
	ID int Identity(1,1) Primary Key,
	QuestionType nvarchar(100),
	Prefix nvarchar(100),
	AfterFix nvarchar(100),
	Contain nvarchar(100)
)

Create Table Settings (
	ID int Identity(1,1) Primary Key,
	Name varchar(500),
	Value varchar(500)
)

Create Table User_ (
	ID int Identity(1,1) Primary Key,
	Name nvarchar(250),
	PassWord_ID int,
	UserInfo_ID int,
	UserHistory_ID int,
	UserRole_ID int,
	CreateAtDate DateTime,
	LastLoginDate DateTime
)

Create Table Password (
	ID int Identity(1,1) Primary key,
	Password varchar(256),
	ChangedAtDate DateTime
)

Create Table UserInfo (
	ID int Identity(1,1) Primary key,
	Level int,
	Email nvarchar(256),
	PhoneNumber char(10),
	ActiveStatus int,
	ChangeAtDate DateTime
)

Create Table UserRole(
	ID int Identity(1,1) Primary Key,
	Name nvarchar(256),
)

Create Table Function_(
	ID int Identity(1,1) Primary Key,
	Name nvarchar(256),
	UniqueOperation_ID int,
	UserRole_ID int
)

Create Table OverwriteMapping(
	ID int Identity(1,1) Primary key,
	Chapter_ID_RegulateEditing int,
	Section_ID_RegulateEditing int,
	Artical_ID_RegulateEditing int,
	Clause_ID_RegulateEditing int,
	Point_ID_RegulateEditing int,
	Chapter_ID_IsEdited int,
	Section_ID_IsEdited int,
	Artical_ID_IsEdited int,
	Clause_ID_IsEdited int,
	Point_ID_IsEdited int,
	DateActive date
)
