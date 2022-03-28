using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using Data.Users;
using UI.ScrollView;
using UI.TabController;
using UnityEngine;
using UniRx;
using Newtonsoft.Json.Linq;
using Data.Bag;

public class UIStoreFurnituresManager : AbstractUIPanel
{
    public delegate void OnOpen();
    public static event OnOpen OnOpenEvent;

    public delegate void OnClose();
    public static event OnClose OnCloseEvent;

    [SerializeField] private TabManager principalTabManager;
    [SerializeField] private TabManager secondaryTabManager;
    [SerializeField] private GameObject secondaryTabsContainer;
    [SerializeField] private UICatalogFurnituresScrollView furnitureScrollView;
    [SerializeField] private UICatalogRoomsScrollView roomsScrollView;
    [SerializeField] private GameObject loaderPanel;
    [SerializeField] private GameObject panelFurnitures;
    [SerializeField] private GameObject panelRooms;
    [SerializeField] private UIPanelBuyFurnitures panelBuyFurnitures;
    [SerializeField] private UIDialogPanel dialogPanel;
    [SerializeField] private UIStoreManager storeManager;

    private List<ItemType> itemTypesSecondaryTabs = new List<ItemType> { ItemType.CHAIR, ItemType.FLOOR, ItemType.LAMP, ItemType.TABLE, ItemType.FURNITURES_INVENTORY };

    private IGameData gameData;
    private IAvatarEndpoints avatarEndpoints;
    private UserInformation userInformation;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        userInformation = gameData.GetUserInformation();

        principalTabManager.OnTabSelected += OnPrincipalTabSelected;
        secondaryTabManager.OnTabSelected += OnSecondaryTabSelected;
        furnitureScrollView.OnCardSelected += OnFurnitureCardSelected;
        roomsScrollView.OnCardSelected += OnRoomCardSelected;

        panelBuyFurnitures.OnBuyButtonPressed += OnBuyConfirmed;
    }

    void OnDestroy()
    {
        principalTabManager.OnTabSelected -= OnPrincipalTabSelected;
        secondaryTabManager.OnTabSelected -= OnSecondaryTabSelected;
        furnitureScrollView.OnCardSelected -= OnFurnitureCardSelected;
        roomsScrollView.OnCardSelected -= OnRoomCardSelected;

        panelBuyFurnitures.OnBuyButtonPressed -= OnBuyConfirmed;
    }

    public override void Open()
    {
        base.Open();
        loaderPanel.SetActive(false);
        ShowSecondaryTabs();
        principalTabManager.SetTab(0);
        if (OnOpenEvent!=null)
        {
            OnOpenEvent();
        }
    }

    public override void Close()
    {
        base.Close();
        if (OnCloseEvent != null)
        {
            OnCloseEvent();
        }
    }

    void OnPrincipalTabSelected(int index)
    {
        if (index==0)
        {
            //Show Furnitures Panel and tabs
            panelFurnitures.gameObject.SetActive(true);
            secondaryTabsContainer.SetActive(true);
            secondaryTabManager.SetTab(0);
            //Hide Rooms  Panel
            panelRooms.SetActive(false);
        }
        else
        {
            //Hide Furnitures Panel and tabs
            panelFurnitures.gameObject.SetActive(false);
            secondaryTabsContainer.SetActive(false);

            //Show rooms panel
            panelRooms.SetActive(true);
            roomsScrollView.ShowObjects();
        }
    }

    //------------------------------------
    //--------- SECONDARY TABS -----------
    //------------------------------------

    void OnSecondaryTabSelected(int index)
    {
        UICatalogAvatarTabController tabItem = secondaryTabManager.GetTabByIndex(index) as UICatalogAvatarTabController;
        furnitureScrollView.ShowObjects(tabItem.ItemType);
    }

    void ShowSecondaryTabs()
    {
        HideSecondaryTabs();
        int amount = itemTypesSecondaryTabs.Count;
        for (int i = 0; i < amount; i++)
        {
            UICatalogAvatarTabController tabItem = secondaryTabManager.GetTabByIndex(i) as UICatalogAvatarTabController;
            tabItem.gameObject.SetActive(true);
            tabItem.SetTab(itemTypesSecondaryTabs[i]);
        }
    }

    void HideSecondaryTabs()
    {
        secondaryTabManager.HideAllTabs();
        secondaryTabManager.UnselectAllTabs();
    }

    //------------------------------
    //--------- BUY ITEM -----------
    //------------------------------

    void OnRoomCardSelected(GenericCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        UICatalogRoomsCardController card = cardController as UICatalogRoomsCardController;
        if (card.ShowPrice)
        {
            //If it is an item to buy
            Close();
            panelBuyFurnitures.Open();
            panelBuyFurnitures.CreateRoomsToBuy(new List<GenericCatalogItem> { catalogItem });
        }
        else
        {
            //It is an item from the inventory
        }
    }

    void OnFurnitureCardSelected(GenericCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        UICatalogFurnituresCardController card = cardController as UICatalogFurnituresCardController;
        if (card.ShowPrice)
        {
            //If it is an item to buy
            Close();
            panelBuyFurnitures.Open();
            panelBuyFurnitures.CreateItemsToBuy(new List<GenericCatalogItem> { catalogItem });
        }
        else
        {
            //It is an item from the inventory
        }
    }

    void OnBuyConfirmed()
    {
        if (userInformation.Gems >= panelBuyFurnitures.TotalPrice)
        {
            avatarEndpoints.PurchaseItem(panelBuyFurnitures.GetListOfItemsToBuy())
           .Subscribe(listItemsBought =>
           {
               userInformation.Gems -= panelBuyFurnitures.TotalPrice;

               //Add items to my inventory
               List<GenericCatalogItem> listItems = panelBuyFurnitures.GetListOfItemsToBuy() as List<GenericCatalogItem>;
               foreach (GenericCatalogItem item in listItems)
               {
                   gameData.AddItemToBag(new GenericBagItem(item.ItemType, item.IdItem.ToString(), 1, item));
               }

               panelBuyFurnitures.Close();
           });
        }
        else
        {
            string txtTitle = "You need more Gems!";
            string txtDesc = $"You can't pay for some of the items. Looks like you need {panelBuyFurnitures.TotalPrice - gameData.GetUserInformation().Gems} more Gems.";
            string txtBtnAccept = "Get more Gems";
            string txtBtnDiscard = "Cancel";
            panelBuyFurnitures.Close();
            dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
            {
                storeManager.Open();
            });
        }
    }
}
