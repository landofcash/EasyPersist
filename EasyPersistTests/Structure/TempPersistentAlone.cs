using System;
using System.Collections;
using System.Data;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Tests.Structure
{
    [PersistentClass(DB_TABLE)]
    class TempPersistentAlone : IPersistent {
        private const string DB_TABLE = "TempPersistentAlone";
        private int _id;
        private string _name;
        private int? _intNullable;
        private ArrayList _tempPersistentBaseArrayList;
        private DateTime _dateTime;
        private string _nameWithSpace;

        [PersistentProperty("TempPersistentAloneId", DbType.Int32)]
        public int Id {
            get => _id;
            set => _id = value;
        }
        [PersistentProperty("Name")]
        public string Name {
            get => _name;
            set => _name = value;
        }

        [PersistentProperty("Name With Space")]
        public string NameWithSpace
        {
            get => _nameWithSpace;
            set => _nameWithSpace = value;
        }

        [PersistentProperty("IntNullable", DbType.Int32)]
        public int? IntNullable
        {
            get => _intNullable;
            set => _intNullable = value;
        }
        [PersistentProperty("Date", DbType.DateTime)]
        public DateTime DateTime
        {
            get => _dateTime;
            set => _dateTime = value;
        }
        [PersistentCollection("TempPersistentAloneId", typeof(TempPersistentBase))]
        public ArrayList TempPersistentBaseArrayList
        {
            get => _tempPersistentBaseArrayList;
            set => _tempPersistentBaseArrayList = value;
        }

        public virtual string GetDbTable() {
            return DB_TABLE;
        }
    }
}