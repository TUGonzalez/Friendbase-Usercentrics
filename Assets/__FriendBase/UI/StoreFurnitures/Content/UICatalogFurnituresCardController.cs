using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.ScrollView;
using System;
using Data.Catalog;
using TMPro;
using Data.Bag;
using Architecture.Injector.Core;
using Data;

[RequireComponent(typeof(Load2DObject))]
public class UICatalogFurnituresCardController : UIAbstractCardController
{
    [SerializeField] protected Image imgDiamond;
    [SerializeField] protected TextMeshProUGUI txtPrice;
    [SerializeField] protected Image imgInventory;

    private Load2DObject load2dObject;
    public GenericCatalogItem ObjCat { get; private set; }
    public bool ShowPrice { get; set; }

    private GenericBag bagFurnitures;

    protected Action<EventType, GenericCatalogItem, UIAbstractCardController> callback;

    protected virtual void Awake()
    {
        load2dObject = GetComponent<Load2DObject>();
    }

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        ObjCat = (GenericCatalogItem)itemData;
        if (ObjCat == null)
        {
            return;
        }

        string namePrefab = ObjCat.GetNameFurniturePrefabUIByItem();
        load2dObject.Load(namePrefab);

        UpdateGems();

        this.callback = callback;
    }

    void UpdateGems()
    {
        imgDiamond.gameObject.SetActive(ShowPrice);
        imgInventory.gameObject.SetActive(!ShowPrice);
        txtPrice.gameObject.SetActive(true);

        if (ShowPrice)
        {
            txtPrice.text = ObjCat.GemsPrice.ToString();
        }
        else
        {
            bagFurnitures = Injection.Get<IGameData>().GetBagByItemType(ObjCat.ItemType);
            GenericBagItem bagItem = bagFurnitures.GetItemById(ObjCat.IdItem);
            if (bagItem != null)
            {
                txtPrice.text = bagItem.Amount.ToString();
            }
            else
            {
                txtPrice.gameObject.SetActive(false);
            }
        }
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, ObjCat, this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, ObjCat, this);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
