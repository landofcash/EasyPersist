using EasyPersist.Core.Attributes;

namespace EasyPersist.Tests.Structure
{
    [PersistentClass(DB_TABLE,typeof(ThirdLevelTempPersistent))]
    class TempPersistent:TempPersistentBase
    {
        private const string DB_TABLE = "TempPersistent";
        private string _caption;
        
        public override string GetDbTable() {
            return DB_TABLE;
        }
        [PersistentProperty("Caption")]
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
    }
}