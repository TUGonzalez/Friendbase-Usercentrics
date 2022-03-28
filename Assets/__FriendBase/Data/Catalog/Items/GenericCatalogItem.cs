using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Catalog
{
    public class GenericCatalogItem
    {
        public ItemType ItemType { get; }
        public bool ActiveInCatalog { get; set; }
        public int IdItemWebClient { get; }
        public int IdItem { get; }
        public string NameItem { get; set; }
        public string NamePrefab { get; }
        public bool Enable { get; set; }
        public int OrderInCatalog { get; set; }
        public int GoldPrice { get; set; }
        public int GemsPrice { get; set; }
        public CurrencyType CurrencyType { get; set; }

        public GenericCatalogItem(ItemType itemType, int idItemWebClient, int idItem, string nameItem, string namePrefab, int orderInCatalog, bool activeInCatalog, int gemsPrice, int goldPrice, CurrencyType currencyType)
        {
            ItemType = itemType;
            IdItemWebClient = idItemWebClient;
            IdItem = idItem;
            NameItem = nameItem;
            NamePrefab = namePrefab;
            Enable = true;
            OrderInCatalog = orderInCatalog;
            ActiveInCatalog = activeInCatalog;
            GemsPrice = gemsPrice;
            GoldPrice = goldPrice;
            CurrencyType = currencyType;
        }

        public string GetNameFurniturePrefabUIByItem()
        {
            return $"UI_Furnitures[{NamePrefab}]";
        }
    }
}