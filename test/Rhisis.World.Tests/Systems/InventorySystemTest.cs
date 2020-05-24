using Moq;
using Rhisis.Core.Data;
using Rhisis.Database.Entities;
using Rhisis.Testing.Abstract;
using Rhisis.World.Game.Components;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Entities.Internal;
using Rhisis.World.Game.Factories;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Tests.Mocks.Database.Entities;
using Rhisis.World.Tests.Mocks.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rhisis.World.Tests.Systems
{
    public sealed class InventorySystemTest : ServiceTestBase<InventorySystem>
    {
        private readonly IPlayerEntity _player;
        private readonly IItemFactory _itemFactory;

        private readonly Mock<IInventoryPacketFactory> _inventoryPacketFactoryMock;

        private readonly IEnumerable<DbItem> _databaseItems;
        private readonly IEnumerable<DbInventoryItem> _databaseInventoryItems;

        public InventorySystemTest()
        {
            _inventoryPacketFactoryMock = new Mock<IInventoryPacketFactory>();
            _itemFactory = new ItemFactoryMock();
            _player = new PlayerEntity(null)
            {
                PlayerData = new PlayerDataComponent
                {
                    Id = 1
                }
            };

            Database.Users.Add(new UserEntityGenerator().Generate(1).First());
            Database.SaveChanges();

            Database.Characters.Add(new CharacterGenerator().Generate(1).First());
            Database.SaveChanges();

            _databaseItems = new ItemEntityGenerator().Generate(_player.Inventory.MaxCapacity);
            Database.Items.AddRange(_databaseItems);
            Database.SaveChanges();

            _databaseInventoryItems = new InventoryItemGenerator(1, _databaseItems)
                .Generate(Faker.Random.Int(10, 30));
            Database.InventoryItems.AddRange(_databaseInventoryItems);
            Database.SaveChanges();

            Service = new InventorySystem(LoggerMock.Object, Database, _itemFactory, _inventoryPacketFactoryMock.Object, null, null, null);
        }

        [Fact]
        public void InitializeInventoryTest()
        {
            Service.Initialize(_player);

            IEnumerable<DbInventoryItem> dbInventoryItems = _databaseInventoryItems.Where(x => x.CharacterId == _player.PlayerData.Id && !x.Item.IsDeleted);

            Assert.NotNull(_player.Inventory);
            Assert.Equal(dbInventoryItems.Count(), _player.Inventory.GetItemCount());

            foreach (DbInventoryItem dbInventoryItem in dbInventoryItems)
            {
                InventoryItem inventoryItem = _player.Inventory.GetItemAtSlot(dbInventoryItem.Slot);

                Assert.NotNull(inventoryItem);
                Assert.Equal(dbInventoryItem.Item.ItemId, inventoryItem.Id);
                Assert.Equal(dbInventoryItem.Slot, inventoryItem.Slot);
                Assert.Equal(dbInventoryItem.Quantity, inventoryItem.Quantity);
                Assert.Equal(dbInventoryItem.Item.Refine, inventoryItem.Refine);
                Assert.Equal(dbInventoryItem.Item.Element, (byte)inventoryItem.Element);
                Assert.Equal(dbInventoryItem.Item.ElementRefine, inventoryItem.ElementRefine);
            }
        }

        [Fact]
        public void MoveNonStackableItemsTest()
        {
            Service.Initialize(_player);

            int sourceSlot = GetRandomSlotInUse();
            int destinationSlot = GetRandomSlotInUse(sourceSlot);
            InventoryItem sourceItem = _player.Inventory.GetItemAtSlot(sourceSlot);
            InventoryItem destinationItem = _player.Inventory.GetItemAtSlot(destinationSlot);

            Service.MoveItem(_player, (byte)sourceSlot, (byte)destinationSlot);

            Assert.Equal(destinationSlot, sourceItem.Slot);

            if (destinationItem != null)
            {
                Assert.Equal(sourceSlot, destinationItem.Slot);
            }

            InventoryItem afterMoveSourceItem = _player.Inventory.GetItemAtSlot(sourceSlot);
            InventoryItem afterMoveDestinationItem = _player.Inventory.GetItemAtSlot(destinationSlot);

            Assert.Equal(afterMoveSourceItem, destinationItem);
            Assert.Equal(afterMoveDestinationItem, sourceItem);

            _inventoryPacketFactoryMock.Verify(x => x.SendItemMove(_player, (byte)sourceSlot, (byte)destinationSlot), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MoveStackableItemsTest(bool shouldSourceDeleteItem)
        {
            Service.Initialize(_player);

            int sourceSlot = GetRandomSlotInUse();
            int destinationSlot = GetRandomSlotInUse(sourceSlot);
            InventoryItem sourceItem = _player.Inventory.GetItemAtSlot(sourceSlot);
            InventoryItem destinationItem = _player.Inventory.GetItemAtSlot(destinationSlot);

            if (sourceItem == null)
            {
                Assert.Throws<InvalidOperationException>(() => Service.MoveItem(_player, (byte)sourceSlot, (byte)destinationSlot));
                return;
            }

            const int itemPackMax = 1000;
            const int sourceItemQuantity = 500;
            var destinationItemQuantity = shouldSourceDeleteItem ? 100 : 600;

            sourceItem = _itemFactory.CreateInventoryItem(1, 0, ElementType.None, 0);
            sourceItem.Data.PackMax = itemPackMax;
            sourceItem.Quantity = sourceItemQuantity;
            sourceItem.Slot = sourceSlot;
            _player.Inventory.SetItemAtSlot(sourceItem, sourceSlot);

            destinationItem = _itemFactory.CreateInventoryItem(1, 0, ElementType.None, 0);
            destinationItem.Data.PackMax = itemPackMax;
            destinationItem.Quantity = destinationItemQuantity;
            destinationItem.Slot = destinationSlot;
            _player.Inventory.SetItemAtSlot(destinationItem, destinationSlot);

            Service.MoveItem(_player, (byte)sourceSlot, (byte)destinationSlot);

            if (shouldSourceDeleteItem)
            {
                Assert.Null(_player.Inventory.GetItemAtSlot(sourceSlot));
                Assert.NotNull(destinationItem);
                Assert.Equal(sourceItemQuantity + destinationItemQuantity, destinationItem.Quantity);
                _inventoryPacketFactoryMock.Verify(x => x.SendItemUpdate(_player, UpdateItemType.UI_NUM, destinationItem.UniqueId, destinationItem.Quantity), Times.Once());
            }
            else
            {
                Assert.Equal(destinationItemQuantity - sourceItemQuantity, sourceItem.Quantity);
                Assert.Equal(itemPackMax, destinationItem.Quantity);
                _inventoryPacketFactoryMock.Verify(x => x.SendItemUpdate(_player, UpdateItemType.UI_NUM, sourceItem.UniqueId, sourceItem.Quantity), Times.Once());
                _inventoryPacketFactoryMock.Verify(x => x.SendItemUpdate(_player, UpdateItemType.UI_NUM, destinationItem.UniqueId, destinationItem.Quantity), Times.Once());
            }
        }

        [Fact]
        public void MoveItemToSameSlotTest()
        {
            Service.Initialize(_player);

            int sourceSlot = GetRandomSlotInUse();

            Assert.Throws<InvalidOperationException>(() => Service.MoveItem(_player, (byte)sourceSlot, (byte)sourceSlot));
        }

        [Fact]
        public void MoveUndefinedItemTest()
        {
            Service.Initialize(_player);

            int sourceSlot = GetRandomSlotNotUsed();
            int destinationSlot = GetRandomSlot();

            Assert.Throws<InvalidOperationException>(() => Service.MoveItem(_player, (byte)sourceSlot, (byte)destinationSlot));
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(int.MaxValue, 0)]
        [InlineData(0, int.MaxValue)]
        [InlineData(InventoryContainerComponent.MaxInventoryItems, 0)]
        [InlineData(0, InventoryContainerComponent.MaxInventoryItems)]
        public void MoveItemWithInvalidSlotsTest(int sourceSlot, int destinationSlot)
        {
            Service.Initialize(_player);

            Assert.Throws<InvalidOperationException>(() => Service.MoveItem(_player, (byte)sourceSlot, (byte)destinationSlot));
        }

        [Fact]
        public void DeleteItemTest()
        {
            Service.Initialize(_player);

            int itemToDeleteSlot = GetRandomSlotInUse();
            InventoryItem itemToDelete = _player.Inventory.GetItemAtSlot(itemToDeleteSlot);
            int itemUniqueId = itemToDelete.UniqueId;
            int deleteQuantity = itemToDelete.Quantity;

            int deletedAmount = Service.DeleteItem(_player, itemUniqueId, deleteQuantity);
            InventoryItem deletedItem = _player.Inventory.GetItemAtSlot(itemToDeleteSlot);

            Assert.Equal(deleteQuantity, deletedAmount);
            Assert.Null(_player.Inventory.GetItemAtSlot(itemToDeleteSlot));

            _inventoryPacketFactoryMock.Verify(x => x.SendItemUpdate(_player, UpdateItemType.UI_NUM, itemUniqueId, 0), Times.Once());
        }

        [Fact]
        public void DeleteItemQuantityTest()
        {
            Service.Initialize(_player);

            int itemToDeleteSlot = GetRandomSlotInUse();
            InventoryItem itemToDelete = _player.Inventory.GetItemAtSlot(itemToDeleteSlot);
            int itemUniqueId = itemToDelete.UniqueId;
            int itemInitialQuantity = itemToDelete.Quantity;
            int deleteQuantity = itemInitialQuantity / 2;

            int deletedAmount = Service.DeleteItem(_player, itemUniqueId, deleteQuantity);

            Assert.Equal(deleteQuantity, deletedAmount);
            Assert.NotNull(_player.Inventory.GetItemAtSlot(itemToDeleteSlot));

            _inventoryPacketFactoryMock.Verify(x => x.SendItemUpdate(_player, UpdateItemType.UI_NUM, itemUniqueId, itemInitialQuantity - deleteQuantity), Times.Once());
        }

        [Fact]
        public void DeleteUndefinedItemTest()
        {
            Service.Initialize(_player);

            int undefinedItemUniqueId = GetRandomSlotNotUsed();

            Assert.Throws<ArgumentNullException>(() => Service.DeleteItem(_player, undefinedItemUniqueId, 1));
        }

        [Fact]
        public void DeleteZeroQuantityItemTest()
        {
            Service.Initialize(_player);

            int itemToDeleteSlot = GetRandomSlotInUse();
            InventoryItem itemToDelete = _player.Inventory.GetItemAtSlot(itemToDeleteSlot);

            int deletedAmount = Service.DeleteItem(_player, itemToDelete.UniqueId, 0);

            Assert.Equal(0, deletedAmount);
        }

        private int GetRandomSlotInUse(int expectSlot = -1)
        {
            int slot;

            do
            {
                slot = _player.Inventory.GetItemAtSlot(GetRandomSlot())?.Slot ?? -1;

                if (expectSlot != -1 && slot == expectSlot)
                {
                    slot = -1;
                }
            } while (slot == -1);

            return slot;
        }

        private int GetRandomSlotNotUsed()
        {
            int slot = GetRandomSlot();
            
            while (_player.Inventory.GetItemAtSlot(slot) != null)
            {
                slot = GetRandomSlot();
            }

            return slot;
        }

        private int GetRandomSlot() => Faker.Random.Byte(0, InventoryContainerComponent.InventorySize);
    }
}
