using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyPersist.Core;
using EasyPersist.Tests.Structure;
using NUnit.Framework;

namespace EasyPersist.Tests
{
    [TestFixture]
    public class LoadWithRawSQLTests : BaseDBDrivenText
    {
        [Test]
        public void LoadWithSqlTest()
        {
            DataBaseObjectsFactorySQLServer dao = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            string[] itemNames = {"Item 1 LWST1", "Item 2 LWST1", "Item 3 LWST2", "Item 4 LWST1", "Item 5 LWST1", "Item 6 LWST2"};
            List<Item> items = itemNames.Select(i => new Item { Name = i }).ToList();
            foreach (Item item in items)
            {
                dao.SaveOrUpdate(item);    
            }
            string selectItemsSql = "SELECT * FROM Item WHERE Name like '%LWST1%'";
            List<Item> loadedItems = dao.getListFromDb<Item>(selectItemsSql);
            Assert.AreEqual(4,loadedItems.Count);
            StringAssert.AreEqualIgnoringCase("Item 1 LWST1", loadedItems[0].Name);
            StringAssert.AreEqualIgnoringCase("Item 2 LWST1", loadedItems[1].Name);
            StringAssert.AreEqualIgnoringCase("Item 4 LWST1", loadedItems[2].Name);
            StringAssert.AreEqualIgnoringCase("Item 5 LWST1", loadedItems[3].Name); 

        }
    }
}
