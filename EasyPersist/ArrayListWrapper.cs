using System;
using System.Collections;
using System.Collections.Generic;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Core {
    /// <summary>
    /// A wrapper for lazy collections
    /// </summary>
    public class ArrayListWrapper : ArrayList {
		private readonly IList<LazySubCollection> _lazySubCollections = new List<LazySubCollection>();

		private IList<LazySubCollection> LazySubCollections {
			get { return _lazySubCollections; }
		}
        private IDataBaseObjectsFactory _dataBaseObjectsFactory;
        private bool initialized;
        private bool initializedCount;

        private int UninitCount { get; set; }

        public ArrayListWrapper(PersistentCollectionAttribute pca,IPersistent parent,
            Type listObjectTypes, IDataBaseObjectsFactory dataBaseObjectsFactory) {
            UninitCount = 0;
            _dataBaseObjectsFactory = dataBaseObjectsFactory;
			addSubCollection(pca, parent, listObjectTypes);		
			
        } 
		/// <summary>
        /// Adds a part of collection with similar elements
		/// </summary>
		/// <param name="pca">Collection attribute</param>
		/// <param name="parent">Parent object</param>
		/// <param name="listObjectTypes">Object typ in this sub collection</param>
		public void addSubCollection(PersistentCollectionAttribute pca,IPersistent parent,
            Type listObjectTypes){
			LazySubCollection lsc = new LazySubCollection(pca, parent, listObjectTypes);
			LazySubCollections.Add(lsc);
		}

        public override int Count {
            get {
                if (initialized) {
                    return base.Count;
                }
                if (!initializedCount) {
					foreach (LazySubCollection lsc in LazySubCollections) {
						UninitCount += _dataBaseObjectsFactory.CountCollectionItems(lsc.Pca, lsc.Parent);
					}
                    initializedCount = true;
                }
                return UninitCount;                
            }
        }
        public override object this[int index] {
            get {
                if (!initialized) {
                    Initialize();
                }
                return base[index];
            }
            set {
                //TODO how to save values?
                base[index] = value;
            }
        }
        public override bool Contains(object item) {
            if (!initialized) {
                Initialize();
            }
            return base.Contains(item);
        }
        public override int IndexOf(object value) {
            if (!initialized) {
                Initialize();
            }
            return base.IndexOf(value);
        }
        public override IEnumerator GetEnumerator() {
            if (!initialized) {
                Initialize();
            }
            return base.GetEnumerator();
        }
        public override IEnumerator GetEnumerator(int s, int e) {
            if (!initialized) {
                Initialize();
            }
            return base.GetEnumerator(s, e);
        }
        public void Initialize() {
        	Clear();
			foreach (LazySubCollection lsc in LazySubCollections) {
				IPersistent parent = lsc.Parent;
				IList<IPersistent> objs = _dataBaseObjectsFactory.getListFromDb(lsc.Pca, ref parent, lsc.ListObjectTypes);
				foreach(IPersistent iPersistent in  objs){
					base.Add(iPersistent);
				}				
			}            
            initialized = true;
        }
		public override int Add(object value) {
			if (!initialized) {
				Initialize();
			}
			return base.Add(value);
		}
		public override void AddRange(ICollection c) {
			if (!initialized) {
				Initialize();
			}
			base.AddRange(c);
		}

		public override int BinarySearch(int index, int count, object value, IComparer comparer) {
			if (!initialized) {
				Initialize();
			}
			return base.BinarySearch(index, count, value, comparer);
		}
		public override int BinarySearch(object value) {
			if (!initialized) {
				Initialize();
			}
			return base.BinarySearch(value);
		}
		public override int BinarySearch(object value, IComparer comparer) {
			if (!initialized) {
				Initialize();
			}
			return base.BinarySearch(value, comparer);
		}
		public override void CopyTo(int index, Array array, int arrayIndex, int count) {
			if (!initialized) {
				Initialize();
			}
			base.CopyTo(index, array, arrayIndex, count);
		}
		public override object Clone() {
			if (!initialized) {
				Initialize();
			}
			return base.Clone();
		}
		public override void CopyTo(Array array) {
			if (!initialized) {
				Initialize();
			}
			base.CopyTo(array);
		}
		public override void CopyTo(Array array, int arrayIndex) {
			if (!initialized) {
				Initialize();
			}
			base.CopyTo(array, arrayIndex);
		}
		public override ArrayList GetRange(int index, int count) {
			if (!initialized) {
				Initialize();
			}
			return base.GetRange(index, count);
		}
		public override int IndexOf(object value, int startIndex) {
			if (!initialized) {
				Initialize();
			}
			return base.IndexOf(value, startIndex);
		}
		public override int IndexOf(object value, int startIndex, int count) {
			if (!initialized) {
				Initialize();
			}
			return base.IndexOf(value, startIndex, count);
		}
		public override void Insert(int index, object value) {
			if (!initialized) {
				Initialize();
			}
			base.Insert(index, value);
		}

		public override void InsertRange(int index, ICollection c) {
			if (!initialized) {
				Initialize();
			}
			base.Insert(index, c);
		}
		public override int LastIndexOf(object value) {
			if (!initialized) {
				Initialize();
			}
			return base.LastIndexOf(value);
		}
		public override int LastIndexOf(object value, int startIndex) {
			if (!initialized) {
				Initialize();
			}
			return base.LastIndexOf(value, startIndex);
		}
		public override int LastIndexOf(object value, int startIndex, int count) {
			if (!initialized) {
				Initialize();
			}
			return base.LastIndexOf(value, startIndex, count);
		}
		public override void Remove(object obj) {
			if (!initialized) {
				Initialize();
			}
			base.Remove(obj);
		}
		public override void RemoveAt(int index) {
			if (!initialized) {
				Initialize();
			}
			base.RemoveAt(index);
		}
		public override void RemoveRange(int index, int count) {
			if (!initialized) {
				Initialize();
			}
			base.RemoveRange(index, count);
		}
		public override void Reverse() {
			if (!initialized) {
				Initialize();
			}
			base.Reverse();
		}
		public override void Reverse(int index, int count) {
			if (!initialized) {
				Initialize();
			}
			base.Reverse(index, count);
		}
		public override void SetRange(int index, ICollection c) {
			if (!initialized) {
				Initialize();
			}
			base.SetRange(index, c);
		}
		public override void Sort() {
			if (!initialized) {
				Initialize();
			}
			base.Sort();
		}
		public override void Sort(int index, int count, IComparer comparer) {
			if (!initialized) {
				Initialize();
			}
			base.Sort(index, count, comparer);
		}
		public override void Sort(IComparer comparer) {
			if (!initialized) {
				Initialize();
			}
			base.Sort(comparer);
		}
		public override object[] ToArray() {
			if (!initialized) {
				Initialize();
			}
			return base.ToArray();
		}
		public override Array ToArray(Type type) {
			if (!initialized) {
				Initialize();
			}
			return base.ToArray(type);
		}
		public override string ToString()
		{
		    if(initialized)
			{
				return base.ToString();
			}
		    return "Uninitialized Lazy Collection";
		}

        /// <summary>
        /// Checks if the array contains an <see cref="IPersistent"/> object
        /// comperes by type and Id.
        /// if Item is null uses <see cref="ArrayList.Contains"/> method
        /// </summary>
        /// <param name="arrayList">an arrayList to search for an item</param>
        /// <param name="item">an IPersistent Item to search for</param>
        /// <returns>true if an array contains an item</returns>
        public static bool Contains(ArrayList arrayList, IPersistent item)
        {
            if(item==null)
            {
                return arrayList.Contains(null);
            }
            foreach (object o in arrayList)
            {
                if (o is IPersistent persistentObj)
                {
                    if(persistentObj.Id==item.Id && persistentObj.GetType()==item.GetType())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
		/// <summary>
        /// Class to describe a part of the main collection of the same type elements
		/// To get objects stored in a specific table in db
		/// </summary>
		private class LazySubCollection {
			private IPersistent _parent;
			public IPersistent Parent {
				set { _parent = value; }
				get { return _parent; }
			}
			private PersistentCollectionAttribute _pca;
			public PersistentCollectionAttribute Pca {
				set { _pca = value; }
				get { return _pca; }
			}
			private Type _listObjectTypes;
			public Type ListObjectTypes {
				set { _listObjectTypes = value; }
				get { return _listObjectTypes; }
			}

			public LazySubCollection(PersistentCollectionAttribute pca,
				IPersistent parent,Type listObjectTypes) {
				Parent = parent;
				Pca = pca;
				ListObjectTypes = listObjectTypes;
			}
		}
    }	
}
