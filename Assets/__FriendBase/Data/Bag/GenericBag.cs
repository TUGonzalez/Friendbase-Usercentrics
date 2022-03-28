using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Architecture.Injector.Core;
using DebugConsole;

namespace Data.Bag
{
    public class GenericBag 
    {
        public bool Ready { get; private set; }
        public ItemType ItemType { get; }
        public List<GenericBagItem> listElements = new List<GenericBagItem>();

        private IDebugConsole debugConsole = Injection.Get<IDebugConsole>();

        public GenericBag(ItemType itemType)
        {
            ItemType = itemType;
            Ready = false;
        }

        public void Initialize(List<GenericBagItem> items)
        {
            if (items == null)
            {
                return;
            }

            listElements = new List<GenericBagItem>();

            int amount = items.Count;
            for (int i = 0; i < amount; i++)
            {
                if (ItemType == items[i].ItemType)
                {
                    listElements.Add(items[i]);
                }
                else
                {
                    debugConsole.ErrorLog("GenericBag:Initialize", "Invalid element:" + items[i].ItemType + " idInstance:" + items[i].IdInstance, "Catalog itemType:" + ItemType);
                }
            }
            
            Ready = true;
        }

        public virtual void AddItem(GenericBagItem item)
        {
            GenericBagItem itemExist = GetItemById(item.ObjCat.IdItem);
            if (itemExist==null)
            {
                //If it does not exist => we add it to the list
                listElements.Insert(0, item);
            }
            else
            {
                //If it does exist => we change amount
                itemExist.Amount += item.Amount;
            }
        }

        public virtual GenericBagItem GetItemById(int IdItem)
        {
            return listElements.Find(item => item.ObjCat.IdItem.Equals(IdItem));
        }

        public virtual GenericBagItem GetItemByIndex(int index)
        {
            if (index < listElements.Count)
            {
                return listElements[index];
            }

            return null;
        }


        public virtual int GetAmountItems()
        {
            return listElements.Count;
        }

        //public virtual void RemoveItem(string idInstance)
        //{
        //    listElements.Remove(GetItemById(idInstance));
        //}

        public virtual bool HasAnyIdItem(int idItem)
        {
            return listElements.Exists(item => item.ObjCat.IdItem == idItem);
        }
    }
}