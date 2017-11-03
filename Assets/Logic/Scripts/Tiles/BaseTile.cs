using System;
using KoreKrush;
using UnityEngine;


public abstract class BaseTile : MonoBehaviour
{
    // TODO: Remove this property
    public int color
    {
        get
        {
            return Array.IndexOf(GameObject.Find("Tiles Manager").GetComponent<TilesManagerController>().Colors, Color);
        }
    }
    
    public Board.Cell Cell;
    public bool IsConnected;
    public int Row { get { return Cell.row; } }
    public int Col { get { return Cell.col; } }
    public bool IsMovable { get { return true; } }

    public SpriteRenderer Sprite;

    public Color Color
    {
        get { return Sprite.color; }
        set { Sprite.color = value; }
    }

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

    public virtual void SetUp()
    {
        Color = Board.Colors.Choice();
    }

    public virtual bool IsAdjacent(BaseTile other)
    {
        return Cell.AdjacentTo(other.Cell);
    }

    public virtual bool IsCompatible(BaseTile other)
    {
        return Color == other.Color;
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
