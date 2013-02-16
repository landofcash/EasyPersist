using System.Data;
using EasyPersist.Core.Attributes;

namespace EasyPersist.Demo {
    public class TestLocation {
        private int _id;
        private string _city;
        private string _county;
        private string _state;
        
        [PersistentProperty("LocCityId", DbType.Int32)]
        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        [PersistentProperty("CityName", DbType.String)]
        public string City {
            get { return _city; }
            set { _city = value; }
        }
        [PersistentProperty("CountyName", DbType.String)]
        public string County {
            get { return _county; }
            set { _county = value; }
        }
        [PersistentProperty("StateName", DbType.String)]
        public string State {
            get { return _state; }
            set { _state = value; }
        }
        public override string ToString()
        {
            return City + ", " + County + ", " + State;
        }
        public string CityCaps {
            get { return _city.ToUpper(); }
        }
    }
}
