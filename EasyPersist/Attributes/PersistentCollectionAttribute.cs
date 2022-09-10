using System;

namespace EasyPersist.Core.Attributes {
    /// <summary>
    /// Marks a persistent collection of child.
    /// Many-to-one collection 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PersistentCollectionAttribute : PersistentPropertyAttribute {
        /// <summary>
        /// An attribute to mark collections of persistent objects.
        /// MUST BE applied to the ArrayList
        /// </summary>
        /// <param name="dbFieldName">FK(Foreign Key) Column name</param>
        /// <param name="persistentType">Typeof child items in the collection</param>
        public PersistentCollectionAttribute(string dbFieldName, Type persistentType) : base(dbFieldName) {
            PersistentType = persistentType;
        }

        /// <summary>
        /// Type of the child items in collection
        /// </summary>
        public Type PersistentType { get; private set; }

        public override string ToString() {
            return "Persistent Collection";
        }

    }
}
