using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using Audio.Music;
using AuthFlow.EndAuth.Repo;
using AuthFlow.AboutYou.Core.Services;
using DeepLink.Delivery;
using Firebase;
using Firebase.Auth;
using JetBrains.Annotations;
using LoadingScreen;
using MemoryStorage.Core;
using Shared.Utils;
using Socket;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Data;
using Newtonsoft.Json;
using Data.Catalog;
using System.Linq;
using Data.Users;
using AuthFlow;
using UnityEngine.SceneManagement;
using Data.Rooms;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MainMenu
{
    [Serializable]
    public class MenuButtonInfo
    {
        public string name;
        public bool isActive = true;

        [Tooltip("this will be called before moving towards outPort (if there is)")]
        public UnityEvent onClick;
    }

    public class MainMenuView : ViewNode
    {
        public List<MenuButtonInfo> buttons;
        public RectTransform container;

        IViewManager viewManager;
        Button menuButtonPrefab;
        ISocketManager socketManager;
        IMemoryStorage memoryStorage;
        IMusicPlayer musicPlayer;
        private IGameData gameData;

        private static bool flagAvatarEndpointsReady = false;

        protected override void OnInit()
        {
            Injection.Get(out musicPlayer);
            Injection.Get(out viewManager);
            Injection.Get(out socketManager);
            Injection.Get(out memoryStorage);
            
            gameData = Injection.Get<IGameData>();
        }

        public void GoToOnboarding()
        {
            SceneManager.LoadScene("Onboarding", LoadSceneMode.Additive);
            HideView();
        }

        public void GoToRoomScene(int id)
        {
            switch (id)
            {
                case 0:
                    gameData.SetRoomInformation(new RoomInformation(roomIdInstance: "7334bdf6-f63d-49ec-a37a-2a025172b789", roomName:"Rio", amountUsers:10, roomId:178, namePrefab:"Supermarket", isEnable:true, playerLimit:100, roomRank:10, roomType:RoomType.PRIVATE));
                    break;
                case 1:
                    gameData.SetRoomInformation(new RoomInformation(roomIdInstance: "7334bdf6-f63d-49ec-a37a-2a025172b789", roomName: "Supermarket", amountUsers: 10, roomId: 178, namePrefab: "Supermarket", isEnable: true, playerLimit: 100, roomRank: 10, roomType: RoomType.PUBLIC));
                    break;
                case 2:
                    gameData.SetRoomInformation(new RoomInformation(roomIdInstance: "8992d3cf-cef7-4c79-ad75-596bcd2cb874", roomName: "School", amountUsers: 10, roomId: 178, namePrefab: "School", isEnable: true, playerLimit: 100, roomRank: 10, roomType: RoomType.PUBLIC));
                    break;
                case 3:
                    gameData.SetRoomInformation(new RoomInformation(roomIdInstance: "afdb0740-eec7-4578-8909-e687fc1ece50", roomName: "Space", amountUsers: 10, roomId: 178, namePrefab: "Space", isEnable: true, playerLimit: 100, roomRank: 10, roomType: RoomType.PUBLIC));
                    break;
                case 4:
                    gameData.SetRoomInformation(new RoomInformation(roomIdInstance: "63e8c727-e1e4-40dd-a5e3-d5ac8c31e883", roomName: "Park", amountUsers: 10, roomId: 178, namePrefab: "Park", isEnable: true, playerLimit: 100, roomRank: 10, roomType: RoomType.PUBLIC));
                    break;
                case 5:
                    gameData.SetRoomInformation(new RoomInformation(roomIdInstance: "16d1149f-8b14-4e39-9a8a-1796129a84fc", roomName: "Sports", amountUsers: 10, roomId: 178, namePrefab: "Sports", isEnable: true, playerLimit: 100, roomRank: 10, roomType: RoomType.PUBLIC));
                    break;
            }

            SceneManager.LoadScene("RoomScene", LoadSceneMode.Additive);
            HideView();
        }

        protected override void OnShow()
        {
            musicPlayer.Play("park-music");
            CleanUp();

            

            socketManager
                .Connect()
                .Subscribe()
                .AddTo(showDisposables);


            //GetInitialAvatarEndpoints();

            ShowMenuItems();
            SimpleSocketManager.Instance.Connect();
        }

#if false
>>>>>>> 9ba7824830fd60d0a5eea82386654ccaed7940f4
        void GetInitialAvatarEndpoints()
        {
            if (flagAvatarEndpointsReady)
            {
                return;
            }

            flagAvatarEndpointsReady = true;

            //Get ALL Catalog of items
            Injection.Get<IAvatarEndpoints>().GetAvatarCatalogItemsList()
              .Subscribe(listItems =>
              {
                  //Initialize Catalof of items
                  gameData.InitializeCatalogs(listItems.ToList<GenericCatalogItem>());

                  //After we get the items we can ask for my avatar skin
                  Injection.Get<IAvatarEndpoints>().GetAvatarSkin()
                   .Subscribe(json =>
                   {
                       //string jsonString = json.ToString(Formatting.Indented);
                       //Debug.Log("GetAvatarSkin <color=green>" + jsonString + "</color>");
                       AvatarCustomizationData avatarData = new AvatarCustomizationData();
                       avatarData.SetData(json);
                       gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarData);
                   });

                  //Get User Inventory
                  Injection.Get<IAvatarEndpoints>().GetPlayerInventory()
                     .Subscribe(listBagItems =>
                     {
                         gameData.AddItemsToBag(listBagItems);
                     });
              });

            //Get User Information
            Injection.Get<IAvatarEndpoints>().GetUserInformation()
                .Subscribe(usInformation =>
                {
                    gameData.GetUserInformation().Initialize(usInformation);
                });
        }
#endif

        void CleanUp()
        {
            memoryStorage.Set("currentRoomName", string.Empty);
            memoryStorage.Set("currentRoomId", string.Empty);
        }

        void ShowMenuItems()
        {
            container.gameObject.DestroyChildren();

            menuButtonPrefab ??= Resources.Load<Button>("MainMenuButton");

            foreach (var button in buttons)
            {
                if (!(button is {isActive: true})) continue;
                var newButton = Instantiate(menuButtonPrefab, container);
                newButton.name = "button " + button.name;
                newButton.GetComponentInChildren<TMP_Text>().text = button.name;
                newButton.OnClickAsObservable().Subscribe(() => button.onClick?.Invoke());
            }
        }

        [UsedImplicitly]
        public void Event_GetOut(string outPort)
        {
            viewManager.GetOut(outPort);
        }

        [UsedImplicitly]
        public void Event_QuitGame()
        {
            JesseUtils.QuitGame();
        }

        [UsedImplicitly]
        public void Event_LogOut()
        {
            JesseUtils.Logout();
        }
    }
}