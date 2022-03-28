using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Socket;
using UnityEngine;
using DebugConsole;

public class AvatarsManager 
{
    private List<AvatarRoomController> listAvatars;
    private GameObject avatarsContainer;
    private GameObject destinationPointsContainer;
    private AvatarRoomController avatarPrefab;

    private IGameData gameData;
    private IDebugConsole debugConsole;

    public AvatarsManager(AvatarRoomController avatarPrefab, GameObject avatarsContainer, GameObject destinationPointsContainer)
    {
        this.avatarPrefab = avatarPrefab;
        this.avatarsContainer = avatarsContainer;
        this.destinationPointsContainer = destinationPointsContainer;
        listAvatars = new List<AvatarRoomController>();
        gameData = Injection.Get<IGameData>();
        debugConsole = Injection.Get<IDebugConsole>();

        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.AVATAR_MOVE, OnAvatarMove);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveRoom);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_ENTER, OnUserEnterRoom);
    }

    public AvatarRoomController AddAvatar(AvatarRoomData avatarData)
    {
        AvatarRoomController avatar = GetAvatarById(avatarData.FirebaseId);
        if (avatar != null)
        {
            debugConsole.ErrorLog("AvatarsManager:AddAvatar", "User already created", "userId:"+ avatarData.FirebaseId);
            return null;
        }

        bool isLocalPlayer = avatarData.FirebaseId == gameData.GetUserInformation().FirebaseId;

        AvatarRoomController avatarRoomController = UnityEngine.Object.Instantiate(avatarPrefab, Vector3.zero, avatarPrefab.transform.rotation);
        avatarRoomController.name = "Avatar_" + avatarData.Username;
        avatarRoomController.transform.SetParent(avatarsContainer.transform, true);
        avatarRoomController.Init(avatarData, destinationPointsContainer, isLocalPlayer);
        listAvatars.Add(avatarRoomController);
        return avatarRoomController;
    }

    private void OnAvatarMove(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventMoveAvatar incomingEventMoveAvatar = incomingEvent as IncomingEventMoveAvatar;
        if (incomingEventMoveAvatar != null && incomingEventMoveAvatar.State == SocketEventResult.OPERATION_SUCCEED)
        {
            //Discard my user as we move it locally
            if (!incomingEventMoveAvatar.FirebaseId.Equals(gameData.GetUserInformation().FirebaseId))
            {
                AvatarRoomController avatar = GetAvatarById(incomingEventMoveAvatar.FirebaseId);
                if (avatar != null)
                {
                    avatar.SetWalkToDestination(incomingEventMoveAvatar.Destinationx, incomingEventMoveAvatar.Destinationy);
                }
            }
        }
    }

    public bool RemoveAvatar(string idFirebase)
    {
        int amountAvatars = listAvatars.Count;
        for (int i = amountAvatars - 1; i >= 0; i--)
        {
            if (listAvatars[i].AvatarData.FirebaseId.Equals(idFirebase))
            {
                AvatarRoomController currentAvatar = listAvatars[i];
                currentAvatar.DestroyAvatar();
                listAvatars.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public AvatarRoomController GetMyAvatar()
    {
        string idFirebase = gameData.GetUserInformation().FirebaseId;
        return GetAvatarById(idFirebase);
    }

    public AvatarRoomController GetAvatarById(string idFirebase)
    {
        foreach (AvatarRoomController avatar in listAvatars)
        {
            if (avatar.AvatarData.FirebaseId.Equals(idFirebase))
            {
                return avatar;
            }
        }
        return null;
    }

    public void Destroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.AVATAR_MOVE, OnAvatarMove);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_ENTER, OnUserEnterRoom);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveRoom);
        foreach (AvatarRoomController avatar in listAvatars)
        {
            avatar.DestroyAvatar();
        }
    }

    private void OnUserEnterRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserEnter incomingEventUserEnter = incomingEvent as IncomingEventUserEnter;
        if (incomingEventUserEnter != null && incomingEventUserEnter.State == SocketEventResult.OPERATION_SUCCEED)
        {
            AddAvatar(incomingEventUserEnter.AvatarData);
        }
    }

    void OnUserLeaveRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserLeave incomingEventUserLeave = incomingEvent as IncomingEventUserLeave;
        if (incomingEventUserLeave != null && incomingEventUserLeave.State == SocketEventResult.OPERATION_SUCCEED)
        {
            RemoveAvatar(incomingEventUserLeave.FirebaseId);
        }
    }
}
