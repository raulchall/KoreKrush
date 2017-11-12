using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Rotator", menuName = "Kore Krush/Permanent Rotator")]
public class PermanentRotator : BaseTileAnimator
{
    public override void Spawn(StandardTile tile, float duration, float delay)
    {
        base.Spawn(tile, duration, delay);
        
        tile.transform.DOLocalRotate(new Vector3(0, 0, 360), 2, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}
