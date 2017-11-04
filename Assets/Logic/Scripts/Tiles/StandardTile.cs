public class StandardTile : BaseTile
{
    public override bool IsCompatible(BaseTile other)
    {
        var o = other as StandardTile;
        return o && TileType == o.TileType;
    }
}
