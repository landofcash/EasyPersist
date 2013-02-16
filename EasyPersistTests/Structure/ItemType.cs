using System.Collections;
using System.Data;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.IFaces;

namespace EasyPersist.Tests.Structure
{
    /// <summary>
    /// many to many left side
    /// </summary>
    [PersistentClass("ItemType")]
    public class ItemType : IPersistent
    {
        [PersistentProperty("ItemTypeId", DbType.Int32)]
        public int Id { get; set; }

        [PersistentProperty("Name")]
        public string Name { get; set; }

        [PersistentCollectionManyToMany("ItemTypeId", typeof(Item), "ItemTypeToItem", "ItemId")]
        public ArrayList Items { get; set; }
    }
}