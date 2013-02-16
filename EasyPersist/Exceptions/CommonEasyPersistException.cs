using System;

namespace EasyPersist.Core.Exceptions {
    [Serializable]
    public class CommonEasyPersistException : Exception {
        public CommonEasyPersistException(string str) : base(str) {
        }

        public CommonEasyPersistException(string str, Exception ex) : base(str, ex) {

        }
    }
}
