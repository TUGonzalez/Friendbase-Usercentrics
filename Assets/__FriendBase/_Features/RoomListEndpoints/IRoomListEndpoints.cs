using System.Collections.Generic;
using System;
using Data.Rooms;
using System.Threading.Tasks;

public interface IRoomListEndpoints 
{
    IObservable<List<RoomInformation>> GetPublicRoomsList();
    IObservable<List<RoomInformation>> GetPublicRoomsListInside();
}
