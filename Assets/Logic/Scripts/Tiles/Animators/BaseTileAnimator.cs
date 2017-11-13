using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(fileName = "Tile Animator", menuName = "Kore Krush/Base Tile Animator")]
public class BaseTileAnimator : ScriptableObject
{
    protected const float ScaleMultiplier = 1.2f;
    
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

    public virtual void Connect(StandardTile tile, float duration, float delay)
    {
        tile.transform.DOComplete();
        
        tile.transform.DOScale(tile.transform.localScale * ScaleMultiplier, duration)
            .SetEase(Ease.Linear)
            .SetDelay(delay);
    }
    
    public virtual void Disconnect(StandardTile tile, float duration, float delay)
    {
        tile.transform.DOComplete();

        tile.transform.DOScale(tile.transform.localScale / ScaleMultiplier, duration)
            .SetEase(Ease.Linear)
            .SetDelay(delay);
    }

    public virtual void Destroy(StandardTile tile, float duration, float delay)
    {
        tile.transform.DOScale(0, duration)
            .SetEase(Ease.Linear)
            .SetDelay(delay);
    }

    public virtual void Aim(StandardTile tile, float duration, float delay)
    {
        tile.Highlight.SetActive(true);
    }

    public virtual void Unaim(StandardTile tile, float duration, float delay)
    {
        tile.Highlight.SetActive(false);
    }
}
