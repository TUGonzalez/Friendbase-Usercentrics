using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventUserEnter : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.USER_ENTER;

        public AvatarRoomData AvatarData { get; private set; }

        public IncomingEventUserEnter(JObject message) : base(message)
        {
            try
            {

                JObject avatarJson = Payload[SocketTags.AFFECTED_MEMBER].Value<JObject>();

                float positionx = avatarJson[SocketTags.POSITION_X].Value<float>();
                float positiony = avatarJson[SocketTags.POSITION_Y].Value<float>();
                string avatarState = avatarJson[SocketTags.AVATAR_STATE].Value<string>();
                int orientation = 1;

                JObject userAvatar = avatarJson[SocketTags.AVATAR_DATA].Value<JObject>();

                AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
                avatarCustomizationData.SetDataFromJoinRoom(userAvatar);

                string firebaseId = avatarJson[SocketTags.FIREBASE_ID].Value<string>();
                string username = avatarJson[SocketTags.USER_NAME].Value<string>();

                AvatarData = new AvatarRoomData(firebaseId, username, avatarState, positionx, positiony, orientation, avatarCustomizationData);
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}

