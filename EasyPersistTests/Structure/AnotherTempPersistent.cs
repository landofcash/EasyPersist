using EasyPersist.Core.Attributes;

namespace EasyPersist.Tests.Structure
{
    [PersistentClass(DB_TABLE)]
    class AnotherTempPersistent : TempPersistentBase {
        private const string DB_TABLE = "AnotherTempPersistent";
        private string _description;

        public override string GetDbTable() {
            return DB_TABLE;
        }
        [PersistentProperty("Description")]
        public string Description {
            get { return _description; }
            set { _description = value; }
        }
    }
}