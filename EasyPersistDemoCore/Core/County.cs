using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;


namespace EasyPersist.Demo.Core {
    [PersistentClass("County")]
    public class County : IPersistent {
        private int _id;
        private string _name;
        private State _state;
        //lasy list of cities (there is a wrapper class created and cities are loaded only if accessed)
        private ArrayList _cities;
        [PersistentProperty("CountyId", DbType.Int32)]
        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("Name")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
        [PersistentProperty("StateId")]
        public State State {
            get { return _state; }
            set { _state = value; }
        }
        [PersistentCollection("CountyId",typeof(City))]
        public ArrayList Cities
        {
            get { return _cities; }
            set { _cities = value; }
        }
    }
}
