using System;

namespace EasyPersist.Core.Exceptions {
    [Serializable] 
	public class LazyAccessException : Exception {
		public LazyAccessException(string str) : base(str){
		}
	}
}
