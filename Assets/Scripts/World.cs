using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {
    public int lengthX = 16;
    public int lengthY = 16;
    public float tileSize = 1;
    public GameObject tilePrefab;

    float offsetX;
    float offsetY;
    Tile[,] tiles;
    float[,] diffs;
    int[,] map;

    Tile selectedTile;

    float maxFlow = 0.25f;

    // Use this for initialization
    void Start () {
        tiles = new Tile[lengthX, lengthY];
        diffs = new float[lengthX, lengthY];

        map = new int[,] {
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0 },
            { 0,0,0,0,1,0,0,0,0,1,1,0,0,0,0,0 },
            { 0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
        };

        offsetX = tileSize * 0.5f;
        offsetY = tileSize * 0.5f;

        InstantiateTiles();
        SetTileNeighbors();
        InvokeRepeating("Simulate", 0, 0.1f);
    }

    void InstantiateTiles()
    {
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                Vector2 position = new Vector2(x * tileSize + offsetX, y * tileSize + offsetY);

                GameObject obj = Instantiate(tilePrefab, position, Quaternion.identity, transform);

                obj.name = "Tile " + x + ":" + y;

                Tile tile = obj.GetComponent<Tile>();

                tile.size = tileSize;
                tile.x = x;
                tile.y = y;
                tile.type = MapToTile(x, y);

                tiles[x, y] = tile;
            }
        }
    }

    TileType MapToTile(int x, int y)
    {
        int mapX = x;
        int mapY = lengthY - y - 1;

        return (TileType)map[mapY, mapX];
    }

    void SetTileNeighbors()
    {
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                // top
                if (y + 1 < lengthY)
                    tiles[x, y].top = tiles[x, y + 1];

                // bottom
                if (y - 1 >= 0)
                    tiles[x, y].bottom = tiles[x, y - 1];

                // left
                if (x - 1 >= 0)
                    tiles[x, y].left = tiles[x - 1, y];

                // right
                if (x + 1 < lengthX)
                    tiles[x, y].right = tiles[x + 1, y];
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            position.x -= offsetX;
            position.y -= offsetY;

            int x = (int)Mathf.Round(position.x / tileSize);
            int y = (int)Mathf.Round(position.y / tileSize);

            if (x < 0 || x > lengthX - 1) return;
            if (y < 0 || y > lengthY - 1) return;

            if (selectedTile != null)
            {
                selectedTile.ShowUI = false;
            }

            selectedTile = tiles[x, y];

            selectedTile.ShowUI = true;
        }
    }

    void Simulate()
    {
        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                diffs[x, y] = 0.0f;
            }
        }

        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                Tile tile = tiles[x, y];

                if (tile.type == TileType.Solid) continue;
                //if (tile.isSettled) continue;
                //if (tile.liquid < 0.01f) continue;

                float flow;
                float remainingLiquid = tile.liquid;

                if (tile.bottom != null && tile.bottom.type == TileType.Blank)
                {
                    if (tile.bottom.liquid < 0.01f)
                    {
                        flow = tile.liquid;

                        remainingLiquid -= flow;
                        diffs[x, y] -= flow;
                        diffs[x, y - 1] += flow;
                    }
                    else
                    {
                        flow = Mathf.Min(1.0f - tile.bottom.liquid, remainingLiquid);

                        // 1 - tile.bottom.liquid = empty part of tile

                        // sum of remainingLiquid + tile.bottom.liquid (what is above 1.0 is pressure)

                        remainingLiquid -= flow;
                        diffs[x, y] -= flow;
                        diffs[x, y - 1] += flow;

                        // tag for pressure set (the rest of the liquid)
                        // presure check should be after left/right spread
                        // top movement is just pressure movement
                    }
                }

                if (remainingLiquid > 0f && tile.left != null && tile.left.type == TileType.Blank)
                {
                    flow = (remainingLiquid - tile.left.liquid) / 4f;
                    flow = Mathf.Max(flow, 0f);

                    remainingLiquid -= flow;
                    diffs[x, y] -= flow;
                    diffs[x - 1, y] += flow;
                }

                if (remainingLiquid > 0f && tile.right != null && tile.right.type == TileType.Blank)
                {
                    flow = (remainingLiquid - tile.right.liquid) / 3f;
                    flow = Mathf.Max(flow, 0f);

                    remainingLiquid -= flow;
                    diffs[x, y] -= flow;
                    diffs[x + 1, y] += flow;
                }
            }
        }

        for (int y = 0; y < lengthY; y++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                Tile tile = tiles[x, y];

                tile.liquid += diffs[x, y];

                float spriteLiquid = tile.liquid;

                if (tile.liquid > 0.01f && tile.top != null && tile.top.type == TileType.Blank && tile.top.liquid > 0.01f)
                {
                    spriteLiquid = 1.0f;
                }

                tile.SpriteLiquidUpdate(spriteLiquid);
                
                if (tile.liquid < 0.01f)
                {
                    tile.isSettled = true;
                } else
                {
                    tile.isSettled = false;
                }
            }
        }
    }
}
