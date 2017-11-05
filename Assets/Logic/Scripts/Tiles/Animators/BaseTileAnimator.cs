using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(fileName = "Tile Animator", menuName = "Kore Krush/Base Tile Animator")]
public class BaseTileAnimator : ScriptableObject
{
    public virtual void Spawn(StandardTile tile, float delay, float duration)
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

    public virtual void Move(StandardTile tile, Vector2 newPos, float delay, float duration)
    {
        tile.transform.DOLocalMove(newPos, duration)
            .SetDelay(delay)
            .SetEase(Ease.Linear);
    }
}
