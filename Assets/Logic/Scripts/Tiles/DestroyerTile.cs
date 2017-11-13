using KoreKrush;

public class DestroyerTile : StandardTile
{
    public int Range = 2;
    
    
    public override void Connect(float animDuration, float animDelay)
    {
        base.Connect(animDuration, animDelay);

        for (int i = 1, leftCol = Col - i, rightCol = Col + i; i <= Range; i++)
        {
            if (leftCol >= 0)
            {
                var tile = Board.Cells[Row, leftCol].Tile;
                
                if (tile && !tile.IsTarget && !tile.IsConnected) 
                    tile.Aim(animDuration, animDelay);
            }

            if (rightCol < Board.Cols)
            {
                var tile = Board.Cells[Row, rightCol].Tile;
                
                if (tile && !tile.IsTarget && !tile.IsConnected) 
                    tile.Aim(animDuration, animDelay);
            }
        }
    }
    
    public override void Disconnect(float animDuration, float animDelay)
    {
        base.Disconnect(animDuration, animDelay);

        for (int i = 1, leftCol = Col - i, rightCol = Col + i; i <= Range; i++)
        {
            if (leftCol >= 0)
            {
                var tile = Board.Cells[Row, leftCol].Tile;
                
                if (tile && !tile.IsTarget && !tile.IsConnected) 
                    tile.Unaim(animDuration, animDelay);
            }

            if (rightCol < Board.Cols)
            {
                var tile = Board.Cells[Row, rightCol].Tile;
                
                if (tile && !tile.IsTarget && !tile.IsConnected) 
                    tile.Unaim(animDuration, animDelay);
            }
        }
    }

    public override void Destroy(float animDuration, float animDelay)
    {
        base.Destroy(animDuration, animDelay);
        
        for (int i = 1, leftCol = Col - i, rightCol = Col + i; i <= Range; i++)
        {
            if (leftCol >= 0)
            {
                var tile = Board.Cells[Row, leftCol].Tile;
                
                if (tile && !tile.IsConnected) 
                    tile.Destroy(animDuration, animDelay);
            }

            if (rightCol < Board.Cols)
            {
                var tile = Board.Cells[Row, rightCol].Tile;
                
                if (tile && !tile.IsConnected) 
                    tile.Destroy(animDuration, animDelay);
            }
        }
    }

    public override void Aim(float animDuration, float animDelay)
    {
        base.Aim(animDuration, animDelay);
        
        for (int i = 1, leftCol = Col - i, rightCol = Col + i; i <= Range; i++)
        {
            if (leftCol >= 0)
            {
                var tile = Board.Cells[Row, leftCol].Tile;
                
                if (tile && !tile.IsTarget && !tile.IsConnected) 
                    tile.Aim(animDuration, animDelay);
            }

            if (rightCol < Board.Cols)
            {
                var tile = Board.Cells[Row, rightCol].Tile;
                
                if (tile && !tile.IsTarget && !tile.IsConnected) 
                    tile.Aim(animDuration, animDelay);
            }
        }
    }
    
    public override void Unaim(float animDuration, float animDelay)
    {
        base.Aim(animDuration, animDelay);
        
        for (int i = 1, leftCol = Col - i, rightCol = Col + i; i <= Range; i++)
        {
            if (leftCol >= 0)
            {
                var tile = Board.Cells[Row, leftCol].Tile;
                
                if (tile && !tile.IsConnected) 
                    tile.Unaim(animDuration, animDelay);
            }

            if (rightCol < Board.Cols)
            {
                var tile = Board.Cells[Row, rightCol].Tile;
                
                if (tile && !tile.IsConnected) 
                    tile.Unaim(animDuration, animDelay);
            }
        }
    }
}
