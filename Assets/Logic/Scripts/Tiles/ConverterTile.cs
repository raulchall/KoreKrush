using DG.Tweening;
using KoreKrush;
using UnityEngine;


public class ConverterTile : StandardTile 
{
    [Range(1, 5)]
    public int Range = 1;
    public GameObject NewTilePrefab;


    public override void Connect(float animDuration, float animDelay)
    {
        base.Connect(animDuration, animDelay);

        var lRow = Row - Range;
        var hRow = Row + Range;
        var lCol = Col - Range;
        var hCol = Col + Range;
        
        for (var i = lRow; i <= hRow; i++)
        {
            if (i < 0 || i >= Board.Rows) continue;
            
            for (var j = lCol; j <= hCol; j++)
            {
                if (j < 0 || j >= Board.Cols) continue;

                var currTile = Board.Cells[i, j].Tile;
                
                if (currTile.IsConnected) continue;
                
                var newTile = Instantiate(NewTilePrefab, transform.parent).GetComponent<StandardTile>();
                newTile.transform.position = currTile.transform.position;
                newTile.Spawn(animDuration, animDelay);
                
                Board.Cells[i, j].PushTile(newTile);
            }
        }
    }

    public override void Disconnect(float animDuration, float animDelay)
    {
        var lRow = Row - Range;
        var hRow = Row + Range;
        var lCol = Col - Range;
        var hCol = Col + Range;
        
        for (var i = lRow; i <= hRow; i++)
        {
            if (i < 0 || i >= Board.Rows) continue;
            
            for (var j = lCol; j <= hCol; j++)
            {
                if (j < 0 || j >= Board.Cols) continue;

                var currTile = Board.Cells[i, j].Tile;
                
                if (currTile.IsConnected) continue;

                var tile = Board.Cells[i, j].PopTile();
                tile.gameObject.SetActive(false);
                Destroy(tile);
            }
        }
        
        base.Disconnect(animDuration, animDelay);
    }
}
