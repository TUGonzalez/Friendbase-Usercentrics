
using Data.Catalog;
using Data.Bag;
using System.Collections.Generic;
using Data.Users;
using Data.Rooms;

namespace Data
{
    public interface IGameData
    {
        GenericCatalog GetCatalogByItemType(ItemType itemType);
        Dictionary<ItemType, AvatarCustomizationRule> GetAvatarCustomizationRules();
        AvatarCustomizationRule GetAvatarCustomizationRuleByItemType(ItemType itemType);
        AvatarGenericCatalogItem GetDefaultAvatarCatalogItem(ItemType itemType);
        ColorCatalogItem GetDefaultColorCatalogItem(ItemType itemType);
        GenericBag GetBagByItemType(ItemType itemType);
        void AddItemsToBag(List<GenericBagItem> items);
        void AddItemToBag(GenericBagItem item);
        void InitializeCatalogs(List<GenericCatalogItem> listItems);
        UserInformation GetUserInformation();
        void AddSkinToInventory(AvatarCustomizationData avatarCustomizationData);
        RoomInformation GetRoomInformation();
        void SetRoomInformation(RoomInformation roomInformation);
    }
}