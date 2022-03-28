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
using Data.Rooms;

public class RoomListEndpoints : IRoomListEndpoints
{
    readonly GetFirebaseUid getFirebaseUid;
    readonly IWebHeadersBuilder headersBuilder;

    private IDebugConsole debugConsole;

    public RoomListEndpoints(GetFirebaseUid getFirebaseUid, IWebHeadersBuilder headersBuilder)
    {
        debugConsole = Injection.Get<IDebugConsole>();
        this.getFirebaseUid = getFirebaseUid;
        this.headersBuilder = headersBuilder;
    }

    public IObservable<List<RoomInformation>> GetPublicRoomsList() => GetPublicRoomsListAsync().ToObservable().ObserveOnMainThread();
    async Task<List<RoomInformation>> GetPublicRoomsListAsync()
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/rooms";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToPublicRoomsList(response.json);
    }

    List<RoomInformation> ToPublicRoomsList(JObject jObject)
    {
        List<RoomInformation> listRoomInformation = new List<RoomInformation>();

        foreach (JObject roomData in jObject["data"])
        {
            try
            {
                RoomInformation roomInformation = new RoomInformation();

                roomInformation.RoomName = roomData["display_name"].Value<string>();
                roomInformation.AmountUsers = roomData["total_members"].Value<int>();
                roomInformation.RoomId = roomData["id"].Value<int>();
                roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
                roomInformation.IsEnable = roomData["enabled"].Value<bool>();
                roomInformation.PlayerLimit = roomData["player_limit"].Value<int>();
                //roomInformation.RoomRank = roomData["room_rank"].Value<int>();
                roomInformation.RoomType = roomData["type"].Value<string>();

                listRoomInformation.Add(roomInformation);
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("RoomListEndpoints:ToPublicRoomsList", "Exception", "Invalid Json Data");
            }
        }
        return listRoomInformation;
    }

    public IObservable<List<RoomInformation>> GetPublicRoomsListInside() => GetPublicRoomsListInsideAsync().ToObservable().ObserveOnMainThread();
    async Task<List<RoomInformation>> GetPublicRoomsListInsideAsync()
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/rooms/178/room-instances";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        Debug.Log("INSIDE====" + response.json);
        return ToPublicRoomsListInside(response.json);
    }

    List<RoomInformation> ToPublicRoomsListInside(JObject jObject)
    {
        List<RoomInformation> listRoomInformation = new List<RoomInformation>();

        foreach (JObject roomData in jObject["data"])
        {
            try
            {
                RoomInformation roomInformation = new RoomInformation();

                roomInformation.RoomName = roomData["display_name"].Value<string>();
                roomInformation.RoomIdInstance = roomData["id"].Value<string>();
                roomInformation.AmountUsers = roomData["members_count"].Value<int>();
                roomInformation.RoomId = roomData["room_id"].Value<int>();

                roomInformation.NamePrefab = roomData["display_name"].Value<string>();
                //roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
                //roomInformation.IsEnable = roomData["enabled"].Value<bool>();
                //roomInformation.PlayerLimit = roomData["player_limit"].Value<int>();
                //roomInformation.RoomRank = roomData["room_rank"].Value<int>();
                //roomInformation.RoomType = roomData["type"].Value<string>();

                listRoomInformation.Add(roomInformation);
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("RoomListEndpoints:ToPublicRoomsList", "Exception", "Invalid Json Data");
            }
        }
        return listRoomInformation;
    }
}
