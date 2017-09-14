using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PlayButtonController : MonoBehaviour
{
    [Header("Animation")]
    public Vector2 maxScale = new Vector2(1.3f, 1.3f);
    public float duration = .5f;
    public LoopType loopType = LoopType.Yoyo;

    // Use this for initialization
    void Start()
    {
        transform.DOScale(maxScale, duration)
            .SetLoops(-1, loopType);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
