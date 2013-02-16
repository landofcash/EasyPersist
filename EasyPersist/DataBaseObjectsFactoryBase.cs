using System;
using System.Collections.Generic;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Core
{
    public abstract class DataBaseObjectsFactoryBase: IDataBaseObjectsFactory
    {
        public DataBaseObjectsFactoryBase()
        {
            CommandTimeout = 30;
        }

        public int CommandTimeout { get; set; }
        public string SqlConnectionString { get; protected set; }

        public abstract void getFromDb(int id, ref IPersistent persistent);
        public abstract void getFromDb(int id, ref IPersistent persistent, bool initLazy);

        public abstract IList<IPersistent> getListFromDb(PersistentCollectionAttribute pca, ref IPersistent parent,
                                                         Type collectionObjectsClass);

        public abstract int CountCollectionItems(PersistentCollectionAttribute pca, IPersistent parent);
        public abstract void SaveOrUpdate(IPersistent persistent);
        public abstract void DeleteObject(ref IPersistent persistent);
    }
}