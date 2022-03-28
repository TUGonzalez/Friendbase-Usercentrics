using System;
using System.Collections;
using System.Collections.Generic;
using Data.Rooms;
using TMPro;
using UI.ScrollView;
using UnityEngine;

public class UIRoomListCardController : UIAbstractCardController
{
    [SerializeField] protected TextMeshProUGUI txtRoomName;
    [SerializeField] protected TextMeshProUGUI txtAmountUsers;
    [SerializeField] protected Load2DObject load2DObject;

    private RoomInformation roomInformation;
    protected Action<EventType, RoomInformation, UIAbstractCardController> callback;

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        roomInformation = (RoomInformation)itemData;
        if (roomInformation == null)
        {
            return;
        }

        txtRoomName.text = roomInformation.RoomName;
        txtAmountUsers.text = roomInformation.AmountUsers.ToString();
        load2DObject.Load(roomInformation.NamePrefab + "_Thumb");

        this.callback = callback;
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, roomInformation, this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, roomInformation, this);
        }
    }
}
