using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileType
{
    Blank = 0,
    Solid = 1
}

public class Tile : MonoBehaviour {
    public int x;
    public int y;
    public float size = 1f;
    public Tile top;
    public Tile bottom;
    public Tile left;
    public Tile right;
    public TileType type;

    public Sprite spriteSolid;
    public Sprite spriteLiquid;

    Vector2 topLeft;
    Vector2 topRight;
    Vector2 bottomLeft;
    Vector2 bottomRight;

    SpriteRenderer spriteRenderer;
    Vector3 spriteRendererOriginalPosition;

    public bool isSettled = false;

    public float liquid;

    private bool showUI = false;

    public bool ShowUI
    {
        get
        {
            return this.showUI;
        }

        set
        {
            if (value)
            {
                StartUI();
            } else
            {
                ClearUI();
            }
        }
    }

    Text uiLiquid;

    // Use this for initialization
    void Start () {
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRendererOriginalPosition = spriteRenderer.transform.position;

        float halfSize = size * 0.5f;
        Vector2 position = transform.position;

        topLeft = new Vector2(position.x - halfSize, position.y + halfSize);
        topRight = new Vector2(position.x + halfSize, position.y + halfSize);
        bottomLeft = new Vector2(position.x - halfSize, position.y - halfSize);
        bottomRight = new Vector2(position.x + halfSize, position.y - halfSize);

        UpdateType(type, liquid);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateUI();
        DebugDraw();
    }

    void ClearUI()
    {
        showUI = false;
        uiLiquid = null;

        UIController.Instance.tooltip.Clear();
    }

    void StartUI()
    {
        showUI = true;

        UIController.Instance.tooltip.Clear();
        UIController.Instance.tooltip.SetText("tile", "Tile: d");
        UIController.Instance.tooltip.SetText("position", "Position:" + (x + ":" + y));
        uiLiquid = UIController.Instance.tooltip.SetText("liquid", "Liquid: " + liquid);

        if (type == TileType.Blank)
        {
            UIController.Instance.tooltip.SetButton("solid", "Solid", delegate { UpdateType(TileType.Solid); });
            UIController.Instance.tooltip.SetButton("add_liquid", "Add Liquid", delegate { UpdateType(TileType.Blank, 1.0f); });
        }
        else if (type == TileType.Solid)
        {
            UIController.Instance.tooltip.SetButton("blank", "Blank", delegate { UpdateType(TileType.Blank); });
        }
    }

    void UpdateUI()
    {
        if (!showUI) return; 

        uiLiquid.text = "Liquid: " + liquid;
    }

    void DebugDraw()
    {
        Debug.DrawLine(topLeft, topRight); // top
        Debug.DrawLine(bottomLeft, bottomRight); // bottom

        Debug.DrawLine(topLeft, bottomLeft); // left
        Debug.DrawLine(topRight, bottomRight); // right
    }

    void ChangeToSolid()
    {
        type = TileType.Solid;

        spriteRenderer.sprite = spriteSolid;

        liquid = 0.0f;

        SpriteLiquidUpdate();
    }

    void ChangeToBlank(float amount)
    {
        type = TileType.Blank;

        spriteRenderer.sprite = spriteLiquid;

        liquid = amount;

        SpriteLiquidUpdate(liquid);
    }

    public void SpriteLiquidUpdate(float amount = 1.0f)
    {
        float scale = 1.0f;

        switch (type)
        {
            case TileType.Blank:
                scale = amount;
                break;
            case TileType.Solid:
                scale = 1.0f;
                break;
        }

        Vector3 localScale = spriteRenderer.transform.localScale;
        localScale.y = scale;

        spriteRenderer.transform.localScale = localScale;

        Vector3 position = spriteRendererOriginalPosition;

        if (scale < 1.0f && scale > 0.0f)
        {
            position.y += -(1 - scale) * 0.5f;
        }

        spriteRenderer.transform.position = position;
    }

    public void UpdateType(TileType toType, float contents = 0.0f)
    {
        switch (toType)
        {
            case TileType.Solid:
                ChangeToSolid();
                break;
            case TileType.Blank:
                ChangeToBlank(contents);
                break;
        }
    }
}
