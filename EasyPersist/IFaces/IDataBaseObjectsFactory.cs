using System;
using System.Collections.Generic;
using EasyPersist.Core.Attributes;

namespace EasyPersist.Core.IFaces
{
	/// <summary>
	/// Interface for Factory Class 
    /// 
    /// Each database engine need to have its own factory class
	/// </summary>
	public interface IDataBaseObjectsFactory
	{
		void getFromDb(int id, ref IPersistent persistent);
		void getFromDb(int id, ref IPersistent persistent, bool initLazy);
		IList<IPersistent> getListFromDb(PersistentCollectionAttribute pca
            , ref IPersistent parent, Type collectionObjectsClass);
        int CountCollectionItems(PersistentCollectionAttribute pca, IPersistent parent);
		void SaveOrUpdate(IPersistent persistent);
		void DeleteObject(ref IPersistent persistent);
	}
}
