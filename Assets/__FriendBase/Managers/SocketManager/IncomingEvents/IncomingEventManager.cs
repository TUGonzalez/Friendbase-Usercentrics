using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Socket
{
    public class IncomingEventManager
    {
        private bool isInsideRoom;

        public IncomingEventManager()
        {
            isInsideRoom = false;
        }

        public AbstractIncomingSocketEvent GetIncomingSocketEvent(string message)
        {
            JObject jsonMessage = JObject.Parse(message);

            string eventType = jsonMessage[SocketTags.EVENT].Value<string>();
            JObject payload = jsonMessage[SocketTags.PAYLOAD].Value<JObject>();
            string reference = jsonMessage[SocketTags.REF].Value<string>();
            string topic = jsonMessage[SocketTags.TOPIC].Value<string>();

            if (eventType.Equals("phx_reply") && reference.Contains("join") && !isInsideRoom)
            {
                isInsideRoom = true;
                return new IncomingEventJoinRoom(jsonMessage);
            }

            if (eventType.Equals("positions_update") && isInsideRoom)
            {
                return new IncomingEventMoveAvatar(jsonMessage);
            }

            if (eventType.Equals("member_join") && isInsideRoom)
            {
                return new IncomingEventUserEnter(jsonMessage);
            }

            if (eventType.Equals("phx_close") && isInsideRoom)
            {
                isInsideRoom = false;
                return new IncomingEventLeaveRoom(jsonMessage);
            }

            if (eventType.Equals("member_left") && isInsideRoom)
            {
                return new IncomingEventUserLeave(jsonMessage);
            }

            if (eventType.Equals("message") && isInsideRoom)
            {
                return new IncomingEventChatMessage(jsonMessage);
            }
            
            return null;
        }
    }
}