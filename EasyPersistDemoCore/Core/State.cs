using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;


namespace EasyPersist.Demo.Core {
    [PersistentClass("State")]
    public class State : IPersistent {
        private int _id;
        private string _name;
        [PersistentProperty("StateId", DbType.Int32)]
        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("Name")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
    }
}
