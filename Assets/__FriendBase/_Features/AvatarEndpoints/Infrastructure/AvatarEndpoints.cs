using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data.Catalog;
using AuthFlow.Firebase.Core.Actions;
using WebClientTools.Core.Services;
using UniRx;
using Newtonsoft.Json.Linq;
using Web;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Bag;
using Data.Catalog.Items;
using Architecture.Injector.Core;
using Data.Users;
using DebugConsole;

public class AvatarEndpoints : IAvatarEndpoints
{
    readonly GetFirebaseUid getFirebaseUid;
    readonly IWebHeadersBuilder headersBuilder;

    private IItemTypeUtils itemTypeUtils;
    private IGameData gameData;
    private IDebugConsole debugConsole;

    public AvatarEndpoints(GetFirebaseUid getFirebaseUid, IWebHeadersBuilder headersBuilder)
    {
        this.getFirebaseUid = getFirebaseUid;
        this.headersBuilder = headersBuilder;

        gameData = Injection.Get<IGameData>();
        debugConsole = Injection.Get<IDebugConsole>();
        itemTypeUtils = Injection.Get<IItemTypeUtils>();
    }

    public IObservable<List<GenericBagItem>> GetPlayerInventory() => GetPlayerInventoryAsync().ToObservable().ObserveOnMainThread();
    async Task<List<GenericBagItem>> GetPlayerInventoryAsync()
    {
        var userId = await getFirebaseUid.Execute();
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/users/{userId}/player-inventory-items";

        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToGenericBagItem(response.json);
    }

    List<GenericBagItem> ToGenericBagItem(JObject jObject)
    {
        List<GenericBagItem> listItemBags = new List<GenericBagItem>();

        foreach (JObject itemData in jObject["data"])
        {
            try
            {
                int idInstance = itemData["id"].Value<int>();
                int count = itemData["count"].Value<int>();
                JObject itemJson = itemData["item"].Value<JObject>();
                int idItem = itemJson["id_in_game"].Value<int>();
                string sItemType = itemJson["type"].Value<string>();
                ItemType itemType = itemTypeUtils.GetItemTypeByName(sItemType);

                GenericCatalog catalog = gameData.GetCatalogByItemType(itemType);

                if (catalog != null)
                {
                    GenericCatalogItem objCat = catalog.GetItem(idItem);
                    if (objCat != null)
                    {
                        listItemBags.Add(new GenericBagItem(itemType, idInstance.ToString(), count, objCat));
                    }
                    else
                    {
                        debugConsole.ErrorLog("AvatarEndpoints:ToGenericBagItem", "idItem Not Found", $"itemType:{itemType} idItem:{idItem}");
                    }
                }
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("AvatarEndpoints:ToGenericBagItem", "Exception", "Invalid Json Data");
            }
        }
        return listItemBags;
    }

    public IObservable<List<AvatarGenericCatalogItem>> GetAvatarCatalogItemsList() => GetAvatarCatalogItemsListAsync().ToObservable().ObserveOnMainThread();
    async Task<List<AvatarGenericCatalogItem>> GetAvatarCatalogItemsListAsync()
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.ItemsEndPoint;
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToAvatarGenericCatalogItem(response.json);
    }

    List<AvatarGenericCatalogItem> ToAvatarGenericCatalogItem(JObject jObject)
    {
        List<AvatarGenericCatalogItem> listItems = new List<AvatarGenericCatalogItem>();

        foreach (JObject catalogItem in jObject["data"])
        {
            try
            {
                //Item
                int id = catalogItem["id"].Value<int>();
                int idItem = catalogItem["id_in_game"].Value<int>();
                string nameItem = catalogItem["name"].Value<string>();
                string namePrefab = catalogItem["name_prefab"].Value<string>();
                string sItemType = catalogItem["type"].Value<string>();
                ItemType itemType = itemTypeUtils.GetItemTypeByName(sItemType);
                string sLayers = catalogItem["layers"].Value<string>();
                int[] layers = null;
                if (!string.IsNullOrEmpty(sLayers))
                {
                    layers = System.Array.ConvertAll(sLayers.Split(','), int.Parse);
                }

                string gender = catalogItem["gender"].Value<string>();

                int orderInCatalog = 0;
                bool activeInCatalog = true;
                int gemsPrice = catalogItem["gems"].Value<int>();
                int goldPrice = catalogItem["gold"].Value<int>();
                CurrencyType currencyType = gemsPrice > 0 ? CurrencyType.GEM_PRICE : CurrencyType.GOLD_PRICE;

                if (itemType == ItemType.BODY)
                {
                    listItems.Add(new BodyCatalogItem(itemType, id, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog,
                        gemsPrice, goldPrice, currencyType, layers, gender.Equals("female")));
                }
                else
                {
                    listItems.Add(new AvatarGenericCatalogItem(itemType, id, idItem, nameItem, namePrefab, orderInCatalog,
                        activeInCatalog, gemsPrice, goldPrice, currencyType, layers));
                }
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("AvatarEndpoints:ToAvatarGenericCatalogItem", "Exception", "Invalid Json Data");
            }
        }
        return listItems;
    }

    public IObservable<JObject> GetAvatarSkin() => GetAvatarSkinAsync().ToObservable().ObserveOnMainThread();
    async Task<JObject> GetAvatarSkinAsync()
    {
        var userId = await getFirebaseUid.Execute();
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = $"{Constants.ApiRoot}/avatar/{userId}";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return response.json;
    }

    public IObservable<JObject> SetAvatarSkin(JObject json) => SetAvatarSkinAsync(json).ToObservable().ObserveOnMainThread();
    async Task<JObject> SetAvatarSkinAsync(JObject json)
    {
        try
        {
            var userId = await getFirebaseUid.Execute();
            var bearerTokenHeader = await headersBuilder.BearerToken;
            var endpoint = $"{Constants.ApiRoot}/avatar/{userId}";

            var response = await WebClient.Put(endpoint, json, true, bearerTokenHeader);

            return response.json;
        }
        catch(Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:SetAvatarSkinAsync", "Endpoint Failed", "");
        }
        return new JObject();
    }

    public IObservable<JObject> PurchaseItem(List<GenericCatalogItem> listItems) => PurchaseItemAsync(listItems).ToObservable().ObserveOnMainThread();
    async Task<JObject> PurchaseItemAsync(List<GenericCatalogItem> listItems)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.PurchaseEndpoint;

        var userId = gameData.GetUserInformation().UserId;

        JObject jsonPurchases = new JObject();
        jsonPurchases["user_id"] = userId;

        JArray jArray = new JArray();
        foreach (GenericCatalogItem item in listItems)
        {
            JObject jItem = new JObject();
            jItem["cost"] = item.GemsPrice;
            jItem["price_type"] = "gems";
            jItem["item_id"] = item.IdItemWebClient;

            jArray.Add(jItem);
        }

        JProperty listPurchases = new JProperty("purchases", jArray);
        jsonPurchases.Add(listPurchases);

        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);
        return response.json;
    }

    public IObservable<UserInformation> GetUserInformation() => GetUserInformationAsync().ToObservable().ObserveOnMainThread();
    async Task<UserInformation> GetUserInformationAsync()
    {
        var userId = await getFirebaseUid.Execute();
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = $"{Constants.ApiRoot}/users/{userId}";

        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);
        return ToUserInformation(response.json);
    }

    UserInformation ToUserInformation(JObject jObject)
    {
        UserInformation userInformation = new UserInformation();

        JObject userJson = jObject["data"].Value<JObject>();

        try
        {
            int idUser = userJson["id"].Value<int>();
            string idFirebase = userJson["firebase_uid"].Value<string>();
            string username = userJson["username"].Value<string>();
            int gold = userJson["gold"].Value<int>();
            int gems = userJson["gems"].Value<int>();
            string country = userJson["country"].Value<string>();
            string email = userJson["email"].Value<string>();
            string gender = userJson["gender"].Value<string>();
            string photo_url = userJson["photo_url"].Value<string>();

            userInformation.UserId = idUser;
            userInformation.UserName = username;
            userInformation.FirebaseId = idFirebase;
            userInformation.Gems = gems;
            userInformation.Gold = gold;
            userInformation.Country = country;
            userInformation.Email = email;
            userInformation.Gender = gender;
            userInformation.Photo_url = photo_url;
        }
        catch (Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:ToAvatarGenericCatalogItem", "Exception", "Invalid Json Data");
        }

        return userInformation;
    }

    public IObservable<bool> PurchaseValidation(string receiptValidation, string transactionId) => PurchaseValidationAsync(receiptValidation, transactionId).ToObservable().ObserveOnMainThread();
    async Task<bool> PurchaseValidationAsync(string receiptValidation, string transactionId)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.VerifyApplePurchaseEndpoint;

        var userId = await getFirebaseUid.Execute();

        JObject jsonPurchases = new JObject();
        JObject jsonReceipt = new JObject();
        jsonReceipt["receipt"] = receiptValidation;
        jsonReceipt["user_id"] = userId;
        jsonReceipt["transaction_id"] = transactionId;

        jsonPurchases["purchase"] = jsonReceipt;

        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);

        JObject jsonResponse = response.json;
        int status = jsonResponse["status"].Value<int>();

        return status==0;
    }

    public IObservable<bool> PurchaseValidationGoogle(string productId, string token) => PurchaseValidationGoogleAsync(productId, token).ToObservable().ObserveOnMainThread();
    async Task<bool> PurchaseValidationGoogleAsync(string productId, string token)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.VerifyGooglePurchaseEndpoint;

        var userId = await getFirebaseUid.Execute();

        JObject jsonPurchases = new JObject();
        JObject jsonReceipt = new JObject();
        jsonReceipt["product_id"] = productId;
        jsonReceipt["user_id"] = userId;
        jsonReceipt["token"] = token;

        jsonPurchases["purchase"] = jsonReceipt;

        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);

        JObject jsonResponse = response.json;

        int status = jsonResponse["purchaseState"].Value<int>();

        return status == 0;
    }
}
