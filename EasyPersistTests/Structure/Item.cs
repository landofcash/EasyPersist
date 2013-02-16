using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Tests.Structure
{
    /// <summary>
    /// many to many left side
    /// </summary>
    [PersistentClass("Item")]
    public class Item:IPersistent
    {
        [PersistentProperty("ItemId", DbType.Int32)]
        public int Id { get; set; }

        [PersistentProperty("Name")]
        public string Name { get; set; }

        [PersistentCollectionManyToMany("ItemId", typeof(ItemType), "ItemTypeToItem","ItemTypeId")]
        public ArrayList ItemTypes { get; set; }
    }
}
