using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.Exceptions;
using EasyPersist.Core.IFaces;
using NLog;

namespace EasyPersist.Core {
    public abstract class DataBaseObjectsFactorySQLCore : DataBaseObjectsFactoryBase {
        private readonly Logger LOGGER = LogManager.GetLogger("DataBaseObjectsFactorySQLCore");

        private readonly Dictionary<PropertyInfo, Object[]> propertyCustomAttributes = new Dictionary<PropertyInfo, object[]>();
        
        /// <summary>
        /// Определяет Имя колонки ИД у IPersistent обьекта
        /// </summary>
        /// <param name="persistent">Обьект у которого определять</param>
        /// <returns>имя колонки Id (из атрибута)</returns>
        protected string IdColumnName(IPersistent persistent) {
            return IdColumnName(persistent.GetType());
        }

        /// <summary>
        /// Определяет Имя колонки ИД у типа тмплементирующего IPersistent 
        /// </summary>
        /// <param name="persistentType">Тип имплементирующий IPersistent у которого опеределять</param>
        /// <returns>имя колонки Id (из атрибута)</returns>
        private string IdColumnName(Type persistentType) {
            PropertyInfo propertyInfo = persistentType.GetProperty("Id");
            Object[] idAttributes = propertyInfo.GetCustomAttributes(typeof(PersistentPropertyAttribute), true);
            if (idAttributes.Length != 1) {
                LOGGER.Log(LogLevel.Error, "Too many IPersistent Attributes with property: Id");
                throw new CommonEasyPersistException("Too many IPersistent Attributes with property: Id");
            }
            PersistentPropertyAttribute persistentPropertyAttribute = (PersistentPropertyAttribute) idAttributes[0];
            string idColumnName = "["+persistentPropertyAttribute.DbFieldName+"]";
            return idColumnName;
        }
        /// <summary>
        /// Определяет Название таблицы для свойства
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetTableName(Type type)
        {
            //определяем название таблицы и делаем правильное название св-ва
            Object[] classAttributes = type.GetCustomAttributes(typeof(PersistentClassAttribute), false);
            if (classAttributes.Length > 1) {
                LOGGER.Log(LogLevel.Error, "Too many PersistentClassAttributes for class:" + type.Name);
                throw new CommonEasyPersistException("Too many PersistentClassAttributes for class:" + type.Name);
            }
            if (classAttributes.Length == 1) {
                PersistentClassAttribute persistentClassAttribute = (PersistentClassAttribute)classAttributes[0];
                string[] items = persistentClassAttribute.DbTableName.Split('.');
                var result = "";
                foreach (var item in items)
                {
                    result += "[" + item + "].";
                }
                return result.Trim('.');
            }
            LOGGER.Log(LogLevel.Error, "Cant find PersistentClassAttributes for class:" + type.Name);
            throw new CommonEasyPersistException("Cant find PersistentClassAttributes for class:" + type.Name);
        }
        
        /// <summary>
        /// Makes an SQL query from persistent object
        /// </summary>
        /// <param name="persistent">On object to make query for</param>
        /// <param name="lazy">If load lazy properties</param>
        /// <returns>an sql query</returns>
        protected string PrepareSelectSql(Type persistentType, bool lazy) {
            StringBuilder selectQuery = new StringBuilder("SELECT ");
            PropertyInfo[] propertyInfos = persistentType.GetProperties();
            //цикл по свойствам обьекта
            foreach (PropertyInfo pi in propertyInfos) {
                //Массив всех атрибутов у свойства
                Object[] customAttributes = pi.GetCustomAttributes(typeof(PersistentPropertyAttribute), true);
                //делаем из атрибута строку
                if (customAttributes.Length > 1)
                {
                    string message = "Too many PersistentPropertyAttribute for property:" + pi.Name;
                    LOGGER.Log(LogLevel.Error, message);
                    throw new CommonEasyPersistException(message);
                }
                if (customAttributes.Length==1)
                {
                    PersistentPropertyAttribute persistentPropertyAttribute = (PersistentPropertyAttribute)customAttributes[0];
                    //Если нашли нужный атрибут то парсим в строку запроса
                    if (!(lazy && persistentPropertyAttribute.Lazy) && persistentPropertyAttribute.GetType() == typeof(PersistentPropertyAttribute)) {
                        //определяем название таблицы и делаем правильное название св-ва
                        selectQuery.Append(GetTableName(pi.DeclaringType)).Append(".[")
                                   .Append(persistentPropertyAttribute.DbFieldName).Append("]").Append(",");
                    }
                }
            }
            selectQuery.Remove(selectQuery.Length - 1,1);
            selectQuery
                .Append(" FROM ")
                .Append(MakeFromString(persistentType))
                .Append(" AND ")
                .Append(GetTableName(persistentType)).Append(".").Append(IdColumnName(persistentType))
                .Append(" = @id");
            return selectQuery.ToString();
        }
        /// <summary>
        /// Creates an SQL SELECT FROM part of the query. 
        /// creates Inner Joins if required by class inheritance
        /// </summary>
        /// <param name="persistentType">Type of persistent to make From query for</param>
        /// <returns></returns>
        private string MakeFromString(Type persistentType) {
            StringBuilder fromString = new StringBuilder(GetTableName(persistentType));
            Type type = persistentType.BaseType;
            object[] classAttributes;
            while (type != null && type != typeof(object)) {
                classAttributes = type.GetCustomAttributes(typeof(PersistentClassAttribute), false);
                if (classAttributes.Length > 1)
                {
                    string message = "Too many PersistentClassAttributes for class:" + type.Name;
                    LOGGER.Log(LogLevel.Error, message);
                    throw new CommonEasyPersistException(message);
                }
                if (classAttributes.Length == 1) {
                    PersistentClassAttribute persistentClassAttribute = (PersistentClassAttribute)classAttributes[0];
                    fromString.AppendFormat(" INNER JOIN {1} ON {2} = {0}",
                                            GetTableName(persistentType) + "." + IdColumnName(persistentType),
                                            persistentClassAttribute.DbTableName,
                                            GetTableName(type) + "." + IdColumnName(type)
                        );
                }
                type = type.BaseType;
            }
            classAttributes = persistentType.GetCustomAttributes(typeof(PersistentClassAttribute), false);
            StringBuilder wherePartStringBuilder = new StringBuilder();
            if (classAttributes.Length > 1)
            {
                string message = "Too many PersistentClassAttributes for class:" + persistentType.Name;
                LOGGER.Log(LogLevel.Error, message);
                throw new CommonEasyPersistException(message);
            }
            if (classAttributes.Length == 1) {
                PersistentClassAttribute persistentClassAttribute = (PersistentClassAttribute)classAttributes[0];
                foreach (Type inheretedType in persistentClassAttribute.InheritedTypes) {
                    fromString.AppendFormat(" LEFT OUTER JOIN {1} ON {2} = {0}",
                                            GetTableName(persistentType) + "." + IdColumnName(persistentType),
                                            GetTableName(inheretedType),
                                            GetTableName(inheretedType) + "." + IdColumnName(inheretedType));
                    wherePartStringBuilder.Append(" AND ").Append(GetTableName(inheretedType)).Append(".")
                                          .Append(IdColumnName(inheretedType)).Append(" is null ");
                }
            } else
            {
                string message = "No PersistentClassAttribute for class:" + persistentType.Name;
                LOGGER.Log(LogLevel.Error, message);
                throw new CommonEasyPersistException(message);
            }
            fromString.Append(" WHERE 1=1 ").Append(wherePartStringBuilder);
            return fromString.ToString();
        }
        
        public abstract void getFromDb(int id, ref IPersistent persistent, IList<IPersistent> alreadyLoaded, bool lazy);

        /// <summary>
        ///			Fills a persistent properties with values from dataRow
        ///			Also creates child persistent objects and collections
        /// </summary>
        /// <param name="dr">DataRow to get values from</param>
        /// <param name="persistent">An Object to fill</param>
        protected void createObject(DataRow dr, IPersistent persistent, IList<IPersistent> alreadyLoaded, bool lazy) {
            alreadyLoaded.Add(persistent);
            //persistent Properties loop
            foreach (PropertyInfo pi in persistent.GetType().GetProperties()) {
                //an optimization to not call GetCustomAttributes ut use cached list
                if (!propertyCustomAttributes.ContainsKey(pi)) 
                {
                    propertyCustomAttributes.Add(pi,pi.GetCustomAttributes(true));
                }
                //getting all Property attributes
                object[] customAttributes = propertyCustomAttributes[pi];
                //looping over all attributes of current  Property
                foreach (Object o in customAttributes) {
                    //If this property is marked by PersistentPropertyAttribute then we need to fill it from rs
                    if (o.GetType()==typeof(PersistentPropertyAttribute)) {
                        PersistentPropertyAttribute ppa = (PersistentPropertyAttribute)o;
                        if (!(ppa.Lazy && lazy)) {
                            //Database column Name (We get it from attribute)
                            string dbFieldName = ppa.DbFieldName;
                            if (pi.CanWrite) {
                                try {
                                    //if value for this property is not null
                                    if (dr[dbFieldName] != null && !(dr[dbFieldName] is DBNull) && !dr.IsNull(dbFieldName)) {
                                        //if this property is a persistent object then we need to get it from db
                                        if (typeof(IPersistent).IsAssignableFrom(pi.PropertyType))
                                        {
                                            //creating a child object with default constructor
                                            ConstructorInfo constructor = pi.PropertyType.GetConstructor(Type.EmptyTypes);
                                            if (constructor != null)
                                            {
                                                IPersistent child = ((IPersistent) constructor.Invoke(new object[]{}));
                                                //getting an object from database
                                                //TODO а может это дерьмо сделать lazy??? lazy objects gigi :)
                                                getFromDb((int) dr[dbFieldName], ref child, alreadyLoaded, true);
                                                //setting Property 
                                                pi.SetValue(persistent, child, null);
                                            }
                                            else
                                            {
                                                string message = "Can't find parametrless constructor for type:" + pi.PropertyType.FullName;
                                                LOGGER.Log(LogLevel.Error, message);
                                                throw new CommonEasyPersistException(message);
                                            }
                                        }
                                        else if (typeof(Guid).IsAssignableFrom(pi.PropertyType))
                                        {
											pi.SetValue(persistent, dr[dbFieldName], null);
                                        } else {
                                            ConstructorInfo prop_ci = pi.PropertyType.GetConstructor(new[] { typeof(string) });
                                            if (prop_ci == null) {
                                                //nulable type
                                                object value = dr[dbFieldName];
                                                if (pi.PropertyType.Name == typeof(Nullable<>).Name)
                                                {
                                                    Type nullableType = Nullable.GetUnderlyingType(pi.PropertyType);
                                                    //if enum
                                                    if (nullableType.IsEnum)
                                                    {
                                                        value = Enum.ToObject(nullableType, dr[dbFieldName]);
                                                    }
                                                }
                                                //just simple type property
                                                if (ppa.Converter != null)
                                                {
                                                    
                                                    pi.SetValue(persistent, ppa.Converter.Convert(value), null);
                                                }
                                                else
                                                {
                                                    pi.SetValue(persistent, value, null);
                                                }
                                            } else {
                                                pi.SetValue(persistent, prop_ci.Invoke(new[] { dr[dbFieldName] }), null);
                                            }
                                        }
                                    }
                                        //setting null if we have got null from db
                                    else {
                                        pi.SetValue(persistent, null, null);
                                    }
                                } catch (Exception e)
                                {
                                    string message = "Exception loading object. FieldName:" + dbFieldName + " Property:" + pi.Name;
                                    LOGGER.LogException(LogLevel.Error, message, e);
                                    throw new CommonEasyPersistException(message, e);
                                }
                                //going to the next attribute
                                break;
                            }
                        }
                    }
                        //for the persistent collections
                    else if (o is PersistentCollectionAttribute) {
                        //we create a list wrapper (lazy collection)
                        ArrayListWrapper alw = (ArrayListWrapper)pi.GetValue(persistent, null);
                        if (alw == null) {
                            alw = new ArrayListWrapper((PersistentCollectionAttribute)o,
                                                       persistent, ((PersistentCollectionAttribute)o).PersistentType, this);
                            pi.SetValue(persistent, alw, null);
                        } else {
                            alw.addSubCollection((PersistentCollectionAttribute)o,
                                                 persistent, ((PersistentCollectionAttribute)o).PersistentType);
                        }

                    }
                }	//next attribute
            } //next property
        }

        protected void CreateReadOnlyObject(DataRow dr, object persistent) {
            //Properties loop
            foreach (PropertyInfo pi in persistent.GetType().GetProperties()) {
                //getting all Property attributes
                if (!propertyCustomAttributes.ContainsKey(pi)) {
                    propertyCustomAttributes.Add(pi, pi.GetCustomAttributes(true));
                }
                Object[] customAttributes = propertyCustomAttributes[pi];
                //looping over all attributes of current  Property
                foreach (Object o in customAttributes) {
                    //If this property is marked by PersistentPropertyAttribute then we need to fill it from rs
                    if (o.GetType().Name.Equals("PersistentPropertyAttribute")) {
                        PersistentPropertyAttribute ppa = (PersistentPropertyAttribute)o;

                        //Database column Name (We get it from attribute)
                        string dbFieldName = ppa.DbFieldName;
                        if (pi.CanWrite) {
                            try {
                                //if value for this property is not null
                                if (!dr.IsNull(dbFieldName)) {
                                    //if this property is a persistent object then exception
                                    if (typeof(IPersistent).IsAssignableFrom(pi.PropertyType))
                                    {
                                        const string message = "Can't load child IPersistent object for object which is not an IPersistent itself. " 
                                            + "You can't make an IPersistent object to be a child of the 'readonly' object. Sorry";
                                        LOGGER.Log(LogLevel.Error, message);
                                        throw new CommonEasyPersistException(message);
                                    }
                                    ConstructorInfo prop_ci = pi.PropertyType.GetConstructor(new Type[] { typeof(string) });
                                    if (prop_ci == null) {
                                        //simple type property
                                        pi.SetValue(persistent, dr[dbFieldName], null);
                                    } else {
                                        pi.SetValue(persistent, prop_ci.Invoke(new[] { dr[dbFieldName] }), null);
                                    }
                                }
                                    //setting null if we have got null from db
                                else {
                                    pi.SetValue(persistent, null, null);
                                }
                            } catch (Exception e)
                            {
                                string message = "Exception loading readonly object. Property:" + pi.Name;
                                LOGGER.LogException(LogLevel.Error, message,e);
                                throw new CommonEasyPersistException(message, e);
                            }
                            //going to the next attribute
                            break;
                        }
                    }
                }	//next attribute
            } //next property
        }

        /// <summary>
        /// Готовим запрос для колекций детей
        /// </summary>
        /// <param name="pca">атрибут колекции там лежат ссылка на парента</param>
        /// <param name="parent">обьект парент</param>
        /// <param name="isCountQuery">для запросов подсчитывающих число записей true</param>
        /// <returns></returns>
        protected string prepareSelectQueryForChildCollections(PersistentCollectionAttribute pca,
                                                               IPersistent parent,
                                                               Type collectionElementType,
                                                               bool isCountQuery)
        {
            if (pca.GetType() == typeof(PersistentCollectionManyToManyAttribute)) {
                return PrepareSelectQueryForChildManyToManyCollections((PersistentCollectionManyToManyAttribute)pca, 
                                                                       parent,
                                                                       isCountQuery);
            }
            return prepareSelectQueryForChildManyToOneCollections(pca, parent, collectionElementType, isCountQuery);
        }

        private string prepareSelectQueryForChildManyToOneCollections(PersistentCollectionAttribute pca,
                                                                      IPersistent parent, Type collectionElementType,
                                                                      bool isCountQuery) {
            return prepareSelectQueryForChildManyToOneCollections(pca, parent, collectionElementType, isCountQuery, true);
        }
        
        private string prepareSelectQueryForChildManyToOneCollections(PersistentCollectionAttribute pca,
                                                                      IPersistent parent, Type collectionElementType,
                                                                      bool isCountQuery, bool isLazy) {
            if (isCountQuery) {
                string selectParams = "SELECT COUNT(" + IdColumnName(collectionElementType) + ") FROM "
                                      + GetTableName(pca.PersistentType) + " WHERE " 
                                      + GetTableName(pca.PersistentType) + ".[" + pca.DbFieldName + "] = " + parent.Id;
                return selectParams;
            } else
            {
                string selectParams = "SELECT ";
                string fromParams = " FROM " + MakeFromString(collectionElementType);
                string whereParams = " AND " + GetTableName(pca.PersistentType) + ".[" + pca.DbFieldName + "] = " + parent.Id;
                string orderParams = null;
                
                foreach (PropertyInfo pic in collectionElementType.GetProperties()) {
                    //getting all Property attributes
                    
                    Object[] customAttributes = pic.GetCustomAttributes(true);
                    foreach (Object customAttribute in customAttributes) {
                        if (customAttribute.GetType() == typeof(PersistentPropertyAttribute)) {
                            PersistentPropertyAttribute ppa = (PersistentPropertyAttribute)customAttribute;
                            if (!(isLazy && ppa.Lazy)) {
                                selectParams += GetTableName(pic.DeclaringType) + ".[" + ppa.DbFieldName + "],";
                            }
                            if (ppa.Order != null) {
                                orderParams = orderParams == null ? " ORDER BY " : ", "; //if first then "order by" else ","
                                orderParams += GetTableName(pic.DeclaringType) + ".[" + ppa.DbFieldName + "] ";
                                orderParams += ppa.Order;
                            }
                        }
                    }
                }
                //удаляем запятую
                selectParams = selectParams.Remove(selectParams.Length - 1);
                string result = selectParams + fromParams + whereParams;
                if (orderParams != null) {
                    result = result + orderParams;
                }
                return result;
            }
        }

        private string PrepareSelectQueryForChildManyToManyCollections(PersistentCollectionManyToManyAttribute pca,
                                                                       IPersistent parent,
                                                                       bool isCountQuery) {
            return PrepareSelectQueryForChildManyToManyCollections(pca, parent, isCountQuery, true);
        }

        private string PrepareSelectQueryForChildManyToManyCollections(PersistentCollectionManyToManyAttribute pca,
                                                                       IPersistent parent,
                                                                       bool isCountQuery, bool isLasy) {
            //throw new NotImplementedException("Sorry this is still under construction.");
            //TODO fix to use inheritance 
            string selectParams;                                                                                                                             
            if (isCountQuery) {
                selectParams = "SELECT COUNT(DISTINCT " + GetTableName(pca.PersistentType) + "." + IdColumnName(pca.PersistentType) + ") ";
            } else {
                selectParams = "SELECT DISTINCT ";
            }
            string fromParams =
                " FROM " + GetTableName(pca.PersistentType) + " LEFT JOIN " + pca.LinkTable
                + " ON " + GetTableName(pca.PersistentType) + "." + IdColumnName(pca.PersistentType) + "=" + pca.LinkTable + "." + pca.LinkTableJoinFieldName;
            string whereParams = " WHERE " + pca.LinkTable + "." + pca.DbFieldName + " = " + parent.Id;
            string orderParams = null;
            if (!isCountQuery) {
                foreach (PropertyInfo pic in pca.PersistentType.GetProperties()) {
                    //getting all Property attributes
                    Object[] customAttributes = pic.GetCustomAttributes(true);
                    foreach (Object customAttribute in customAttributes) {
                        if (customAttribute.GetType() == typeof(PersistentPropertyAttribute)) {
                            PersistentPropertyAttribute ppa = (PersistentPropertyAttribute)customAttribute;
                            if (!(isLasy && pca.Lazy)) {
                                selectParams += GetTableName(pca.PersistentType)+".[" + ppa.DbFieldName + "],";
                            }
                            if (ppa.Order != null) {
                                orderParams = orderParams == null ? " ORDER BY " : ", ";
                                //TODO Добавить порядок добавления ордеров в конечный список
                                orderParams += GetTableName(pca.PersistentType) + ".[" + ppa.DbFieldName + "] ";
                                orderParams += ppa.Order;
                            }
                        }
                    }
                }
                //удаляем запятую
                selectParams = selectParams.Remove(selectParams.Length - 1);
            }
            string result = selectParams + fromParams + whereParams;
            if (orderParams != null) {
                result = result + orderParams;
            }
            return result;
        }
    }
}