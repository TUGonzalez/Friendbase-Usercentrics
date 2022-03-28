using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Users;
using UnityEngine;
using DG.Tweening;
using Pathfinding;

public class AvatarRoomController : MonoBehaviour
{
    [SerializeField] private AvatarCustomizationController avatarCustomizationController;
    [SerializeField] private AvatarAnimationController avatarAnimationController;
    [SerializeField] private AIPath aIPath;
    [SerializeField] private AIDestinationSetter aIDestinationSetter;

    public AvatarRoomData AvatarData { get; private set; }

    public AvatarAnimationController AnimationController => avatarAnimationController;

    public AvatarCustomizationController CustomizationController => avatarCustomizationController;

    private GameObject destinationPointsContainer;
    private GameObject destinationPoint;
    private bool isLocalPlayer;

    public void Init(AvatarRoomData avatarData, GameObject destinationPointsContainer, bool isLocalPlayer)
    {
        this.AvatarData = avatarData;
        this.destinationPointsContainer = destinationPointsContainer;
        this.isLocalPlayer = isLocalPlayer;
        //Set Avatar Skin
        avatarCustomizationController.SetAvatarCustomizationData(avatarData.AvatarCustomizationData.GetSerializeData());
        //Set Avatar Position
        SetPosition(avatarData.Positionx, avatarData.Positiony);
        SetOrientation(avatarData.Orientation);

        destinationPoint = new GameObject(avatarData.Username + "_destinationPoint");
        destinationPoint.transform.position = new Vector3(avatarData.Positionx, avatarData.Positiony, 0);
        destinationPoint.transform.SetParent(destinationPointsContainer.transform);

        aIDestinationSetter.target = destinationPoint.transform;
    }

    void SetPosition(float positionx, float positiony)
    {
        AvatarData.Positionx = positionx;
        AvatarData.Positiony = positiony;
        Vector3 currentPosition = transform.position;

        currentPosition.x = positionx;
        currentPosition.y = positiony;

        transform.position = currentPosition;
    }

    public void SetOrientation(int orientation)
    {
        if (orientation!=1 && orientation!=-1)
        {
            return;
        }
        AvatarData.Orientation = orientation;

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * orientation;

        transform.localScale = scale;
    }

    public bool IsLocalPlayer()
    {
        return isLocalPlayer;
    }

    public void SetWalkToDestination(float positionx, float positiony)
    {
        avatarAnimationController.SetWalkState();

        destinationPoint.transform.position = new Vector3(positionx, positiony, 0);

        //this.transform.DOKill(false);

        //this.transform.DOMove(new Vector2(positionx, positiony), 1).SetEase(Ease.Linear).OnComplete(()=> {
        //    avatarAnimationController.SetIdleState();
        //});
    }

    private void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        Vector2 vel = aIPath.desiredVelocity;
        if (vel.magnitude > 0.01)
        {
            avatarAnimationController.SetWalkState();
            this.transform.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
        }
        else
        {
            avatarAnimationController.SetIdleState();
        }
    }

    public void DestroyAvatar()
    {
        Destroy(destinationPoint);
        Destroy(this.gameObject);
    }
}
