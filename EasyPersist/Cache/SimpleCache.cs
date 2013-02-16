using System;
using System.Collections.Generic;
using System.Globalization;
using EasyPersist.Core.Exceptions;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Core.Cache {
	public class SimpleCache : ICache
	{
        private Dictionary<string, IPersistent> _elements = new Dictionary<string, IPersistent>();
		
        /// <summary>
        /// Returns an item from cache
        /// </summary>
        /// <param name="id">ID on an item</param>
        /// <param name="type">Type of an item to return</param>
        /// <returns>Persistent object</returns>
		public IPersistent Get(int id, Type type)
		{
			IPersistent value;
			_elements.TryGetValue(GetCacheId(id, type), out value);
			return value;
			
		}
        /// <summary>
        /// Insert an item in cache
        /// </summary>
        /// <param name="obj"></param>
		public void Put(IPersistent obj)
		{
            string key = GetCacheId(obj);
			if(!_elements.ContainsKey(key)){
				_elements.Add(key, obj);
			} else {
				_elements[key] = obj;
			}
		}
        /// <summary>
        /// Number of items in cache
        /// </summary>
	    public int Count { get { return _elements.Count; } }
        
        /// <summary>
        /// Removes an item from cache
        /// </summary>
        /// <param name="persistent"></param>
	    public void Remove(IPersistent persistent)
		{
			_elements.Remove(GetCacheId(persistent));
		}

        /// <summary>
        /// removes all items from cache
        /// </summary>
	    public void RemoveAll()
	    {
            _elements.Clear();
	    }


        private string GetCacheId(IPersistent pobj)
		{
			return GetCacheId(pobj.Id,pobj.GetType());
		}
		private string GetCacheId(int id,Type type) {
			if (id == 0) {
                throw new CommonEasyPersistException("Can't put not saved object in cache");
			}
			return (type + id.ToString(CultureInfo.InvariantCulture));
		}
	}
	
}
