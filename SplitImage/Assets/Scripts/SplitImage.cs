using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curves;
using System.Linq;

public class SplitImage : MonoBehaviour
{
    public string mImageFilename;
    public SpriteRenderer mSpriteRenderer;
    public Sprite mSprite;
    public LineRenderer mBezierCurve;

    Vec2[] mCurvyCoords = new Vec2[]
    {
        new Vec2(0, 0),
        new Vec2(35, 15),
        new Vec2(37, 5),
        new Vec2(37, 5),
        new Vec2(40, 0),
        new Vec2(38, -5),
        new Vec2(38, -5),
        new Vec2(20, -20),
        new Vec2(50, -20),
        new Vec2(50, -20),
        new Vec2(80, -20),
        new Vec2(62, -5),
        new Vec2(62, -5),
        new Vec2(60, 0),
        new Vec2(63, 5),
        new Vec2(63, 5),
        new Vec2(65, 15),
        new Vec2(100, 0)
    };

    void CreateSprite()
    {
        Texture2D tex = SpriteUtils.LoadTexture("Images/" + mImageFilename);
        if (tex == null) return;

        int w = tex.width > 140 ? 140 : tex.width;
        int h = tex.height > 140 ? 140 : tex.height;
        Sprite sprite = SpriteUtils.LoadNewSprite(tex, 0, 0, w, h);

        mSpriteRenderer.sprite = sprite;
    }

    int GetInterpolatedY(List<Vector3> mBezierPoints, int x)
    {
        for(int i = 1; i < mBezierPoints.Count; ++i)
        {
            if(mBezierPoints[i].x >= x)
            {
                float x1 = mBezierPoints[i - 1].x;
                float x2 = mBezierPoints[i].x;

                float y1 = mBezierPoints[i - 1].y;
                float y2 = mBezierPoints[i].y;

                float y = (x - x1) * (y2 - y1) / (x2 - x1) + y1;
                return (int)y;
            }
        }
        return (int)mBezierPoints[mBezierPoints.Count - 1].y;
    }

    public enum Direction
    {
        UP,
        UP_REVERSE,
        RIGHT,
        RIGHT_REVERSE,
        DOWN,
        DOWN_REVERSE,
        LEFT,
        LEFT_REVERSE,
        NONE,
    }

    Direction GetRandomDirection(int side)
    {
        float rand = Random.Range(0.0f, 1.0f);
        switch(side)
        {
            case 0:
                {
                    if (rand < 0.5f) return Direction.UP;
                    else return Direction.UP_REVERSE;
                }
            case 1:
                {
                    if (rand < 0.5f) return Direction.RIGHT;
                    else return Direction.RIGHT_REVERSE;
                }
            case 2:
                {
                    if (rand < 0.5f) return Direction.DOWN;
                    else return Direction.DOWN_REVERSE;
                }
            case 3:
                {
                    if (rand < 0.5f) return Direction.LEFT;
                    else return Direction.LEFT_REVERSE;
                }
        }
        return Direction.UP;
    }

    List<Vector3> mBezierPoints = new List<Vector3>();
    Texture2D mBaseTexture;

    Color trans = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    GameObject[,] mGameObjects;

    private int mTilesX;
    private int mTilesY;

    void SetupLineRenderer()
    {
        // show the bezier curve.
        mBezierCurve.material = new Material(Shader.Find("Sprites/Default"));
        mBezierCurve.startWidth = 0.1f;
        mBezierCurve.endWidth = 0.1f;

        for (int i = 0; i < 100; i++)
        {
            mBezierCurve.SetPosition(i, mBezierPoints[i] + new Vector3(20, 20, 0.0f));
        }
    }

    void CreateBezierCurve()
    {
        // use bezier curve.
        Bezier bez = new Bezier(mCurvyCoords.OfType<Vec2>().ToList());

        for (int i = 0; i < 100; i++)
        {
            Vec2 bp = bez.ValueAt(i / 100.0f);
            Vector3 p = new Vector3(bp.x, bp.y, 0.0f);

            mBezierPoints.Add(p);
        }
    }

    Texture2D CreateTileTexture(int indx, int indy)
    {
        int w = 140;
        int h = 140;

        Texture2D new_tex = new Texture2D(w, h, TextureFormat.ARGB32, 1, true);

        int startX = indx * 100;
        int startY = indy * 100;
        for (int i = 0; i < 140; ++i)
        {
            for (int j = 0; j < 140; ++j)
            {
                Color color = mBaseTexture.GetPixel(i + startX, j + startY);
                new_tex.SetPixel(i, j, color);
                if (i < 20 && j < 20)
                {
                    new_tex.SetPixel(i, j, trans);
                }
                if (i >= 120 && j < 20)
                {
                    new_tex.SetPixel(i, j, trans);
                }
                if (i >= 120 && j >= 120)
                {
                    new_tex.SetPixel(i, j, trans);
                }
                if (i < 20 && j >= 120)
                {
                    new_tex.SetPixel(i, j, trans);
                }
            }
        }
        return new_tex;
    }

    void CreateSpriteGameObject(int i, int j)
    {
        GameObject obj = new GameObject();
        obj.name = "Tile_" + i.ToString() + "_" + j.ToString();
        SplitTile tile = obj.AddComponent<SplitTile>();
        tile.mIndex = new Vector2Int(i, j);
        mGameObjects[i, j] = obj;

        SpriteRenderer spren = obj.AddComponent<SpriteRenderer>();

        obj.transform.position = new Vector3(i * 100, j * 100, 0.0f);

        // create a new tile texture.
        Texture2D mTileTexture = CreateTileTexture(i, j);

        tile.mDirections[0] = GetRandomDirection(0);
        tile.mDirections[1] = GetRandomDirection(1);
        tile.mDirections[2] = GetRandomDirection(2);
        tile.mDirections[3] = GetRandomDirection(3);

        // check for bottom and left tile.
        if (j > 0)
        {
            SplitTile downTile = mGameObjects[i, j-1].GetComponent<SplitTile>();
            if(downTile.mDirections[0] == Direction.UP)
            {
                tile.mDirections[2] = Direction.DOWN_REVERSE;
            }
            else
            {
                tile.mDirections[2] = Direction.DOWN;
            }
        }

        // check for bottom and left tile.
        if (i > 0)
        {
            SplitTile downTile = mGameObjects[i - 1, j].GetComponent<SplitTile>();
            if (downTile.mDirections[1] == Direction.RIGHT)
            {
                tile.mDirections[3] = Direction.LEFT_REVERSE;
            }
            else
            {
                tile.mDirections[3] = Direction.LEFT;
            }
        }

        if (i == 0)
        {
            tile.mDirections[3] = Direction.NONE;
        }
        if (i == mTilesX - 1)
        {
            tile.mDirections[1] = Direction.NONE;
        }
        if (j == 0)
        {
            tile.mDirections[2] = Direction.NONE;
        }
        if (j == mTilesY - 1)
        {
            tile.mDirections[0] = Direction.NONE;
        }
        for (int d = 0; d < tile.mDirections.Length; ++d)
        {
            if(tile.mDirections[d] != Direction.NONE)
            ApplyBezierMask(mTileTexture, tile.mDirections[d]);
        }

        mTileTexture.Apply();

        // Set the tile texture to the sprite.
        Sprite sprite = SpriteUtils.LoadNewSprite(mTileTexture, 0, 0, 140, 140);
        spren.sprite = sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load the main image.
        Texture2D tex = SpriteUtils.LoadTexture("Images/" + mImageFilename);
        if (!tex.isReadable)
        {
            Debug.Log("Texture is not readable");
            return;
        }

        mTilesX = tex.width / 100;
        mTilesY = tex.height / 100;

        // add 20 pixel border around.
        Texture2D new_tex = new Texture2D(tex.width + 40, tex.height + 40, TextureFormat.ARGB32, 1, true);
        for(int i = 20; i < tex.width + 20; ++i)
        {
            for (int j = 20; j < tex.height + 20; ++j)
            {
                Color col = tex.GetPixel(i - 20, j - 20);
                col.a = 1.0f;
                new_tex.SetPixel(i, j, col);
            }
        }
        new_tex.Apply();
        mBaseTexture = new_tex;

        // create the bezier curve.
        CreateBezierCurve();

        mGameObjects = new GameObject[mTilesX, mTilesY];
        for (int i = 0; i < mTilesX; ++i)
        {
            for (int j = 0; j < mTilesY; ++j)
            {
                CreateSpriteGameObject(i, j);
            }
        }

        // now make the background image light transparent.
        for (int i = 20; i < tex.width + 20; ++i)
        {
            for (int j = 20; j < tex.height + 20; ++j)
            {
                Color col = tex.GetPixel(i - 20, j - 20);
                col.a = 0.2f;
                new_tex.SetPixel(i, j, col);
            }
        }
        new_tex.Apply();
        mBaseTexture = new_tex;

        Sprite sprite = SpriteUtils.LoadNewSprite(mBaseTexture, 0, 0, mBaseTexture.width, mBaseTexture.height);
        mSpriteRenderer.sprite = sprite;

        RelocateCamera();
    }

    void RelocateCamera()
    {
        Camera.main.orthographicSize = 1.2f * mBaseTexture.height / 2.0f;
        Camera.main.transform.position = new Vector3(mBaseTexture.width / 2.0f, mBaseTexture.height / 2.0f, -10.0f);
    }

    void ApplyBezierMask(Texture2D mTileTexture, Direction dir)
    {
        switch(dir)
        {
            case Direction.UP:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = -GetInterpolatedY(mBezierPoints, i);

                        for (int j = 120 + y; j < 140; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                        mTileTexture.SetPixel(i + 20, 120 + y, Color.gray);
                    }
                    break;
                }
            case Direction.UP_REVERSE:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = GetInterpolatedY(mBezierPoints, i);

                        for (int j = 120 + y; j < 140; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                        mTileTexture.SetPixel(i + 20, 120 + y, Color.gray);
                    }
                    break;
                }
            case Direction.RIGHT:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = -GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(120 + x, j + 20, Color.gray);
                        for (int i = 119 + x; i < 140; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
            case Direction.RIGHT_REVERSE:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(120 + x, j + 20, Color.gray);
                        for (int i = 121 + x; i < 140; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
            case Direction.DOWN:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = GetInterpolatedY(mBezierPoints, i);

                        //mTileTexture.SetPixel(i + 20, y + 20, trans);
                        for (int j = 0; j < y + 19; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                    }
                    break;
                }
            case Direction.DOWN_REVERSE:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = -GetInterpolatedY(mBezierPoints, i);

                        //mTileTexture.SetPixel(i + 20, y + 20, trans);
                        for (int j = 0; j < y + 19; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                    }
                    break;
                }
            case Direction.LEFT:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(x + 20, j, trans);
                        for (int i = 0; i < x + 19; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
            case Direction.LEFT_REVERSE:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = -GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(x + 20, j + 20, trans);
                        for (int i = 0; i < x + 21; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
        }
    }

    private void Awake()
    {
        //const int initialSeed = 1;
        //Random.InitState(initialSeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
