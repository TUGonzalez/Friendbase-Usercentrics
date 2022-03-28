using UnityEngine;

namespace Data.Catalog
{
    public class ColorCatalogItem : GenericCatalogItem
    {
        public Color Color { get;}

        public ColorCatalogItem(ItemType itemType, int idItemWebClient, int idItem, string nameItem, string namePrefab, int orderInCatalog, bool activeInCatalog, int gemsPrice, int goldPrice, CurrencyType currencyType, string color)
            : base(itemType, idItemWebClient, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog, gemsPrice, goldPrice, currencyType)
        {
            Color = HexToColor(color);
        }

        Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}