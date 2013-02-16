using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using EasyPersist.Core;
using EasyPersist.Core.Attributes;
using EasyPersist.Core.Exceptions;
using EasyPersist.Core.IFaces;
using EasyPersist.Tests.Structure;
using NUnit.Framework;

namespace EasyPersist.Tests {
    [TestFixture]
    public class CommonTest : BaseDBDrivenText{
        [Test]
        public void PrepareSelectSqlTest()
        {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp = new TempPersistent();
            //TODO fix this
            //string selectQuery = dbo.PrepareSelectSql(temp, true);
            //Assert.Equals("SELECT [TempPersistent].[Caption],[TempPersistentBase].[TempPersistentBaseId],[TempPersistentBase].[Name],[TempPersistentBase].[TempPersistentAloneId] FROM [TempPersistent] INNER JOIN TempPersistentBase ON [TempPersistentBase].[TempPersistentBaseId] = [TempPersistent].[TempPersistentBaseId] LEFT OUTER JOIN [ThirdLevelTempPersistent] ON [ThirdLevelTempPersistent].[TempPersistentBaseId] = [TempPersistent].[TempPersistentBaseId] WHERE 1=1  AND [ThirdLevelTempPersistent].[TempPersistentBaseId] is null  AND [TempPersistent].[TempPersistentBaseId] = @id", selectQuery);
        }
        [Test]
        public void LoadTest() {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp = new TempPersistent();
            temp.Name = "Name Of TempPersistent";
            temp.Caption = "Caption Of TempPersistent";
            dbo.SaveOrUpdate(temp);

            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp1 = (TempPersistent)dbo.getFromDb(temp.Id, typeof(TempPersistent));
            Assert.AreEqual(temp.Id, temp1.Id);
            StringAssert.AreEqualIgnoringCase("Name Of TempPersistent", temp1.Name);
            StringAssert.AreEqualIgnoringCase("Caption Of TempPersistent", temp1.Caption);
            Assert.IsNull(temp1.TempPersistentAlone);
        }
        [Test]
        public void LoadIncorrectClassTest() {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp = new TempPersistent();
            temp.Name = "SaveInheritedTest Name";
            temp.Caption = "SaveInheritedTest Caption";
            dbo.SaveOrUpdate(temp);
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            AnotherTempPersistent temp1 = (AnotherTempPersistent)dbo.getFromDb(temp.Id, typeof(AnotherTempPersistent));
            Assert.IsNull(temp1);
        }
        [Test]
        public void LoadAloneTest() {
            DateTime wdate = new DateTime(2012, 12, 24);
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone temp = new TempPersistentAlone();
            temp.Name = "Name Of TempPersistentAlone";
            temp.DateTime = wdate;
            dbo.SaveOrUpdate(temp);

            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone temp1 = (TempPersistentAlone)dbo.getFromDb(temp.Id, typeof(TempPersistentAlone));
            Assert.AreEqual(temp.Id, temp1.Id);
            Assert.IsNull(temp.IntNullable);
            Assert.AreEqual(wdate, temp.DateTime);
            StringAssert.AreEqualIgnoringCase("Name Of TempPersistentAlone", temp1.Name);
        }
        [Test]
        public void SaveWithChildTest() {
            //creating a new child object
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone child = new TempPersistentAlone();
            child.Name = "SaveWithChildTest this is a child";
            dbo.SaveOrUpdate(child);
            //creating a new parent object
            TempPersistent temp = new TempPersistent();
            temp.Name = "SaveWithChildTest Name";
            temp.Caption = "SaveWithChildTest Caption";
            temp.TempPersistentAlone = child;
            dbo.SaveOrUpdate(temp);
            //creating loading and testing
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp1 = (TempPersistent)dbo.getFromDb(temp.Id, typeof(TempPersistent));
            StringAssert.AreEqualIgnoringCase(temp.Name, temp1.Name);
            StringAssert.AreEqualIgnoringCase(temp.Caption, temp1.Caption);
            StringAssert.AreEqualIgnoringCase(temp.TempPersistentAlone.Name, child.Name);
        }
        [Test]
        public void LoadAbstractCollectionTest() {
            //creating a new child object
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone child = new TempPersistentAlone();
            child.Name = "LoadAbstractCollectionTest this is a child";
            dbo.SaveOrUpdate(child);
            //creating a new parent object
            TempPersistent parent1 = new TempPersistent();
            parent1.Name = "LoadAbstractCollectionTest parent Name #1";
            parent1.Caption = "LoadAbstractCollectionTest TempPersistent";
            parent1.TempPersistentAlone = child;
            dbo.SaveOrUpdate(parent1);
            //creating a new parent object
            ThirdLevelTempPersistent parent2 = new ThirdLevelTempPersistent();
            parent2.Name = "SaveWithChildTest parent Name #2";
            parent2.Caption = "SaveWithChildTest ThirdLevelTempPersistent";
            
            parent2.TempPersistentAlone = child;
            dbo.SaveOrUpdate(parent2);
            //creating loading and testing
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone temp1 = (TempPersistentAlone)dbo.getFromDb(child.Id, typeof(TempPersistentAlone));
            Assert.AreEqual(2, temp1.TempPersistentBaseArrayList.Count);
            Assert.AreEqual(typeof(TempPersistent), temp1.TempPersistentBaseArrayList[0].GetType());
            Assert.AreEqual(typeof(ThirdLevelTempPersistent), temp1.TempPersistentBaseArrayList[1].GetType());
        }
        [Test]
        public void SaveAloneTest() {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone temp = new TempPersistentAlone();
            temp.Name = "SaveAloneTest";
            dbo.SaveOrUpdate(temp);
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistentAlone temp1 = (TempPersistentAlone)dbo.getFromDb(temp.Id, typeof(TempPersistentAlone));
            StringAssert.AreEqualIgnoringCase(temp.Name, temp1.Name);
        }
        [Test]
        public void SaveInheritedTest() {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp = new TempPersistent();
            temp.Name = "SaveInheritedTest Name";
            temp.Caption = "SaveInheritedTest Caption";
            dbo.SaveOrUpdate(temp);
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            TempPersistent temp1 = (TempPersistent)dbo.getFromDb(temp.Id, typeof(TempPersistent));
            StringAssert.AreEqualIgnoringCase(temp.Name, temp1.Name);
            StringAssert.AreEqualIgnoringCase(temp.Caption, temp1.Caption);
        }
        [Test]
        public void SaveInheritedThirdLevelTest() {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            ThirdLevelTempPersistent temp = new ThirdLevelTempPersistent();
            temp.Name = "SaveInheritedThirdLevelTest Name";
            temp.Caption = "SaveInheritedThirdLevelTest Caption";
            temp.Description = "SaveInheritedThirdLevelTest Description";
            dbo.SaveOrUpdate(temp);
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            ThirdLevelTempPersistent temp1 = (ThirdLevelTempPersistent)dbo.getFromDb(temp.Id, typeof(ThirdLevelTempPersistent));
            StringAssert.AreEqualIgnoringCase(temp.Name, temp1.Name);
            StringAssert.AreEqualIgnoringCase(temp.Caption, temp1.Caption);
            StringAssert.AreEqualIgnoringCase(temp.Description, temp1.Description);
        }
        [Test]
        public void UpdateInheritedThirdLevelTest() {
            DataBaseObjectsFactorySQLServer dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            ThirdLevelTempPersistent temp = new ThirdLevelTempPersistent();
            temp.Name = "SaveInheritedThirdLevelTest Name";
            temp.Caption = "SaveInheritedThirdLevelTest Caption";
            temp.Description = "SaveInheritedThirdLevelTest Description";
            dbo.SaveOrUpdate(temp);
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            ThirdLevelTempPersistent temp1 = (ThirdLevelTempPersistent)dbo.getFromDb(temp.Id, typeof(ThirdLevelTempPersistent));
            temp1.Name = temp1.Name + " Changed";
            temp1.Caption = temp1.Caption + " Changed";
            temp1.Description = temp1.Description + " Changed";
            dbo.SaveOrUpdate(temp1);
            dbo = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            ThirdLevelTempPersistent temp2 = (ThirdLevelTempPersistent)dbo.getFromDb(temp.Id, typeof(ThirdLevelTempPersistent));
            StringAssert.AreEqualIgnoringCase(temp1.Name, temp2.Name);
            StringAssert.AreEqualIgnoringCase(temp1.Caption, temp2.Caption);
            StringAssert.AreEqualIgnoringCase(temp1.Description, temp2.Description);
        }
        [Test]
        public void OrderChildCollectionTest()
        {
            DataBaseObjectsFactorySQLServer dao = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            Guid g1 = Guid.NewGuid();
            Guid g1l1 = Guid.NewGuid();
            Guid g2l1A = Guid.NewGuid();
            Guid g2l1B = Guid.NewGuid();
            //create MainTestItem
            MainTestItem mti = new MainTestItem();
            mti.GuidNotNull = g1;
            mti.GuidNullable = null;
            mti.OrderIndex = "Root";
            mti.StringNullable = null;
            dao.SaveOrUpdate(mti);
            Assert.AreEqual(1, dao.Cache.Count);
            //1st level child
            //#1
            MainTestItem mti1L1 = new MainTestItem();
            mti1L1.GuidNotNull = g1l1;
            mti1L1.GuidNullable = g1l1;
            mti1L1.OrderIndex = "Alfa";
            mti1L1.StringNullable = "mti1L1";
            mti1L1.Parent = mti;
            dao.SaveOrUpdate(mti1L1);
            //#2
            MainTestItem mti2L1 = new MainTestItem();
            mti2L1.GuidNotNull = g2l1A;
            mti2L1.GuidNullable = g2l1B;
            mti2L1.OrderIndex = "Beta";
            mti2L1.StringNullable = "mti2L1";
            mti2L1.Parent = mti;
            dao.SaveOrUpdate(mti2L1);
            //#3
            MainTestItem mti1L3 = new MainTestItem();
            mti1L3.GuidNotNull = g1l1;
            mti1L3.GuidNullable = g1l1;
            mti1L3.OrderIndex = "Gamma";
            mti1L3.StringNullable = "mti1L3";
            mti1L3.Parent = mti2L1;
            dao.SaveOrUpdate(mti1L3);
            Assert.AreEqual(4, dao.Cache.Count);

            dao = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            Assert.AreEqual(0,dao.Cache.Count);
            MainTestItem root = dao.getFromDb<MainTestItem>(mti.Id);
            Assert.AreEqual(g1, root.GuidNotNull);
            Assert.IsNull(root.GuidNullable);
            StringAssert.AreEqualIgnoringCase("Root", root.OrderIndex);
            Assert.IsNull(root.StringNullable);
            Assert.AreEqual(2, root.Children.Count);
            Assert.IsNull(root.Parent);
            //check sorting is correct (DESC)
            StringAssert.AreEqualIgnoringCase("Beta", ((MainTestItem)root.Children[0]).OrderIndex);
            StringAssert.AreEqualIgnoringCase("Alfa", ((MainTestItem)root.Children[1]).OrderIndex);

        }
        [Test]
        public void ManyToManyLoadTest()
        {
            DataBaseObjectsFactorySQLServer dao = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            //create left side
            string[] itemNames = {"Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6"};
            List<Item> items = itemNames.Select(i => new Item { Name = i }).ToList();
            foreach (Item item in items)
            {
                dao.SaveOrUpdate(item);    
            }
            //create right side
            string[] itemTypeNames = {"Item Type 1", "Item Type 2", "Item Type 3", "Item Type 4", "Item Type 5", "Item Type 6"};
            List<ItemType> itemTypes = itemTypeNames.Select(i=>new ItemType{Name = i}).ToList();
            foreach (ItemType item in itemTypes)
            {
                dao.SaveOrUpdate(item);    
            }
            //create middle
            //1-6
            dao.SaveManyToManyLinkItem(items[0], itemTypes[0]);
            dao.SaveManyToManyLinkItem(items[0], itemTypes[1]);
            dao.SaveManyToManyLinkItem(items[0], itemTypes[2]);
            dao.SaveManyToManyLinkItem(items[0], itemTypes[3]);
            dao.SaveManyToManyLinkItem(items[0], itemTypes[4]);
            dao.SaveManyToManyLinkItem(items[0], itemTypes[5]);
            //2-3
            dao.SaveManyToManyLinkItem(items[1], itemTypes[0]);
            dao.SaveManyToManyLinkItem(items[1], itemTypes[2]);
            dao.SaveManyToManyLinkItem(items[1], itemTypes[4]);
            //3-1 (3 duplicates)
            dao.SaveManyToManyLinkItem(items[2], itemTypes[1]);
            dao.SaveManyToManyLinkItem(items[2], itemTypes[1]);
            dao.SaveManyToManyLinkItem(items[2], itemTypes[1]);
            //kill dao
            dao = new DataBaseObjectsFactorySQLServer(ConnectionSting);
            //test left side
            //#1
            Item item1 = dao.getFromDb<Item>(items[0].Id);
            Assert.IsNotNull(item1);
            Assert.AreEqual(items[0].Id,item1.Id);
            StringAssert.AreEqualIgnoringCase(items[0].Name, item1.Name);
            Assert.AreEqual(6, item1.ItemTypes.Count);
            //#2
            Item item2 = dao.getFromDb<Item>(items[1].Id);
            Assert.IsNotNull(item2);
            Assert.AreEqual(items[1].Id, item2.Id);
            StringAssert.AreEqualIgnoringCase(items[1].Name, item2.Name);
            Assert.AreEqual(3, item2.ItemTypes.Count);
            Assert.AreEqual(itemTypes[4].Id,((ItemType) item2.ItemTypes[2]).Id);
            Assert.AreEqual(itemTypes[2].Id, ((ItemType)item2.ItemTypes[1]).Id);
            Assert.AreEqual(itemTypes[0].Id, ((ItemType)item2.ItemTypes[0]).Id);
            //#3
            Item item3 = dao.getFromDb<Item>(items[2].Id);
            StringAssert.AreEqualIgnoringCase(items[2].Name, item3.Name);
            Assert.AreEqual(1, item3.ItemTypes.Count);//1 expected as duplicates are not allowed
            Assert.AreEqual(itemTypes[1].Id, ((ItemType)item3.ItemTypes[0]).Id);
            
            
            //test right side
            //1
            ItemType itemType1 = dao.getFromDb<ItemType>(itemTypes[0].Id);
            Assert.AreEqual(itemTypes[0].Id, itemType1.Id);
            Assert.AreEqual(2, itemType1.Items.Count);
            Assert.AreEqual(items[0].Id, ((Item)itemType1.Items[0]).Id);
            Assert.AreEqual(items[1].Id, ((Item)itemType1.Items[1]).Id);
            //2
            ItemType itemType2 = dao.getFromDb<ItemType>(itemTypes[1].Id);
            Assert.AreEqual(2, itemType2.Items.Count);
            Assert.AreEqual(items[0].Id, ((Item)itemType2.Items[0]).Id);
            Assert.AreEqual(items[2].Id, ((Item)itemType2.Items[1]).Id);
        }
        
        
    }
}

