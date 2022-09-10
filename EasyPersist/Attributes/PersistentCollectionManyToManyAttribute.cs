using System;
using EasyPersist.Core.Exceptions;

namespace EasyPersist.Core.Attributes {
    /// <summary>
    /// marks the collection which is many-to-many collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PersistentCollectionManyToManyAttribute : PersistentCollectionAttribute {
        /// <summary>
        /// An attribute to mark many-to-many collections
        /// (This class is parent collection contain children.)
        /// </summary>
        /// <param name="dbFieldName">LinkTable Parent Id Column Name</param>
        /// <param name="persistentType">Child types of the current one (will be added in the list)</param>
        /// <param name="persistentTypeLinkTable">Link TableName</param>
        /// <param name="persistentTypeLinkTableJoinFieldName">LinkTable Child Ids Column Name</param>
        public PersistentCollectionManyToManyAttribute(string dbFieldName,
            Type persistentType,
            string persistentTypeLinkTable,
            string persistentTypeLinkTableJoinFieldName) : base(dbFieldName, persistentType) {
            if (string.Compare(dbFieldName, persistentTypeLinkTableJoinFieldName, StringComparison.OrdinalIgnoreCase) == 0){
                throw new CommonEasyPersistException("Can't create PersistentCollectionManyToManyAttribute " +
                                                     "with same left and right side linkage key column." +
                                                     " (check the first and last parameters you pass into the constructor)");
            }
            LinkTable=persistentTypeLinkTable;
            LinkTableJoinFieldName=persistentTypeLinkTableJoinFieldName;
        }

        public string LinkTable { get; private set; }
        public string LinkTableJoinFieldName { get; private set; }

        public override string ToString() {
            return "Persistent Many-to-Many Collection";
        }
    }
}