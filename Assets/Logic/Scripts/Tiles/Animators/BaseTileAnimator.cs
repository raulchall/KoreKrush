using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(fileName = "Tile Animator", menuName = "Kore Krush/Base Tile Animator")]
public class BaseTileAnimator : ScriptableObject
{
    protected const float ScaleMultiplier = 1.2f;
    protected const float ScaleTime = .2f;
    
    public virtual void Spawn(StandardTile tile, float duration, float delay)
    {
        tile.transform.DOLocalMoveZ(10, duration)
            .From()
            .SetDelay(delay);

        tile.Sprite.DOColor(Color.clear, duration)
            .From()
            .SetDelay(delay)
            .SetEase(Ease.Linear);

        tile.transform.DOScale(0, duration)
            .From()
            .SetDelay(delay);
    }

    public virtual void Move(StandardTile tile, Vector2 newPos, float duration, float delay)
    {
        tile.transform.DOLocalMove(newPos, duration)
            .SetDelay(delay)
            .SetEase(Ease.Linear);
    }

    public virtual void Connect(StandardTile tile)
    {
        tile.transform.DOScale(tile.transform.localScale * ScaleMultiplier, ScaleTime)
            .SetEase(Ease.Linear);
    }
    
    public virtual void Disconnect(StandardTile tile)
    {
        tile.transform.DOScale(tile.transform.localScale / ScaleMultiplier, ScaleTime)
            .SetEase(Ease.Linear);
    }
}
