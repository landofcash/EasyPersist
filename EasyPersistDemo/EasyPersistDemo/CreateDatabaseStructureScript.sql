SET QUOTED_IDENTIFIER ON

go


/* Create new table "dbo"."City".                                                             */
/* "dbo"."City" : Table of City                                                               */
/* 	"CityId" : CityId identifies City                                                         */
/* 	"Name" : CityName is of City                                                              */
/* 	"ChangeDate" : ChangeDate is of City                                                      */
/* 	"IsActive" : IsActive is of City                                                          */
/* 	"SettlementType" : SettlementType is of City                                              */
/* 	"CountyId" : CountyId is of City                                                          */  
create table "dbo"."City" ( 
	"CityId" int identity not null,
	"Name" nvarchar(255) not null,
	"ChangeDate" datetime null,

	"IsActive" bit default 0 not null,
	"SettlementType" int not null,
	"CountyId" int not null) ON 'PRIMARY'  

go

alter table "dbo"."City"
	add constraint "City_PK" primary key clustered ("CityId")   


go

/* Create new table "dbo"."County".                                                           */
/* "dbo"."County" : Table of County                                                           */
/* 	"CountyId" : CountyId identifies County                                                   */
/* 	"Name" : CountyName is of County                                                          */
/* 	"StateId" : StateId is of County                                                          */  
create table "dbo"."County" ( 
	"CountyId" int identity not null,
	"Name" nvarchar(255) not null,
	"StateId" int not null) ON 'PRIMARY'  

go

alter table "dbo"."County"
	add constraint "Country_PK" primary key clustered ("CountyId")   


go

/* Create new table "dbo"."State".                                                            */
/* "dbo"."State" : Table of State                                                             */
/* 	"StateId" : StateId identifies State                                                      */
/* 	"Name" : StateName is of State                                                            */  
create table "dbo"."State" ( 
	"StateId" int identity not null,
	"Name" nvarchar(255) not null) ON 'PRIMARY'  

go

alter table "dbo"."State"
	add constraint "State_PK" primary key clustered ("StateId")   


go

/* Add foreign key constraints to table "dbo"."City".                                         */
alter table "dbo"."City"
	add constraint "County_City_FK1" foreign key (
		"CountyId")
	 references "dbo"."County" (
		"CountyId") on update no action on delete no action  

go

/* Add foreign key constraints to table "dbo"."County".                                       */
alter table "dbo"."County"
	add constraint "State_County_FK1" foreign key (
		"StateId")
	 references "dbo"."State" (
		"StateId") on update no action on delete no action  

go


/* This is the end of the Microsoft Visual Studio generated SQL DDL script.                   */
