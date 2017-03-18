using System.Data;
using EasyPersist.Core;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Tests.Structure
{
    [PersistentClass(TempPersistentBase.DB_TABLE,
        typeof(TempPersistent),
        typeof(AnotherTempPersistent), 
        typeof(ThirdLevelTempPersistent))]
    abstract class  TempPersistentBase : IPersistent
    {
        private const string DB_TABLE = "TempPersistentBase";
        private int _id;
        private string _name;
        private TempPersistentAlone _tempPersistentAlone;
            
        [PersistentProperty("TempPersistentBaseId", DbType.Int32)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("Name", SortOrder.DESC)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [PersistentProperty("TempPersistentAloneId")]
        public TempPersistentAlone TempPersistentAlone
        {
            get { return _tempPersistentAlone; }
            set { _tempPersistentAlone = value; }
        }

        public virtual string GetDbTable()
        {
            return DB_TABLE;
        }
    }
}