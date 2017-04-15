using System;
using System.Data;
using System.Reflection;
using EasyPersist.Core.Exceptions;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Core.Attributes
{
    /// <summary>
    /// Marks the property which is mapped in to a column in database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistentPropertyAttribute : Attribute
    {
        /// <summary>
        /// Marks the field which can be saved/loaded from database.
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="dbType">
        /// DbType of the column. 
        /// No needed for strings, persistent objects, and types with new(string) constructor
        /// </param>
        ///  <param name="lazy">
        /// Default false. If true the value is not included in the sql request. 
        /// Usfull for columns with big data inside. 
        /// </param>
        /// <param name="order">SortOrder when an object is loaded in child collections</param>
        [Obsolete("Use PersistentPropertyAttribute(string,DbType, SortOrder,bool) method instead")]
        public PersistentPropertyAttribute(string dbFieldName, DbType dbType, string order, bool lazy) : this(dbFieldName, dbType, order)
        {
            Lazy = lazy;
        }


        /// <summary>
        /// Marks the field which can be saved/loaded from database.
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="dbType">
        /// DbType of the column. 
        /// No needed for strings, persistent objects, and types with new(string) constructor
        /// </param>
        /// <param name="order">SortOrder when an object is loaded in child collections</param>
        public PersistentPropertyAttribute(string dbFieldName, DbType dbType, SortOrder order, DBValueConverterBase converter = null) : this(dbFieldName, dbType)
        {
            SetOrder(order);
        }

        /// <summary>
        /// Marks the field which can be saved/loaded from database.
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="dbType">
        /// DbType of the column. 
        /// No needed for strings, persistent objects, and types with new(string) constructor
        /// </param>
        /// <param name="order">SortOrder when an object is loaded in child collections</param>
        [Obsolete("Use PersistentPropertyAttribute(string,DbType, SortOrder) method instead")]
        public PersistentPropertyAttribute(string dbFieldName, DbType dbType, string order) : this(dbFieldName, dbType)
        {
            Order = order;
        }


        /// <summary>
        /// Marks the field which can be saved/loaded from database. 
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="order">SortOrder when an object is loaded in child collections</param>
        public PersistentPropertyAttribute(string dbFieldName, SortOrder order) : this(dbFieldName)
        {
            SetOrder(order);
        }

        /// <summary>
        /// SortOrder Enum to SQL string order
        /// </summary>
        /// <param name="order"></param>
        private void SetOrder(SortOrder order)
        {
            if (order == SortOrder.ASC)
            {
                Order = "ASC";
            }
            if (order == SortOrder.DESC)
            {
                Order = "DESC";
            }
        }

        [Obsolete("Use PersistentPropertyAttribute(string, SortOrder) method instead")]
        /// <summary>
        /// Marks the field which can be saved/loaded from database. 
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="order">string ASC or DESC</param>
        public PersistentPropertyAttribute(string dbFieldName, string order)
            : this(dbFieldName)
        {
            Order = order;
        }
        /// <summary>
        /// Marks the field which can be saved/loaded from database. 
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="dbType">
        /// DbType of the column. 
        /// No needed for strings, persistent objects, and types with new(string) constructor
        /// </param>
        /// <param name="lazy">
        /// Default false. If true the value is not included in the sql request. 
        /// Usfull for columns with big data inside. 
        /// </param>
        public PersistentPropertyAttribute(string dbFieldName, DbType dbType, bool lazy) : this(dbFieldName, dbType)
        {
            Lazy = lazy;
        }
        /// <summary>
        /// Marks the field which can be saved/loaded from database. 
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="dbType">
        /// DbType of the column. 
        /// No needed for strings, persistent objects, and types with new(string) constructor
        /// </param>
        public PersistentPropertyAttribute(string dbFieldName, DbType dbType) : this(dbFieldName)
        {
            DbType = dbType;
        }
        /// <summary>
        /// Marks the field which can be saved/loaded from database. 
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="lazy">
        /// Default false. If true the value is not included in the sql request. 
        /// Usfull for columns with big data inside. 
        /// </param>
        public PersistentPropertyAttribute(string dbFieldName, bool lazy) : this(dbFieldName)
        {
            Lazy = lazy;
        }


        /// <summary>
        /// Marks the field which can be saved/loaded from database. 
        /// </summary>
        /// <param name="dbFieldName">Column name in the database</param>
        /// <param name="lazy">
        /// Default false. If true the value is not included in the sql request. 
        /// Usfull for columns with big data inside. 
        /// </param>
        /// <param name="dbType">
        /// DbType of the column. 
        /// No needed for strings, persistent objects, and types with new(string) constructor
        /// </param>
        public PersistentPropertyAttribute(string dbFieldName, bool lazy = false, DbType dbType = DbType.AnsiString, Type converter = null)
        {
            Lazy = lazy;
            DbType = dbType;
            if (converter != null)
            {
                
                ConstructorInfo converterConstructor = converter.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    Type.EmptyTypes,
                    null
                );
                if (converterConstructor == null)
                {
                    throw new CommonEasyPersistException($"Can't find default constructor in converter class {converter.FullName}");
                }
                Converter = (DBValueConverterBase)converterConstructor.Invoke(new Object[0]);
            }
            DbFieldName = dbFieldName;
        }

        public string Order { get; private set; }

        public string DbFieldName { get; private set; }

        public DbType DbType { get; private set; }

        public bool Lazy { get; private set; }

        public override String ToString()
        {
            return "Persistent Property";
        }
        public DBValueConverterBase Converter { get; private set; }
    }
}
