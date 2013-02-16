using System;

namespace EasyPersist.Core.IFaces
{
	public interface ICache
	{
        int Count { get; }
		IPersistent Get(int id, Type type);
		void Put(IPersistent obj);
		void Remove(IPersistent persistent);
        void RemoveAll();
	}
}