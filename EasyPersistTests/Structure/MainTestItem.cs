using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyPersist.Core;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Tests.Structure
{
    [PersistentClass("MainTestItem")]
    public class MainTestItem : IPersistent
    {
        [PersistentProperty("MainTestItemId", DbType.Int32)]
        public int Id { get; set; }

        [PersistentProperty("GuidNotNull")]
        public Guid GuidNotNull { get; set; }

        [PersistentProperty("GuidNullable")]
        public Guid? GuidNullable { get; set; }

        [PersistentProperty("StringNullable")]
        public string StringNullable { get; set; }

        [PersistentProperty("OrderIndex",SortOrder.DESC)]
        public string OrderIndex { get; set; }

        [PersistentCollection("ParentId", typeof(MainTestItem))]
        public ArrayList Children { get; set; }

        [PersistentProperty("ParentId")]
        public MainTestItem Parent { get; set; }
    }
}
