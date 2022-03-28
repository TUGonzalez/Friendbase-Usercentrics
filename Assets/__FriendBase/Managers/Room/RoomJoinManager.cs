using System.Collections;
using System.Collections.Generic;
using AddressablesSystem;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using Data.Users;
using Socket;
using UnityEngine;
using Data.Rooms;
using Architecture.ViewManager;
using UnityEngine.SceneManagement;
using AvatarCustomization;
using UnityEngine.UI;
using DG.Tweening;
using Pathfinding;

public class RoomJoinManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loaderPanel;

    private IGameData gameData;
    private ILoader loader;
    RoomInformation roomInformation;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        loader = Injection.Get<ILoader>();

        loaderPanel.gameObject.SetActive(true);

        CurrentRoom.Instance.IsReady = false;
        StartCoroutine(JoinRoom());
    }

    void LoadRoomPathfindingMesh(string roomName)
    {
        TextAsset pathsInfo = Resources.Load<TextAsset>(roomName + "_path");
        AstarPath.active.data.DeserializeGraphs(pathsInfo.bytes);
    }

    IEnumerator JoinRoom()
    {
        //roomInfo = gameData.GetCatalogByItemType(Data.Catalog.ItemType.ROOM).GetItem(2);
        roomInformation = gameData.GetRoomInformation();

        LoadRoomPathfindingMesh(roomInformation.NamePrefab);

        //PRELOAD ITEMS TO USE IN THE ROOM
        string[] namesToPreload = { roomInformation.NamePrefab + "_prefab" };
        foreach (string nameToPreload in namesToPreload)
        {
            loader.LoadItem(new LoaderItemModel(nameToPreload));
        }

        
        //WAIT UNTIL ALL ITEMS HAS BEEN LOADED
        while (!AreAllItemsLoaded(namesToPreload))
        {
            yield return null;
        }

        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.LEAVE_ROOM, OnLeaveRoom);
        //Send JOIN to the server
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.JOIN_ROOM, OnJoinRoom);
        
        SimpleSocketManager.Instance.JoinChatRoom(roomInformation.RoomName, roomInformation.RoomIdInstance, 3f, -1f);

        yield return null;
    }

    bool AreAllItemsLoaded(string[] namesToCheck)
    {
        foreach (string nameToPreload in namesToCheck)
        {
            LoaderAbstractItem item = loader.GetItem(nameToPreload);
            if (item==null || item.State != LoaderItemState.SUCCEED)
            {
                return false;
            }
        }
        return true;
    }

    void OnJoinRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.JOIN_ROOM, OnJoinRoom);

        IncomingEventJoinRoom incomingEventJoinRoom = incomingEvent as IncomingEventJoinRoom;
        if (incomingEventJoinRoom!=null && incomingEventJoinRoom.State == SocketEventResult.OPERATION_SUCCEED)
        {
            //Initialize Current Room
            string ownerUserId = ""; //When Refactor is done on backend
            CurrentRoom.Instance.InitRoom(ownerUserId, roomInformation);

            //Create Users
            OnJoinRoomUsers(incomingEventJoinRoom.ListAvatarData);

            CurrentRoom.Instance.IsReady = true;
            CurrentRoom.Instance.SetCameras();
            StartCoroutine(LoaderFadeOut());
        }
    }

    IEnumerator LoaderFadeOut()
    {
        yield return new WaitForEndOfFrame();
        loaderPanel.DOFade(0, 1).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(1);
        loaderPanel.gameObject.SetActive(false);
    }

    void OnJoinRoomUsers(List<AvatarRoomData> listAvatarData)
    {
        foreach (AvatarRoomData avatarRoomData in listAvatarData)
        {
            CurrentRoom.Instance.AvatarsManager.AddAvatar(avatarRoomData);
        }
    }

    void OnLeaveRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventLeaveRoom incomingEventLeaveRoom = incomingEvent as IncomingEventLeaveRoom;
        if (incomingEventLeaveRoom != null && incomingEventLeaveRoom.State == SocketEventResult.OPERATION_SUCCEED)
        {
            CurrentRoom.Instance.DestroyRoom();
            StartCoroutine(LeaveRoomCoroutine());
        }
    }

    IEnumerator LeaveRoomCoroutine()
    {
        Injection.Get<IViewManager>().Show<AvatarCustomizationPanel>();

        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync("RoomScene");
    }

    public void LeaveChatRoom()
    {
        string roomName = CurrentRoom.Instance.RoomInformation.RoomName;
        string roomId = CurrentRoom.Instance.RoomInformation.RoomIdInstance;

        SimpleSocketManager.Instance.LeaveChatRoom(roomName, roomId);

    }
    private void OnDestroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.LEAVE_ROOM, OnLeaveRoom);
    }
}

/*
 * Load room with addressables 
 * Data structures 
 * Managers
 * Place avatar, move avatar
 * 
 * Refactor Socket Manager
 * System for Suscribe on events on Socket Manager
 * 
 * Integrate socket manager to join/leaveRoom /user enter/user leave
 * 
 * 
 * Integrate pathfinding
 * Implement Camera system
 * 
 * Change system to new Backend
 * Refactor RoomList modal
 * 
 * Chair system
 * 
 * FriendList/Chat small modifications
 * 
 * Music
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * DELETE
 * UIStoreFurnituresManager
 * MenusSwitcher
 * namespace WorldAreasCanvas
 * 
 * Fix Broken Fonts
 */