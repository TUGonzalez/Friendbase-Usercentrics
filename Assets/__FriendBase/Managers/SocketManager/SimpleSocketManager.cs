using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using System.Reflection;
using System.Threading.Tasks;
using AuthFlow;
using AuthFlow.EndAuth.Repo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using NativeWebSocket;
using DebugConsole;

namespace Socket
{
    public class SimpleSocketManager : GenericSingleton<SimpleSocketManager>, ISimpleSocketManager
    {
        private Dictionary<string, List<Action<AbstractIncomingSocketEvent>>> socketEventDeliveries;

        private WebSocket websocket;
        readonly SocketStateManager state = new SocketStateManager();
        ILocalUserInfo userInfo;
        int retriesLeft = 3;
        private IDebugConsole debugConsole;
        private IncomingEventManager incomingEventManager;

        void Start()
        {
            debugConsole = Injection.Get<IDebugConsole>();
            incomingEventManager = new IncomingEventManager();
            socketEventDeliveries = new Dictionary<string, List<Action<AbstractIncomingSocketEvent>>>();
        }

        public void Connect()
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_INFO)) debugConsole.TraceLog(LOG_TYPE.SOCKET_INFO, "CONNECT");

            //Create Header with Auth
            Injection.SafeGet(ref userInfo);
            var token = userInfo["firebase-login-token"];
            var headers = new Dictionary<string, string>();
            headers.Add("x-authorization", token);

            //Callbacks and Connect
            websocket = new WebSocket(Constants.SocketUrl, headers);

            websocket.OnMessage += OnMessageReceive;
            websocket.OnError += HandleConnectionError;
            websocket.OnClose += OnCloseSocket;
            websocket.OnOpen += OnConnect;

            websocket.Connect();
        }

        private void OnConnect()
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_INFO)) debugConsole.TraceLog(LOG_TYPE.SOCKET_INFO, "CONNECTION SUCCEED");
            InvokeRepeating("SendHeartbeat", 1, 10);
        }

        private void OnCloseSocket(WebSocketCloseCode closeCode)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_INFO)) debugConsole.TraceLog(LOG_TYPE.SOCKET_INFO, "CONNECTION CLOSED:" + closeCode);
        }

        private void OnMessageReceive(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_IN)) debugConsole.TraceLog(LOG_TYPE.SOCKET_IN, "ONMESSAGE:" + message);

            AbstractIncomingSocketEvent incomingSocketEvent = incomingEventManager.GetIncomingSocketEvent(message);
            if (incomingSocketEvent!=null)
            {
                DeliverSocketEventToSuscribers(incomingSocketEvent);
            }
        }

        void Update()
        {
            if (websocket != null)
            {
                websocket.DispatchMessageQueue();
            }
        }

        private void SendHeartbeat()
        {
            // methods for common events that will be needed on friendbase(plus heartbeat that is always needed for phoenix sockets to keep the connection alive)
            SendEvent("phoenix", "heartbeat", new JObject(), "heartbeat_ref");
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            websocket.Close();
            websocket.OnMessage -= OnMessageReceive;
            websocket.OnError -= HandleConnectionError;
            websocket.OnClose -= OnCloseSocket;
            websocket.OnOpen -= OnConnect;
        }

        void HandleConnectionError(string er)
        {
            //if there is an error connecting
            //remove one available attempt
            retriesLeft--;
            if (retriesLeft == 0)
            {
                //if there is no more attempts the reboot the application
                Debug.LogError(er + " reloading ... ");
                JesseUtils.Nuke();
                return;
            }
            Debug.LogError(er + " attempting reconection");
            //if there is available attempts then try to reconnect again
            // TODO implement reconnect without observables
        }

        public void JoinChatRoom(string roomName, string roomId, float positionX, float positionY)
        {
            // Supermarket
            // 7334bdf6-f63d-49ec-a37a-2a025172b789
            // 7.6
            // -1.52
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinChatRoom roomName:{roomName} roomId:{roomId}");

            Vector2 pos = new Vector2(positionX, positionY);
            string topic = $"chat_room:{roomName}:{roomId}";
            JObject payload = new JObject
            {
                ["position_x"] = pos.x,
                ["position_y"] = pos.y
            };
            var eventRef = $"join_{roomId}";

            SendEvent(topic, "phx_join", payload, eventRef);
        }

        void SendEvent(string topic, string eventType, JObject payload, string eventRef = "none")
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                var eventData = new JObject
                {
                    ["topic"] = topic,
                    ["event"] = eventType,
                    ["payload"] = payload,
                    ["ref"] = eventRef == "none" ? $"{topic}_{eventType}" : eventRef
                };
                websocket.SendText(eventData.ToString());
            }
        }
        public void SendChatMessage(string roomName, string roomId, string content, string username, string usernameColor)
        {
             SendEvent($"chat_room:{roomName}:{roomId}",
                "message",
                new JObject
                {
                    ["content"] = content,
                    ["username"] = username,
                    ["usernameColor"] = usernameColor
                },
                $"message_{roomId}");
        }

        public void SendAvatarMove(string roomName, string roomId, float positionX, float positionY)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendAvatarMove roomName:{roomName} roomId:{roomId} positionX:{positionX} positionY:{positionY}");
            SendEvent($"chat_room:{roomName}:{roomId}", "change_position", new JObject { ["position_x"] = positionX, ["position_y"] = positionY }, $"message_{roomId}");
        }

        public void LeaveChatRoom(string roomName, string roomId)
        {
            SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", new JObject (), $"leave_{roomId}");
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-------------------------   S U S C R I P T I O N S   -----------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------

        public bool Suscribe(string eventType, Action<AbstractIncomingSocketEvent> suscriber)
        {
            if (suscriber == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(eventType))
            {
                return false;
            }

            if (!socketEventDeliveries.ContainsKey(eventType))
            {
                socketEventDeliveries[eventType] = new List<Action<AbstractIncomingSocketEvent>>();
            }
            socketEventDeliveries[eventType].Add(suscriber);

            return true;
        }

        public bool Unsuscribe(string eventType, Action<AbstractIncomingSocketEvent> suscriber)
        {
            bool flag = false;
            if (suscriber == null)
            {
                return flag;
            }
            if (eventType == null)
            {
                return flag;
            }
            if (socketEventDeliveries.ContainsKey(eventType))
            {
                for (int j = socketEventDeliveries[eventType].Count - 1; j >= 0; j--)
                {
                    if (socketEventDeliveries[eventType][j] == suscriber)
                    {
                        socketEventDeliveries[eventType].RemoveAt(j);
                        flag = true;
                    }
                }
            }

            return flag;
        }

        public void DeliverSocketEventToSuscribers(AbstractIncomingSocketEvent socketEvent)
        {
            if (socketEvent == null)
            {
                return;
            }
            string eventType = socketEvent.EventType;

            if (socketEventDeliveries.ContainsKey(eventType))
            {
                List<Action<AbstractIncomingSocketEvent>> duplicateList = new List<Action<AbstractIncomingSocketEvent>>(socketEventDeliveries[eventType]);
                int amount = duplicateList.Count;
                for (int i = 0; i < amount; i++)
                {
                    if (duplicateList[i] != null)
                    {
                        duplicateList[i](socketEvent);
                    }
                }
            }
        }














        // overload of the method above so it can be called from other classes without them needing to add "using Newtonsoft.Json.Linq;" to all files
        //public async Task SendEvent(string topic, string eventType, string payloadString,
        //    string eventRef = "none")
        //{
        //    await SendEvent(topic, eventType, JObject.Parse(payloadString), eventRef);
        //}

        //public async void SendMessage(string roomName, string roomId, string content)
        //{
        //    await SendEvent($"chat_room:{roomName}:{roomId}", "message", new JObject { ["content"] = content },
        //        $"message_{roomId}");
        //}
        //public async void SendChatMessage(string roomName, string roomId, string content, string username)
        //{
        //    await SendEvent($"chat_room:{roomName}:{roomId}",
        //        "message",
        //        new JObject
        //        {
        //            ["content"] = content,
        //            ["username"] = username
        //        },
        //        $"message_{roomId}");
        //}

        //public async void LeaveChatRoom(string roomName, string roomId)
        //{
        //    await SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", "{}", $"leave_{roomId}");
        //}
        //public async void LeaveCurrentChatRoom()
        //{
        //    var (roomName, roomId) = (state.chatRoomName, state.chatRoomId);
        //    await SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", "{}", $"leave_{roomId}");
        //}
    }
}