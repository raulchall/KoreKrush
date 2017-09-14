using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class SocialButtonController : MonoBehaviour
{
    [Header("Animation")]
    public float socialsGroupOffset = 350;
    public float duration = .5f;
    public Transform socialGroup;

    private Tween selfAnimation;
    private Sequence socialsGroupAnimation;

    // Use this for initialization
    void Start()
    {
        selfAnimation = transform.DOPunchRotation(new Vector3(0, 0, 90), duration)
            .SetAutoKill(false)
            .SetRelative()
            .Pause();

        socialsGroupAnimation = DOTween.Sequence()
            .Append(socialGroup.DOLocalMoveX(socialsGroupOffset, duration)
                .SetRelative()
                .SetEase(Ease.OutBounce))
            .AppendCallback(() => socialsGroupAnimation.Pause())
            .Append(socialGroup.DOLocalMoveX(-socialsGroupOffset, duration)
                .SetRelative()
                .SetEase(Ease.OutBounce))
            .SetLoops(-1)
            .OnStepComplete(() => socialsGroupAnimation.Pause())
            .Pause();
    }

    public void ToggleSocials()
    {
        selfAnimation.Restart();
        socialsGroupAnimation.Play();
    }

    public void Print(string msg)
    {
        print(msg);
    }
}
