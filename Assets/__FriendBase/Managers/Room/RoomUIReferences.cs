using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomUIReferences : MonoBehaviour
{
    public enum UI_TYPES
    {
        BtnAvatar,
        BtnChat,
        BtnBurger,
        BtnRooms,
        BrtnFurnitures,
        BtnHome,
        GemsHolder
    }

    [SerializeField] public UIStoreManager UIStoreManager;
    [SerializeField] public UIStoreFurnituresManager uIStoreFurnitures;
    [SerializeField] public UIRoomListManager UIRoomListPanel;
    [SerializeField] public Button AvatarButton;
    [SerializeField] public ChatView.View.ChatView Chat;
    [SerializeField] public Button BtnChat;
    [SerializeField] public Button BtnBurger;
    [SerializeField] public Button BtnRooms;
    [SerializeField] public Button BtnFurnitures;
    [SerializeField] public Button BtnHome;
    [SerializeField] public Button BtnAddGems;
    [SerializeField] public GameObject GemsHolder;

    private bool isPublicRoom;

    public void Initialize(bool isPublicRoom)
    {
        this.isPublicRoom = isPublicRoom;

        BtnFurnitures.onClick.AddListener(OpenStoreFurnituresPanel);
        BtnAddGems.onClick.AddListener(OpenStorePanel);
        BtnRooms.onClick.AddListener(OpenRoomListPanel);

        UIStoreFurnituresManager.OnOpenEvent += OnOpenFurnituresStore;
        UIStoreFurnituresManager.OnCloseEvent += OnCloseFurnituresStore;

        BtnChat.onClick.AddListener(OnShowChat);
        ChatView.View.ChatView.OnCloseChat += OnCloseChat;

        ShowAll();
    }

    public void ShowAll()
    {
        AvatarButton.gameObject.SetActive(true);
        BtnChat.gameObject.SetActive(isPublicRoom);
        Chat.gameObject.SetActive(isPublicRoom);
        BtnBurger.gameObject.SetActive(true);
        BtnRooms.gameObject.SetActive(true);
        BtnFurnitures.gameObject.SetActive(!isPublicRoom);
        BtnHome.gameObject.SetActive(isPublicRoom);
        GemsHolder.SetActive(true);
    }

    public void HideAll()
    {
        AvatarButton.gameObject.SetActive(false);
        BtnChat.gameObject.SetActive(false);
        BtnBurger.gameObject.SetActive(false);
        BtnRooms.gameObject.SetActive(false);
        BtnFurnitures.gameObject.SetActive(false);
        BtnHome.gameObject.SetActive(false);
        GemsHolder.SetActive(false);
    }

    public void ShowUiElement(UI_TYPES uiElementType)
    {
        string nameUiElement = Enum.GetName(typeof(UI_TYPES), uiElementType);
        GameObject uiElement = GameObject.Find(nameUiElement);
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }

    public void HideUiElement(UI_TYPES uiElementType)
    {
        string nameUiElement = Enum.GetName(typeof(UI_TYPES), uiElementType);
        GameObject uiElement = GameObject.Find(nameUiElement);
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        BtnFurnitures.onClick.RemoveListener(OpenStoreFurnituresPanel);
        BtnAddGems.onClick.RemoveListener(OpenStorePanel);
        BtnRooms.onClick.RemoveListener(OpenRoomListPanel);

        UIStoreFurnituresManager.OnOpenEvent -= OnOpenFurnituresStore;
        UIStoreFurnituresManager.OnCloseEvent -= OnCloseFurnituresStore;

        BtnChat.onClick.RemoveListener(OnShowChat);
        ChatView.View.ChatView.OnCloseChat -= OnCloseChat;
    }

    public void OpenStorePanel()
    {
        UIStoreManager.Open();
    }

    public void CloseStorePanel()
    {
        UIStoreManager.Close();
    }

    public void OpenStoreFurnituresPanel()
    {
        uIStoreFurnitures.Open();
    }

    public void CloseStoreFurnituresPanel()
    {
        uIStoreFurnitures.Close();
    }

    public void OpenRoomListPanel()
    {
        UIRoomListPanel.Open();
    }

    public void CloseRoomListPanel()
    {
        UIRoomListPanel.Close();
    }

    void OnOpenFurnituresStore()
    {
        HideAll();
    }

    void OnCloseFurnituresStore()
    {
        ShowAll();
    }
    
    private void OnCloseChat()
    {
        ShowUiElement(UI_TYPES.GemsHolder);
    }
    
    private void OnShowChat()
    {
        HideUiElement(UI_TYPES.GemsHolder);
        Chat.ShowChat();
    }
}