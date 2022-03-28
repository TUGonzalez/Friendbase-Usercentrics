using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;
using Data.Users;

namespace Socket
{
    public class IncomingEventJoinRoom : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.JOIN_ROOM;

        public List<AvatarRoomData> ListAvatarData { get; private set; }
        
        public IncomingEventJoinRoom(JObject message):base(message)
        {
            try
            {
                ListAvatarData = new List<AvatarRoomData>();

                JObject response = Payload[SocketTags.RESPONSE].Value<JObject>();

                foreach (JObject memberPosition in response[SocketTags.MEMBER_POSITIONS])
                {
                    float positionx = memberPosition[SocketTags.POSITION_X].Value<float>();
                    float positiony = memberPosition[SocketTags.POSITION_Y].Value<float>();
                    string avatarState = memberPosition[SocketTags.AVATAR_STATE].Value<string>();
                    int orientation = 1;

                    JObject userAvatar = memberPosition[SocketTags.AVATAR_DATA].Value<JObject>();

                    AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
                    avatarCustomizationData.SetDataFromJoinRoom(userAvatar);

                    string firebaseId = memberPosition[SocketTags.FIREBASE_ID].Value<string>();
                    string username = memberPosition[SocketTags.USER_NAME].Value<string>();

                    ListAvatarData.Add(new AvatarRoomData(firebaseId, username, avatarState, positionx, positiony, orientation, avatarCustomizationData));
                }
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}

