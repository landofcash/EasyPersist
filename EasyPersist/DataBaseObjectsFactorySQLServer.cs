namespace EasyPersist.Core {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using EasyPersist.Core.Attributes;
    using EasyPersist.Core.Cache;
    using EasyPersist.Core.Exceptions;
    using EasyPersist.Core.IFaces;
    using Microsoft.Extensions.Logging;

    public class DataBaseObjectsFactorySQLServer : DataBaseObjectsFactorySQLCore
    {
        public ILogger<DataBaseObjectsFactorySQLServer> LOGGER { get; set; } = new EmptyLogger<DataBaseObjectsFactorySQLServer>();
        private static readonly Regex alfaNumOnlyRegex = new Regex("[^a-zA-Z0-9]");

        private readonly ICache _cache = new SimpleCache();

        public SqlTransaction SqlTransaction { get; set; }

        public ICache Cache
        {
            get { return _cache; }
        }
        public DataBaseObjectsFactorySQLServer()
        {
            
        }

        public DataBaseObjectsFactorySQLServer(string sqlConnectionString) {
            SqlConnectionString = sqlConnectionString;
        }
        public DataBaseObjectsFactorySQLServer(string sqlConnectionString, ICache cache) {
            SqlConnectionString = sqlConnectionString;
            _cache = cache;
        }

        /// <summary>
        /// Loads object from database by ID
        /// </summary>
        /// <typeparam name="T">type of the IPersistent to load</typeparam>
        /// <param name="id">ID of an object</param>
        /// <returns>Object loaded from DB</returns>
        public virtual T getFromDb<T>(int id) where T:IPersistent
        {
           return (T)getFromDb(id, typeof(T));
        }

        /// <summary>
        ///  Loads object from database by ID
        /// </summary>
        /// 
        /// <param name="id">ID of an object</param>
        /// <param name="persistent">object to load</param>
        public override void getFromDb(int id, ref IPersistent persistent) {
            getFromDb(id, ref persistent, true);
        }
        
        /// <summary>
        ///  Loads object from database by ID
        /// </summary>
        /// <param name="id">ID of an object</param>
        /// <param name="persistent">object to load</param>
        /// <param name="lazy">lazy or not (shows if need to load lazy properties)</param>
        public override void getFromDb(int id, ref IPersistent persistent, bool lazy) {
            IList<IPersistent> alreadyLoaded = new List<IPersistent>();
            getFromDb(id, ref persistent, alreadyLoaded, lazy);
        }

        private SqlCommand GetSqlCommand(string cmdText)
        {
            SqlConnection sqlConnection;
            SqlCommand cmd = new SqlCommand(cmdText);
            if (SqlTransaction != null)
            {
                cmd.Transaction = SqlTransaction;
                sqlConnection = SqlTransaction.Connection;
            }
            else
            {
                sqlConnection = new SqlConnection(SqlConnectionString);
            }
            cmd.Connection = sqlConnection;
            cmd.CommandTimeout = CommandTimeout;
            return cmd;
        }

        public override void getFromDb(int id, ref IPersistent persistent, IList<IPersistent> alreadyLoaded, bool lazy)
        {
            Type persistentType = persistent.GetType();
            if (lazy)
            {
                IPersistent cachedItem= GetFromCache(id, persistentType, alreadyLoaded);
                if (cachedItem != null)
                {
                    persistent = cachedItem;
                    return; //item from cache returned
                }
            }
            String selectOfferString = PrepareSelectSql(persistentType, lazy);
            SqlCommand cmd = GetSqlCommand(selectOfferString);
            cmd.Parameters.AddWithValue("id", id);
            //
            LOGGER.Log(LogLevel.Information, String.Format("Getting an object with id:{0} SQL:{1}", id, selectOfferString));
            DataTable table = FillTableFromDatabase(cmd);
            if (table.Rows.Count > 0) {
                createObject(table.Rows[0], persistent, alreadyLoaded, lazy);
                if (lazy) {
                    _cache.Put(persistent); //������ � ���
                }
            } else {
                persistent = null;
            }
        }

        /// <summary>
        /// Checks Global and local caches for an object of specified Type and Id.
        /// </summary>
        /// <param name="id">Id of an object</param>
        /// <param name="persistentType">Object Type</param>
        /// <param name="localCache">Local Cache</param>
        /// <returns>an object from cache or null if not found</returns>
        private IPersistent GetFromCache(int id, Type persistentType, IList<IPersistent> localCache)
        {
            //TODO: change local cache to a dicionary key: table name
            //Check Global Cache
            IPersistent cachedPersistent = _cache.Get(id, persistentType);
            if (cachedPersistent != null)
            {
                return cachedPersistent;
            }
            //Check Local cache, to prevent duplicate load of an item
            if (localCache != null && localCache.Count > 0)
            {
                string persistentTypeTableName = GetTableName(persistentType);
                return localCache.FirstOrDefault(pObj => pObj.Id == id && GetTableName(pObj.GetType()) == persistentTypeTableName);
            }
            return null;
        }

        public virtual IPersistent getFromDb(int id, Type objectType, bool lazy = true) {
            //trying to use default constructor
            ConstructorInfo persistentConstructor = objectType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                Type.EmptyTypes,
                null
                );
            if (persistentConstructor == null) {
                throw new CommonEasyPersistException("Can't find default constructor in persistent class " + objectType.FullName);
            }
            Object[] parameters = new Object[0];
            Object p = persistentConstructor.Invoke(parameters);
            //creating object of type IPersistent using default constructor
            IPersistent persistent = (IPersistent)p;
            getFromDb(id, ref persistent,lazy);
            return persistent;
        }

        /// <summary>
        /// Loads a single object from db, or null if no objects found.Throws an Exception if more than 1 record is returned
        /// </summary>
        /// <param name="sql">an sql which returns a single row result</param>
        /// <param name="objectType">Type of object to return</param>
        /// <returns>an IPersistent ogect of type from param</returns>
        public virtual IPersistent getFromDb(string sql, Type objectType) {
            IList<IPersistent> objectsList = getListFromDb(sql, objectType);
            if (objectsList.Count > 1) {
                throw new CommonEasyPersistException("Too many objects loaded. Rows count:" + objectsList.Count);
            }
            if (objectsList.Count == 0) {
                return null;
            }
            return objectsList[0];
        }
        /// <summary>
        /// Loads a single object from db 
        /// </summary>
        /// <param name="command">an sql which returns a single row result</param>
        /// <param name="type">Type of object to return</param>
        /// <returns>an IPersistent ogect of type from param</returns>
        public virtual IPersistent getFromDb(SqlCommand command, Type type) {
            IList<IPersistent> objectsList = getListFromDb(command, type);
            if (objectsList.Count > 1) {
                throw new CommonEasyPersistException("Too many objects loaded. Rows count:" + objectsList.Count);
            }
            if (objectsList.Count == 0) {
                return null;
            }
            return objectsList[0];
        }

        private IList<IPersistent> getListFromDb(DataTable table, Type collectionObjectsClass) {
            return getListFromDb(table, collectionObjectsClass, new List<IPersistent>());
        }
        /// <summary>
        ///		creates object of the specified type from the DataTable
        /// </summary>
        /// <param name="table">table with data</param>
        /// <param name="collectionObjectsClass">object types to create</param>
        /// <returns>IList of objects</returns>    
        private IList<IPersistent> getListFromDb(DataTable table, Type collectionObjectsClass, IList<IPersistent> alreadyLoaded) {
            IList<IPersistent> list = new List<IPersistent>();
            foreach (DataRow row in table.Rows) {
                //if passed type that doesn't implement IPersistent
                if (collectionObjectsClass.IsAssignableFrom(typeof(IPersistent))) {
                    throw new CommonEasyPersistException("Persistent collection class " + collectionObjectsClass.FullName
                        + " must implement Persistent interface");
                }
                //trying to use default constructor
                ConstructorInfo persistentConstructor = collectionObjectsClass.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    Type.EmptyTypes,
                    null
                    );
                if (persistentConstructor == null) {
                    throw new CommonEasyPersistException("Can't find default constructor in persistent class " + collectionObjectsClass.FullName);
                }
                Object[] parameters = new Object[0];
                Object p = persistentConstructor.Invoke(parameters);
                //creating object of the type IPersistent using default constructor
                IPersistent persistent = (IPersistent)p;
                //create object from the data raw
                createObject(row, persistent, alreadyLoaded, true);
                list.Add(persistent);
                _cache.Put(persistent);
            }
            return list;
        }

        /// <summary>
        /// Loads object collection from the database
        /// </summary>
        /// <param name="pca">Collection attribute</param>
        /// <param name="parent">Parent object</param>
        /// <param name="collectionObjectsClass">Collection object type</param>
        /// <returns>Collection of IPersistent objects from DB</returns>
        public override IList<IPersistent> getListFromDb(PersistentCollectionAttribute pca
            , ref IPersistent parent, Type collectionObjectsClass) {
            SqlConnection sqlConnection;
            if(SqlTransaction!=null){
                sqlConnection = SqlTransaction.Connection;
            } else
            {
                sqlConnection = new SqlConnection(SqlConnectionString);
            }
            Type baseCollectionElementType = pca.PersistentType;
            List<IPersistent> result = new List<IPersistent>();
            //get PersistentClassAttribute
            object[] classAttributes = baseCollectionElementType.GetCustomAttributes(typeof(PersistentClassAttribute), false);
            if (classAttributes.Length > 1) {
                throw new CommonEasyPersistException("Too many PersistentClassAttributes for class:"
                                                       + baseCollectionElementType.Name);
            }
            if (classAttributes.Length == 1) {
                PersistentClassAttribute persistentClassAttribute = (PersistentClassAttribute)classAttributes[0];
                Type[] inherTypes = new Type[persistentClassAttribute.InheritedTypes.Length + 1];
                persistentClassAttribute.InheritedTypes.CopyTo(inherTypes, 0);
                inherTypes[persistentClassAttribute.InheritedTypes.Length] = collectionObjectsClass;
                foreach (Type type in inherTypes) {
                    if (!type.IsAbstract && !type.IsInterface) {
                        string selectOfferString = prepareSelectQueryForChildCollections(pca, parent, type, false);
                        LOGGER.Log(LogLevel.Information, $"Loading List SQL:{selectOfferString}");
                        using (SqlCommand cmd = new SqlCommand(selectOfferString, sqlConnection)) {
                            DataTable table = FillTableFromDatabase(cmd);
                            result.AddRange(getListFromDb(table, type));
                        }
                    }
                }
            } else {
                throw new CommonEasyPersistException("No PersistentClassAttribute for class:" + baseCollectionElementType.Name);
            }
            return result;
        }

        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <param name="cmd">SqlCommand db query</param>
        /// <param name="collectionObjectsClass">Objects that will be restored from db records</param>
        /// <returns> IList of collectionObjectsClass Objects</returns>
        public virtual IList<IPersistent> getListFromDb(SqlCommand cmd, Type collectionObjectsClass) {
            LOGGER.Log(LogLevel.Information, $"Loading List SQL:{cmd.CommandText}");
            DataTable table = FillTableFromDatabase(cmd);
            return getListFromDb(table, collectionObjectsClass);
        }

        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <typeparam name="T">Typeof IPersistent object to return </typeparam>
        /// <param name="cmd">SQL Command</param>
        /// <returns>List of objects</returns>
        public virtual List<T> getListFromDb<T>(SqlCommand cmd) where T : IPersistent
        {
            IList<IPersistent> items = getListFromDb(cmd, typeof(T));
            List<T> result = items.Cast<T>().ToList();
            return result;
        }

        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <param name="cmd">SqlCommand ����� � ����</param>
        /// <param name="collectionObjectsClass">Objects that will be restored from db records</param>
        /// <param name="alreadyLoaded">Already Loaded objects</param>
        /// <returns> IList of collectionObjectsClass Objects</returns>
        public virtual IList<IPersistent> getListFromDb(SqlCommand cmd, Type collectionObjectsClass, IList<IPersistent> alreadyLoaded) {
            LOGGER.Log(LogLevel.Information, $"Loading List Loaded objects:{alreadyLoaded.Count} SQL:{cmd.CommandText}");
            DataTable table = FillTableFromDatabase(cmd);
            return getListFromDb(table, collectionObjectsClass, alreadyLoaded);
        }
        /// <summary>
        /// Applies the current transaction
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private SqlConnection ApplayTransaction(SqlCommand cmd)
        {
            SqlConnection sqlConnection;
            if (SqlTransaction != null) {
                sqlConnection = SqlTransaction.Connection;
            } else {
                sqlConnection = new SqlConnection(SqlConnectionString);
            }
            cmd.Connection = sqlConnection;
            cmd.Transaction = SqlTransaction;
            return sqlConnection;
        }

        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <typeparam name="T">Typeof IPersistent object to return </typeparam>
        /// <param name="cmd">SQL Command</param>
        /// <returns>List of objects</returns>
        public virtual List<T> getListFromDb<T>(string sql) where T : IPersistent
        {
            IList<IPersistent> items = getListFromDb(sql, typeof (T));
            List<T> result = items.Cast<T>().ToList();
            return result;
        }

        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <param name="sql">Sql query to db�</param>
        /// <param name="collectionObjectsClass">Objects that will be restored from db records</param>
        /// <returns> IList of collectionObjectsClass Objects</returns>
        public virtual IList<IPersistent> getListFromDb(string sql, Type collectionObjectsClass) {
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandTimeout = CommandTimeout;
            return getListFromDb(cmd, collectionObjectsClass);
        }
        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <param name="sql">Sql ����� � ����</param>
        /// <param name="collectionObjectsClass">Objects that will be restored from db records</param>
        /// <param name="alreadyLoaded">Already Loaded objects</param>
        /// <returns> IList of collectionObjectsClass Objects</returns>
        public virtual IList<IPersistent> getListFromDb(String sql, Type collectionObjectsClass, IList<IPersistent> alreadyLoaded) {
            LOGGER.Log(LogLevel.Information, $"Loading List Loaded objects:{alreadyLoaded.Count} SQL:{sql}");
            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandTimeout = CommandTimeout;
            return getListFromDb(cmd, collectionObjectsClass, alreadyLoaded);
        }
        public virtual List<T> GetReadOnlyListFromDb<T>(SqlCommand cmd)
        {
            LOGGER.Log(LogLevel.Information, $"Loading Read Only List SQL:{cmd.CommandText}");
            ApplayTransaction(cmd);
            DataTable table = FillTableFromDatabase(cmd);
            return GetReadOnlyListFromDb<T>(table);
        }
        public virtual List<T> GetReadOnlyListFromDb<T>(DataTable table)
        {
            List<T> list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                //trying to use default constructor
                ConstructorInfo persistentConstructor = typeof(T).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    Type.EmptyTypes,
                    null
                );
                if (persistentConstructor == null)
                {
                    throw new CommonEasyPersistException("Can't find default constructor in class " + typeof(T).FullName);
                }
                object[] parameters = new object[0];
                T obj = (T)persistentConstructor.Invoke(parameters);
                CreateReadOnlyObject(row, obj);
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="collectionObjectsClass"></param>
        /// <returns></returns>
        [Obsolete("Use Generic GetReadOnlyListFromDb")]
        public virtual ArrayList GetReadOnlyListFromDb(SqlCommand cmd, Type collectionObjectsClass) {
            LOGGER.Log(LogLevel.Information, $"Loading Read Only List SQL:{cmd.CommandText}");
            ApplayTransaction(cmd);
            DataTable table = FillTableFromDatabase(cmd);
            return GetReadOnlyListFromDb(table, collectionObjectsClass);
        }


        /// <summary>
        /// Gets a collection of objects from db 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="collectionObjectsClass"></param>
        /// <returns></returns>
        [Obsolete("Use Generic GetReadOnlyListFromDb")]
        public virtual ArrayList GetReadOnlyListFromDb(DataTable table, Type collectionObjectsClass) {
            ArrayList list = new ArrayList();
            foreach (DataRow row in table.Rows) {
                ConstructorInfo persistentConstructor = collectionObjectsClass.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    Type.EmptyTypes,
                    null
                    );
                if (persistentConstructor == null) {
                    throw new CommonEasyPersistException("Can't find default constructor in class " + collectionObjectsClass.FullName);
                }
                var parameters = new object[0];
                object obj = persistentConstructor.Invoke(parameters);
                CreateReadOnlyObject(row, obj);
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Counts the number of elements in collection
        /// </summary>
        /// <param name="pca">Collection attribute</param>
        /// <param name="parent">owner of the collection</param>
        /// <returns>number of elements in collection</returns>
        public override int CountCollectionItems(PersistentCollectionAttribute pca, IPersistent parent) {
            String selectOfferString = prepareSelectQueryForChildCollections(pca, parent, pca.PersistentType, true);
            int res;
            SqlConnection sqlConnection;
            using (SqlCommand cmd = new SqlCommand(selectOfferString))
            {
                //if global transaction is not null we use it
                if (SqlTransaction != null) {
                    sqlConnection = SqlTransaction.Connection;
                    cmd.CommandTimeout = CommandTimeout;
                    cmd.Connection = sqlConnection;
                    cmd.Transaction = SqlTransaction;
                    res = (int)cmd.ExecuteScalar();
                } else {
                    using (sqlConnection = new SqlConnection(SqlConnectionString)) {
                        if (sqlConnection.State != ConnectionState.Open) {
                            sqlConnection.Open();
                        }
                        cmd.CommandTimeout = CommandTimeout;
                        cmd.Connection = sqlConnection;
                        res = (int)cmd.ExecuteScalar();
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Saves  persistent object in a transaction
        /// 
        /// </summary>
        /// <param name="persistent">an object to save or update</param>
        /// <param name="transaction">a transaction to use</param>
        public virtual void SaveOrUpdate(IPersistent persistent, SqlTransaction transaction) {
            bool update = false;
            if (persistent == null) {
                throw new CommonEasyPersistException("Can't save null in db");
            }
            //Update if ID exists
            if (persistent.Id != 0) {
                update = true;
            }
            if (!update && persistent is ILazy && !((ILazy)persistent).Initialized) {
                throw new LazyAccessException("Can't save an object which is lazy and not initialized.");
            }
            ///Parent types stack
            Stack<Type> stack = new Stack<Type>();
            Type type = persistent.GetType();
            while (type != null && type != typeof(object) && typeof(IPersistent).IsAssignableFrom(type)) {
                stack.Push(type);
                type = type.BaseType;
            }

            foreach (Type typeFromStack in stack) {
                SqlCommand updateCommand = GetUpdateCommandWithParameters(typeFromStack, persistent, transaction);
                //TODO probably move into loop above
                //Creating SQL queries
                //For insert
                string updateCommandText;
                if (!update) {
                    StringBuilder dbNames = new StringBuilder("");
                    StringBuilder dbValues = new StringBuilder("");
                    for (int i = 0; i < updateCommand.Parameters.Count; i++) {
                        dbNames.Append("[").Append(updateCommand.Parameters[i].SourceColumn).Append("]");
                        dbValues.Append("@").Append(updateCommand.Parameters[i].ParameterName);
                        if (i + 1 < updateCommand.Parameters.Count) {
                            dbNames.Append(", ");
                            dbValues.Append(", ");
                        }
                    }
                    updateCommandText = "INSERT INTO " + GetTableName(typeFromStack)
                        + " ( " + dbNames + " ) VALUES ( " + dbValues + " ); select scope_identity()";
                } else {
                    //for update
                    StringBuilder dbNamesValues = new StringBuilder("");
                    for (int i = 0; i < updateCommand.Parameters.Count; i++) {
                        dbNamesValues.Append("[").Append(updateCommand.Parameters[i].SourceColumn)
                        .Append("]=@").Append(updateCommand.Parameters[i].ParameterName);
                        if (i + 1 < updateCommand.Parameters.Count) {
                            dbNamesValues.Append(", ");
                        }
                    }
                    updateCommandText = "UPDATE " + GetTableName(typeFromStack) + " SET " + dbNamesValues
                        + " WHERE " + GetTableName(typeFromStack) + "." + IdColumnName(persistent) + "=" + persistent.Id;
                }
                //doing db query
                updateCommand.CommandText = updateCommandText;
                try {
                    if (updateCommand.Connection.State == ConnectionState.Closed) {
                        updateCommand.Connection.Open();
                    }
                    if (update) {
                        LOGGER.Log(LogLevel.Information, $"Updating an object with id:{persistent.Id} SQL:{updateCommandText}");
                        LOGGER.Log(LogLevel.Information, updateCommand.Parameters.ToString());
                        updateCommand.ExecuteNonQuery();
                    } else {
                        LOGGER.Log(LogLevel.Information, $"Inserting an object with id:{persistent.Id} SQL:{updateCommandText}");
                        LOGGER.Log(LogLevel.Information, updateCommand.Parameters.ToString());
                        object result = updateCommand.ExecuteScalar();
                        if (result != null && result != DBNull.Value) {
                            if (persistent.Id == 0)//for inheritance
                            {
                                persistent.Id = decimal.ToInt32((decimal) result);
                            }
                        }
                    }
                    LOGGER.Log(LogLevel.Information, updateCommand.CommandText);
                } catch (Exception exception) {
                    foreach (SqlParameter sqlParameter in updateCommand.Parameters) {
                        LOGGER.Log(LogLevel.Error, sqlParameter.ParameterName + "=" + sqlParameter.Value);
                    }
                    throw new CommonEasyPersistException("Exception saving object", exception);
                }
            }
            _cache.Put(persistent);
        }

        private SqlCommand GetUpdateCommandWithParameters(Type type, IPersistent persistent, SqlTransaction transaction) {
            SqlCommand updateCommand = new SqlCommand();
            updateCommand.Connection = transaction.Connection;
            updateCommand.Transaction = transaction;
            updateCommand.CommandTimeout = CommandTimeout;
            //for lasy checks if initialized object, for not lazy always is false
            bool isInitialized = persistent is ILazy && ((ILazy)persistent).Initialized;
            //loop object properties
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)) {
                object[] customAttributes = pi.GetCustomAttributes(false);
                //loop property attributes
                foreach (object o in customAttributes) {
                    if (o.GetType() == typeof(PersistentPropertyAttribute) && pi.Name != "Id") {
                        PersistentPropertyAttribute ppa = (PersistentPropertyAttribute)o;
                        if ((ppa.Lazy && isInitialized) || !ppa.Lazy) {//add only if not lazy or not initialized
                            string dbFieldName = ppa.DbFieldName;
                            SqlParameter sqlParameter = new SqlParameter();
                            sqlParameter.DbType = ppa.DbType;
                            sqlParameter.IsNullable = true;
                            sqlParameter.ParameterName = alfaNumOnlyRegex.Replace(dbFieldName, "_");
                            sqlParameter.SourceColumn = dbFieldName;
                            //in not object (doesn't implement IPersistent)
                            if (!typeof(IPersistent).IsAssignableFrom(pi.PropertyType)) {
                                sqlParameter.Value = pi.GetValue(persistent, null);
                                if (sqlParameter.Value == null) {
                                    sqlParameter.Value = DBNull.Value;
                                } else if (sqlParameter.DbType == DbType.DateTime) {
                                    //change min date to null
                                    if (((DateTime)sqlParameter.Value) == DateTime.MinValue) {
                                        sqlParameter.Value = DBNull.Value;
                                    }
                                } else if (sqlParameter.DbType == DbType.AnsiString) {
                                    //serialize into string, probably change this later???
                                    sqlParameter.Value = sqlParameter.Value.ToString();
                                }
                            } else {
                                IPersistent linkedPersistent = (IPersistent)pi.GetValue(persistent, null);
                                if (linkedPersistent != null) {
                                    sqlParameter.Value = linkedPersistent.Id;
                                } else {
                                    sqlParameter.Value = DBNull.Value;
                                }
                            }
                            updateCommand.Parameters.Add(sqlParameter);
                        }
                    }
                    if(o.GetType() == typeof(PersistentCollectionAttribute))
                    {
                        if (pi.GetValue(persistent,null) == null)
                        {
                            ArrayListWrapper alw = new ArrayListWrapper((PersistentCollectionAttribute)o,
                                                       persistent, ((PersistentCollectionAttribute)o).PersistentType, this);
                            pi.SetValue(persistent, alw, null);
                        }
                    }
                }
            }
            //we add an id in to update parameters collection if needed
            if (type.GetProperty("Id", BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public) == null) {
                PropertyInfo pi = type.GetProperty("Id");
                PersistentPropertyAttribute ppa =
                   (PersistentPropertyAttribute)pi.GetCustomAttributes(typeof(PersistentPropertyAttribute), false)[0];
                string dbFieldName = ppa.DbFieldName;
                SqlParameter sqlParameter = new SqlParameter();
                sqlParameter.DbType = ppa.DbType;
                sqlParameter.IsNullable = true;
                sqlParameter.ParameterName = alfaNumOnlyRegex.Replace(dbFieldName, "_");
                sqlParameter.Value = pi.GetValue(persistent, null);
                sqlParameter.SourceColumn = dbFieldName;
                updateCommand.Parameters.Add(sqlParameter);
            }
            return updateCommand;
        }

        /// <summary>
        /// Saves or updates a persistent in db
        /// If (Id == 0) then Saves!
        /// it saves only primitive properties (not persistent child)
        /// </summary>
        /// <param name="persistent">an object to save</param>
        public override void SaveOrUpdate(IPersistent persistent) {
            SqlConnection sqlConnection;
            if (SqlTransaction != null) {
                sqlConnection = SqlTransaction.Connection;
                if (sqlConnection.State != ConnectionState.Open) {
                    sqlConnection.Open();
                }
                SaveOrUpdate(persistent, SqlTransaction);
            } else {
                using (sqlConnection = new SqlConnection(SqlConnectionString))
                {
                    if (sqlConnection.State != ConnectionState.Open) {
                        sqlConnection.Open();
                    }
                    using(SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        try {
                            SaveOrUpdate(persistent, transaction);
                            transaction.Commit();
                        } catch (Exception e) {
                            transaction.Rollback();
                            LOGGER.Log(LogLevel.Error, e.Message, e);
                            throw new CommonEasyPersistException("Save or Update Error: " + e.Message, e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// DELETE object
        /// </summary>
        /// <param name="persistent">Persistent to delete</param>
        public override void DeleteObject(ref IPersistent persistent) {
            if(SqlTransaction==null)
            {
                using(SqlConnection sqlConnection = new SqlConnection(SqlConnectionString))
                {
                    if (sqlConnection.State != ConnectionState.Open) {
                        sqlConnection.Open();
                    }
                    using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                    {
                        try {
                            DeleteObject(ref persistent, transaction);
                            transaction.Commit();
                        } catch (Exception e) {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }else
            {
                DeleteObject(ref persistent, SqlTransaction);
            }
            persistent = null;
        }
        /// <summary>
        /// DELETE object in a transaction
        /// </summary>
        /// <param name="persistent"></param>
        /// <param name="transaction"></param>
        public virtual void DeleteObject(ref IPersistent persistent, SqlTransaction transaction) {
            string deleteSql = $"DELETE FROM {GetTableName(persistent.GetType())} WHERE {IdColumnName(persistent)}=@id";
            SqlCommand sqlCommand = new SqlCommand(deleteSql, transaction.Connection, transaction);
            sqlCommand.Parameters.AddWithValue("id", persistent.Id);
            sqlCommand.CommandTimeout = CommandTimeout;
            LOGGER.Log(LogLevel.Information, $"Deleting an object with id:{persistent.Id} SQL:{deleteSql}");
            sqlCommand.ExecuteNonQuery();
            _cache.Remove(persistent);

        }

        /// <summary>
        ///Initializes collection
        /// (Is used to initialize lazy collection)
        ///usually used to remove extra count query when the collection is not supposed to be empty
        /// </summary>
        /// <param name="collection">Lazy Collection ArrayListWrapper</param>
        public static void InitializeLazyCollection(ArrayList collection) {
            if (collection != null) {
                if (collection.GetType() == typeof(ArrayListWrapper)) {
                    ((ArrayListWrapper)collection).Initialize();
                } else {
                    throw new CommonEasyPersistException("This collection is not a descendant of ArrayListWrapper.");
                }
            }
        }

        /// <summary>
        /// Creates many to many item.
        /// </summary>
        /// <param name="parent">parent item</param>
        /// <param name="child">child item</param>
        public virtual void SaveManyToManyLinkItem(IPersistent parent, IPersistent child)
        {
            var props = parent.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersistentCollectionManyToManyAttribute)));
            foreach (PropertyInfo propertyInfo in props)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(typeof(PersistentCollectionManyToManyAttribute), false);
                if (attributes.Length != 1)
                {
                    string message = "Property: " + propertyInfo.Name + " of type:" + parent.GetType()
                                     + " has more than one PersistentCollectionManyToManyAttributes. Only one is allowed.";
                    throw new CommonEasyPersistException(message);
                }
                PersistentCollectionManyToManyAttribute attribute = (PersistentCollectionManyToManyAttribute)attributes[0];
                if (attribute.PersistentType == child.GetType())
                {
                    string linkTableName = attribute.LinkTable;
                    string parentIdColumn = attribute.DbFieldName;
                    string childIdColumn = attribute.LinkTableJoinFieldName;
                    //sql commands
                    string deleteSql = "DELETE FROM [" + linkTableName + "] WHERE ["
                                       + parentIdColumn + " ] = @parentId AND [" + childIdColumn + "]=@childId;";
                    string insertSql = "INSERT INTO [" + linkTableName + "] (["
                                       + parentIdColumn + "],[" + childIdColumn + "]) VALUES (@parentId,@childId);";
                    string batchedCommand = deleteSql + insertSql;
                    using (SqlCommand command = new SqlCommand(batchedCommand))
                    {
                        command.Parameters.AddWithValue("parentId", parent.Id);
                        command.Parameters.AddWithValue("childId", child.Id);
                        ExecuteNonQuery(command);
                    }
                }
            }
        }
        /// <summary>
        /// Executes the command and fills DataTable with results.
        /// </summary>
        /// <param name="cmd">Sql Command to execute</param>
        /// <param name="sqlConnection">Connection to use</param>
        /// <returns>DataTable with results from database</returns>
        private DataTable FillTableFromDatabase(SqlCommand cmd)
        {
            DataTable table = new DataTable();
            ApplayTransaction(cmd);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(table);
            }
            return table;
        }


        #region STATIC_HELPER_METHODS


        /// <summary>
        /// 
        /// makes a human readable string from persistent object :)
        /// just gets all properties and their names
        /// </summary>
        /// <param name="persistent">a persistent to make string from</param>
        /// <returns>human readable representation of object</returns>
        public static string ToString(IPersistent persistent) {
            string value = "";
            if (!(persistent is ILazy)) {
                foreach (PropertyInfo pi in persistent.GetType().GetProperties()) {
                    value += pi.Name + "=" + pi.GetGetMethod().Invoke(persistent, new object[]{}) + "\n";
                }
            } else {
                value += "LAZY OBJECT TODO";
            }
            return value;
        }
        /// <summary>
        /// Makes a comma separated line
        /// </summary>
        /// <param name="persistent">an object to make csv from</param>
        /// <returns>csv string of properties marked with PersistentPropertyAttribute </returns>
        public static string ToCsvString(IPersistent persistent) {
            StringBuilder stringBuilder = new StringBuilder();
            if (!(persistent is ILazy)) {
                foreach (PropertyInfo pi in persistent.GetType().GetProperties()) {
                    Object[] customAttributes = pi.GetCustomAttributes(true);
                    foreach (Object o in customAttributes) {
                        if (o.GetType().Name.Equals("PersistentPropertyAttribute")) {
                            object result = pi.GetGetMethod().Invoke(persistent, new object[] { });
                            string resultedString = result.ToString();
                            resultedString = resultedString.Replace(Environment.NewLine, " ");
                            resultedString = resultedString.Replace("\"", "'");
                            stringBuilder.Append("\"");
                            stringBuilder.Append(resultedString);
                            stringBuilder.Append("\",");
                        }
                    }
                }
            }
            if (stringBuilder.Length > 0) {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            return stringBuilder.ToString();

        }
        #endregion

        #region PURE_SQL_HELPER_FUNCTIONS
        /// <summary>
        /// Helper function to query database usually used to collect elements 
        /// 
        /// </summary>
        /// <param name="cmd">query</param>
        /// <returns>(int)cmd.ExecuteScalar();</returns>
        public virtual int CountItems(SqlCommand cmd) {
            SqlConnection sqlConnection = ApplayTransaction(cmd);
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            int res;
            try {
                LOGGER.Log(LogLevel.Information, $"Executing Count Items command SQL:{cmd.CommandText}");
                res = (int)cmd.ExecuteScalar();
            } finally {
                if(SqlTransaction==null)
                {
                    sqlConnection.Close();
                }
            }
            cmd.Dispose();
            return res;
        }
        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>returns cmd.ExecuteScalar(); Object</returns>
        public virtual object ExecuteScalar(SqlCommand cmd) {
            SqlConnection sqlConnection = ApplayTransaction(cmd);
            if (sqlConnection.State!=ConnectionState.Open) {
                sqlConnection.Open();
            }
            object res;
            try {
                LOGGER.Log(LogLevel.Information, $"Executing ExecuteScalar command SQL:{cmd.CommandText}");
                res = cmd.ExecuteScalar();
                cmd.Dispose();
            } catch {
                sqlConnection.Close();
                throw;
            } finally {
                if (SqlTransaction==null) {
                    if (sqlConnection.State != ConnectionState.Closed)
                    {
                        sqlConnection.Close();
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Executes a command in a transaction
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="transaction">Transaction to use</param>
        /// <returns>cmd.ExecuteScalar()</returns>
        public virtual object ExecuteScalar(SqlCommand cmd, SqlTransaction transaction) {
            object res;
            cmd.Transaction = transaction;
            cmd.Connection = transaction.Connection;
            LOGGER.Log(LogLevel.Information, $"Executing ExecuteScalar command SQL:{cmd.CommandText}");
            res = cmd.ExecuteScalar();
            cmd.Dispose();
            return res;
        }
        /// <summary>
        /// Executes a query cmd.ExecuteNonQuery()
        /// </summary>
        /// <param name="cmd">SQL command to execute</param>
        /// <returns>The number of rows affected</returns>
        public virtual int ExecuteNonQuery(SqlCommand cmd) {
            SqlConnection sqlConnection = ApplayTransaction(cmd);
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            int result;
            try
            {
                LOGGER.Log(LogLevel.Information, $"Executing ExecuteNonQuery command SQL:{cmd.CommandText}");
                result = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                LOGGER.Log(LogLevel.Error, $"Error Executing ExecuteNonQuery :{ex.Message}", ex);
                sqlConnection.Close();
                throw;
            }
            finally
            {
                if (SqlTransaction == null)
                {
                    if (sqlConnection.State != ConnectionState.Closed)
                    {
                        sqlConnection.Close();
                    }
                }
            }
            cmd.Dispose();
            return result;
        }
        /// <summary>
        /// Executes a query cmd.ExecuteNonQuery() in a custom transaction
        /// </summary>
        /// <param name="cmd">SQL command to execute</param>
        /// <returns>The number of rows affected</returns>
        public virtual int ExecuteNonQuery(SqlCommand cmd, SqlTransaction transaction) {
            cmd.Transaction = transaction;
            cmd.Connection = transaction.Connection;
            LOGGER.Log(LogLevel.Information, $"Executing ExecuteNonQuery command SQL:{cmd.CommandText}");
            int res = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return res;
        }
        /// <summary>
        /// Executes the command and returns a filled DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public virtual DataTable GetList(SqlCommand cmd) {
            LOGGER.Log(LogLevel.Information, $"Executing GetList command SQL:{cmd.CommandText}");
            DataTable table = FillTableFromDatabase(cmd);
            cmd.Dispose();
            return table;
        }
        

        /// <summary>
        /// Executes a query. returns the number of rows affected (actually <seealso cref="SqlCommand.ExecuteNonQuery"/> is returned)
        /// 
        /// </summary>
        /// <param name="cmdText">SQL query text</param>
        /// <returns>The number of rows affected</returns>
        public virtual int ExecuteNonQuery(string cmdText) {
            SqlCommand sqlCommand = new SqlCommand(cmdText);
            return ExecuteNonQuery(sqlCommand);
        }
        #endregion
    }
}