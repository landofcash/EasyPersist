# EasyPersist ORM Framework Guide
github page: [https://github.com/landofcash/EasyPersist](https://github.com/landofcash/EasyPersist)

---

## Quick info about mapping.

1. Class and properties are marked with an attribute to make them load/save in db.
2. Class must Implement an intrface with only 1 property -- `int Id{get;set;}`
That's it. No complex xml, mapping tools etc.

## Where and when to use this lib:

* This lib is good for prototype, small and medium size projects. 
* For projects where you want to use native SQL queries but also simplify save/update/load of data objects.
* For quickly patching unsupported bloated code projects.
----
# Contents
[Core Demo Classes Description](Core Demo Classes Description.md) 
[Console Demo Description](Console Demo Description.md) 
[Web Demo Description](Web Demo Description.md)

## Quick Start Guide

EasyPersist  ORM lib is used to simplify Loading and Saving objects in SQL Server. The lib uses native SQL to query db.
Core functionality includes:
* Save/Update object in DB
* Load objects from DB
* Load lists of objects from DB
* Delete object from DB

The mapping mechanism is very simple. Done via attributes (no complex xml etc.).

There are two ways to use it 
* Load/Save “Persistent” object in db.
* Load “Read-only” object from db.

## ReadOnly Classes
Used in situation when you need to quickly load custom data and don’t want to save it back in db.

So first of all you need a class to work with.

LocationHelper class is: 
```C#
using System.Data;
using Loc.HibernateMini.Attributes;

namespace EasyPersistDemo {
    public class TestLocation {
        private int _id;
        private string _city;
        private string _county;
        private string _state;
        
        [PersistentProperty("LocCityId", DbType.Int32)]
        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("CityName", DbType.String)]
        public string City {
            get { return _city; }
            set { _city = value; }
        }
        [PersistentProperty("CountyName", DbType.String)]
        public string County {
            get { return _county; }
            set { _county = value; }
        }
        [PersistentProperty("StateName", DbType.String)]
        public string State {
            get { return _state; }
            set { _state = value; }
        }
        public override string ToString()
        {
            return City + ", " + County + ", " + State;
        }
        public string CityCaps {
            get { return _city.ToUpper(); }
        }
    }
}
```

The lib will take this TestLocation, loop thru properties marked with PersistentProperty attribute and set their values from the dataset. Note that CityCaps property is not marked with PersistentProperty Attribute. It encapsulates some business logic and should not be bind/loaded with data. (Attribute first param is the column name in sql query)
 
Ok now you can load data from db
```C#
    string locationSql = @"
    SELECT City.CityId as LocCityId, City.Name as CityName, County.Name as CountyName, State.Name as StateName
    FROM City 
    INNER JOIN County ON City.CountyId=County.CountyId
    INNER JOIN State ON County.StateId=State.StateId
    ORDER BY City.Name, County.Name, State.Name
    ";
    ArrayList locations = program.Dao.GetReadOnlyListFromDb(new SqlCommand(locationSql), typeof(TestLocation));
    foreach (TestLocation location in locations)
    {
        Console.WriteLine(location.ToString());
    }
```

Very easy. All you need is a sql query (column names should be the same as defined in TestLocation property attributes). And call GetReadOnlyListFromDb method:

```
_dao.GetReadOnlyListFromDb(sqlCommand, typeof(TestLocation));
```

It returns a list of objects of type TestLocation.

## “Persistent” classes
Persistent is the the class which is reflected by db table and could be loaded and saved in db.
“Persistent” classes are the core of your application and the data layer. Persistent class is marked with PersistentClass Attribute and implements IPersistent interface. 
e.g.:

```C#
[PersistentClass("City")]
 public class City: IPersistent {
```

There should be an Id property required by interface

```C#
[PersistentProperty("CityId", DbType.Int32)]
public int Id {
   get { return _id; }
   set { _id = value; }
}
```

All properties which should be saved in db are marked with PersistentProperty attribute

```C#
using System;
using System.Data;
using Loc.HibernateMini.Attributes;
using Loc.HibernateMini.IFaces;

namespace EasyPersistDemo.Core {
    [PersistentClass("City")]
    public class City : IPersistent {
        //ID of the city (required to identify a record in db. usually it is identity column)
        private int _id;
        //name of the city (just a textual property)
        private string _name;
        // nullable DateTime property
        private DateTime? _changeDate; 
        //boolean property
        private bool _isActive; 
        //Enum property (maps to int in db)
        private SettlementType _type; 
        //Persistent object (many-to-one)
        private County _county; 
        
        [PersistentProperty("CityId", DbType.Int32)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("Name", DbType.String)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [PersistentProperty("ChangeDate", DbType.DateTime)]
        public DateTime? ChangeDate
        {
            get { return _changeDate; }
            set { _changeDate = value; }
        }
        [PersistentProperty("IsActive", DbType.Boolean)]
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        [PersistentProperty("CountyId")]
        public County County
        {
            get { return _county; }
            set { _county = value; }
        }
        [PersistentProperty("SettlementType", DbType.Int32)]
        public SettlementType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
    public enum SettlementType
    {
        Hamlet,Village,Town,City,Megalopolis 
    }
}
```

How to save a new City:
```
   City city = new City();
                    city.Name = "Jacksonville";
                    city.ChangeDate = DateTime.Now;
                    city.IsActive = true;
                    city.Type = SettlementType.City;
                    city.County = county;
                    _dao.SaveOrUpdate(city);
                    Console.WriteLine("Saved City " + city.Name + " ID:" + city.Id);
```
All you need is to create the new instance of City, set properties and call _dao.SaveOrUpdate(City);

The lib will look an Id (via IPersistent interface) and if it is 0 a new City will be created (INSERT stetement) else the lib will generate an UPDATE sql and the row will be updated.
e.g.:

```SQL
INSERT INTO [City](City) ( [Name](Name), [ChangeDate](ChangeDate), [IsActive](IsActive), [CountyId](CountyId), [SettlementType](SettlementType) ) 
VALUES ( @Name, @ChangeDate, @IsActive, @CountyId, @SettlementType ); select scope_identity()
```


The lib prints all db calls in Debug out so you can see all db calls
e.g.:

```
INSERT INTO [State](State) ( [Name](Name) ) VALUES ( @Name ); select scope_identity()
INSERT INTO [State](State) ( [Name](Name) ) VALUES ( @Name ); select scope_identity()
UPDATE [State](State)(State) SET [Name](Name)=@Name WHERE [State](State)(State).[StateId](StateId)=2
Loading List SQL:SELECT top 1 * FROM State WHERE State.Name = @name
INSERT INTO [County](County) ( [Name](Name), [StateId](StateId) ) VALUES ( @Name, @StateId ); select scope_identity()
INSERT INTO [County](County) ( [Name](Name), [StateId](StateId) ) VALUES ( @Name, @StateId ); select scope_identity()
Loading List SQL:SELECT * FROM County
```

Note that the lib caches objects.


Use CsUnit to run tests
http://www.csUnit.org
And unitRun plug-in may be also helpful
http://www.jetbrains.com/unitrun/

Thanks.
