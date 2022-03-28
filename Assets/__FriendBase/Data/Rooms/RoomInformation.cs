namespace Data.Rooms
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class RoomInformation
    {
        public string RoomIdInstance { get; set; }
        public string RoomName { get; set; }
        public int AmountUsers { get; set; }
        public int RoomId { get; set; }
        public string NamePrefab { get; set; }
        public bool IsEnable { get; set; }
        public int PlayerLimit { get; set; }

        public int RoomRank { get; set; }
        public string RoomType { get; set; }

        public RoomInformation()
        {

        }

        public RoomInformation(string roomIdInstance, string roomName, int amountUsers, int roomId, string namePrefab, bool isEnable, int playerLimit, int roomRank, string roomType)
        {
            RoomIdInstance = roomIdInstance;
            RoomName = roomName;
            AmountUsers = amountUsers;
            RoomId = roomId;
            NamePrefab = namePrefab;
            IsEnable = isEnable;
            PlayerLimit = playerLimit;
            RoomRank = roomRank;
            RoomType = roomType;
        }

        public RoomInformation Duplicate()
        {
            return new RoomInformation(RoomIdInstance, RoomName, AmountUsers, RoomId, NamePrefab, IsEnable, PlayerLimit, RoomRank, RoomType);
        }
    }
}