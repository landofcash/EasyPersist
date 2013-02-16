using System;
using System.Data;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Demo.Core {
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
