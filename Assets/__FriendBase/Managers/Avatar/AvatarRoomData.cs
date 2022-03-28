using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Users;

public class AvatarRoomData
{
    public string FirebaseId { get; private set; }
    public string Username { get; private set; }
    public string AvatarState { get; private set; }
    public float Positionx { get; set; }
    public float Positiony { get; set; }
    public int Orientation { get; set; }
    public AvatarCustomizationData AvatarCustomizationData { get; set; }

    public AvatarRoomData(string firebaseId, string userName, string avatarState, float positionx, float positiony, int orientation, AvatarCustomizationData avatarCustomizationData)
    {
        FirebaseId = firebaseId;
        Username = userName;
        AvatarState = avatarState;
        Positionx = positionx;
        Positiony = positiony;
        Orientation = orientation;
        AvatarCustomizationData = avatarCustomizationData;
    }
}
