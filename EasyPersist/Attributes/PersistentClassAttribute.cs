using System;

namespace EasyPersist.Core.Attributes {
    /// <summary>
    /// Persistent Class Attribute.
    /// Marks the class as persistent (can be saved/loaded into DB with DAO)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PersistentClassAttribute : Attribute {
        /// <summary>
        /// Name of the Table In DB
        /// </summary>
        public string DbTableName { get; private set; }
        
        /// <summary>
        /// Inherited (children) Types array
        /// </summary>
        public Type[] InheritedTypes { get; private set; }
        
        /// <summary>
        /// Creates the PersistentClassAttribute 
        /// </summary>
        /// <param name="dbTableName">Name of the table in database</param>
        /// <param name="inheritedTypes">Types inherit this class (persistent children types)</param>
        public PersistentClassAttribute(string dbTableName,  params Type[] inheritedTypes)
        {
            DbTableName = dbTableName;
            InheritedTypes = inheritedTypes;
        }

        public override String ToString() {
            return "Persistent Class";
        }
    }
}
