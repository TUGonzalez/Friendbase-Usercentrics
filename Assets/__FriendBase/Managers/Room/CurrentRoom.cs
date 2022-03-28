using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using AddressablesSystem;
using Data.Rooms;

public class CurrentRoom : GenericSingleton<CurrentRoom>
{
    [SerializeField] private GameObject roomContainer;
    [SerializeField] private GameObject avatarContainer;
    [SerializeField] private GameObject itemsContainer;
    [SerializeField] private GameObject destinationPointsContainer;
    [SerializeField] private RoomUIReferences roomUIReferences;
    [SerializeField] private AvatarRoomController avatarPrefab;

    public string UserId { get; private set; }
    public AvatarsManager AvatarsManager { get; private set; }

    public bool IsReady { get; set; }
    public RoomInformation RoomInformation { get; private set; }

    private IGameData gameData;
    private ILoader loader;

    private RoomManager CurrentRoomManager;

    public void InitRoom(string userId, RoomInformation roomInformation)
    {
        gameData = Injection.Get<IGameData>();
        loader = Injection.Get<ILoader>();

        this.UserId = userId;
        this.AvatarsManager = new AvatarsManager(avatarPrefab, avatarContainer, destinationPointsContainer);
        this.RoomInformation = roomInformation;

        roomUIReferences.Initialize(IsPublicRoom());

        CreateRoomPrefab();
    }

    void CreateRoomPrefab()
    {
        GameObject roomPrefab = loader.GetModel(RoomInformation.NamePrefab + "_prefab");
        roomPrefab.transform.position = Vector3.zero;
        roomPrefab.transform.SetParent(roomContainer.transform, true);

        CurrentRoomManager = roomPrefab.GetComponent<RoomManager>();
    }

    public void SetCameras()
    {
        Transform player = AvatarsManager.GetMyAvatar().transform;

        CurrentRoomManager.SetCameras(player);
    }

    public bool IsMyRoom()
    {
        return UserId.Equals(gameData.GetUserInformation().UserId);
    }

    public bool IsPublicRoom()
    {
        return RoomInformation.RoomType == RoomType.PUBLIC;
    }

    public bool IsPrivateRoom()
    {
        return RoomInformation.RoomType == RoomType.PRIVATE;
    }

    public void DestroyRoom()
    {
        AvatarsManager.Destroy();
    }
}
