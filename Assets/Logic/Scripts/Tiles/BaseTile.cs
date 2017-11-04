using System;
using KoreKrush;
using UnityEngine;


public abstract class BaseTile : MonoBehaviour
{
    // TODO: Remove this patch
    [HideInInspector]
    public int color = 0;
    
    public TileType TileType = TileType.None;
    public int Value = 1;
    public Board.Cell Cell;
    public bool IsConnected;
    public int Row { get { return Cell.row; } }
    public int Col { get { return Cell.col; } }
    public bool IsMovable = true;

    public SpriteRenderer Sprite;

    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }
    
    private void OnMouseDown()
    {
        KoreKrush.Events.Logic.TileSelect_L(this);
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
            KoreKrush.Events.Logic.TileSelect_L(this);
    }

    public abstract bool IsCompatible(BaseTile other);

    public virtual bool IsAdjacent(BaseTile other)
    {
        return Cell.AdjacentTo(other.Cell);
    }

    public virtual void Connect()
    {
        IsConnected = true;
    }

    public virtual void Disconnect()
    {
        IsConnected = false;
    }
}
