using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AvatarAnimationController : MonoBehaviour
{
    public const string ANIM_IDLE = "Player_Idle";
    public const string ANIM_WALK = "Player_Walk";
    public const string ANIM_HELLO = "Player_Hello";
    public const string ANIM_SIT = "Player_Sit";
    public const string ANIM_DANCE = "Player_Dance";

    private Animator animator;
    private Coroutine talkCoroutine;
    private int talkLayer;

    [SerializeField] Vector2 buttOffset;

    public Vector2 ButtOffset
    {
        get
        {
            var offset = buttOffset;

            return transform.localScale * offset;
        }
    }

    static readonly int Idle = Animator.StringToHash("Idle");
    static readonly int Walk = Animator.StringToHash("Walk");
    static readonly int Seat = Animator.StringToHash("Seat");
    static readonly int Hello = Animator.StringToHash("Hello");

    void Start()
    {
        animator = GetComponent<Animator>();
        talkLayer = animator.GetLayerIndex("Talking");
    }

    public void SetHelloState()
    {
        ResetAnimatorTriggers();
        animator.SetTrigger(Hello);
    }

    public void SetIdleState()
    {
        ResetAnimatorTriggers();
        animator.SetTrigger(Idle);
    }

    public void SetWalkState()
    {
        if (animator!=null)
        {
            AnimatorClipInfo[] _CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (_CurrentClipInfo!=null && _CurrentClipInfo.Length>0)
            {
                string nameClip = _CurrentClipInfo[0].clip.name;
                if (!nameClip.Equals(ANIM_WALK))
                {
                    ResetAnimatorTriggers();
                    animator.SetTrigger(Walk);
                }
            }
        }
    }

    public void ResetAnimatorTriggers()
    {
        animator.ResetTrigger(Idle);
        animator.ResetTrigger(Walk);
        animator.ResetTrigger(Seat);
    }
    public void SetSeatState()
    {
        ResetAnimatorTriggers();
        animator.SetTrigger(Seat);
    }



    public void SetTalkAnimation(float time)
    {
        if (talkCoroutine != null)
        {
            StopCoroutine(talkCoroutine);
        }

        talkCoroutine = StartCoroutine(TalkAnimationCoroutine(time));
    }

    IEnumerator TalkAnimationCoroutine(float time)
    {
        animator.SetLayerWeight(talkLayer, 1);

        yield return new WaitForSeconds(time);

        animator.SetLayerWeight(talkLayer, 0);
    }

    void OnDrawGizmos()
    {
        var p1 = transform.position;
        var p2 = transform.position + (Vector3) ButtOffset;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(p1, p2);
        Gizmos.color = Color.cyan * 0.75f;
        Gizmos.DrawSphere(p2, 0.2f);
    }
}