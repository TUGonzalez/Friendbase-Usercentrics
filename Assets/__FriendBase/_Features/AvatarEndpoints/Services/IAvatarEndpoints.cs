using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data.Catalog;
using Newtonsoft.Json.Linq;
using Data.Bag;
using Data.Users;

public interface IAvatarEndpoints 
{
    IObservable<List<AvatarGenericCatalogItem>> GetAvatarCatalogItemsList();
    IObservable<List<GenericBagItem>> GetPlayerInventory();
    IObservable<JObject> GetAvatarSkin();
    IObservable<JObject> SetAvatarSkin(JObject json);
    IObservable<UserInformation> GetUserInformation();
    IObservable<JObject> PurchaseItem(List<GenericCatalogItem> listItems);
    IObservable<bool> PurchaseValidation(string receiptValidation, string transactionId);
    IObservable<bool> PurchaseValidationGoogle(string productId, string token);
}
