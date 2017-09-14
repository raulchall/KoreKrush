using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class TitleController : MonoBehaviour
{
    [Header("Animation")]
    public Color color;
    public float duration = 2;
    public LoopType loopType = LoopType.Yoyo;
    public Ease ease;

    // Use this for initialization
    void Start()
    {
        var text = GetComponent<Text>();

        text.DOColor(color, duration)
            .SetLoops(-1, loopType)
            .SetEase(ease);

        text.DOText("Kore Krush", duration)
            .SetLoops(-1, loopType)
            .SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
