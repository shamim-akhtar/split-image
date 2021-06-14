using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Linq;

public class SplitImage : MonoBehaviour
{
    public string mImageFilename;
    public SpriteRenderer mSpriteRenderer;
    public Sprite mSprite;
    public LineRenderer mBezierCurve;

    Utils.Vec2[] mCurvyCoords = new Utils.Vec2[]
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

    int GetInterpolatedY(List<Vector3> points, int x)
    {
        for(int i = 1; i < points.Count; ++i)
        {
            if(points[i].x >= x)
            {
                float x1 = points[i - 1].x;
                float x2 = points[i].x;

                float y1 = points[i - 1].y;
                float y2 = points[i].y;

                float y = (x - x1) * (y2 - y1) / (x2 - x1) + y1;
                return (int)y;
            }
        }
        return (int)points[points.Count - 1].y;
    }

    // Start is called before the first frame update
    void Start()
    {
        int offset_x = 20;
        int offset_y = 20;

        // show the bezier curve.
        mBezierCurve.material = new Material(Shader.Find("Sprites/Default"));
        mBezierCurve.startWidth = 0.1f;
        mBezierCurve.endWidth = 0.1f;

        // use bezier curve.
        Bezier bez = new Bezier(mCurvyCoords.OfType<Vec2>().ToList());
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < 100; i++)
        {
            Vec2 bp = bez.ValueAt(i / 100.0f);
            Vector3 p = new Vector3(bp.x, bp.y, 0.0f);

            points.Add(p);
            mBezierCurve.SetPosition(i, p + new Vector3(offset_x, offset_y, 0.0f));
        }

        Texture2D texture = SpriteUtils.LoadTexture("Images/" + mImageFilename);

        Texture2D new_tex = new Texture2D(140, 140, TextureFormat.ARGB32, 1, true);

        Color trans = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        if (!texture.isReadable)
        {
            Debug.Log("Texture is not readable");
        }
        for (int i = 0; i < 140; ++i)
        {
            for (int j = 0; j <140; ++j)
            {
                Color color = texture.GetPixel(i, j);
                new_tex.SetPixel(i, j, color);
                if(i < 20 && j < 20)
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


        //// Bottom.
        //for (int i = 0; i < 100; ++i)
        //{
        //    int y = GetInterpolatedY(points, i);

        //    for (int j = 0; j < y + offset_y; ++j)
        //    {
        //        new_tex.SetPixel(i+offset_x, j, trans);
        //    }
        //}
        // Bottom reverse
        for (int i = 0; i < 100; ++i)
        {
            int y = -GetInterpolatedY(points, i);

            for (int j = 0; j < y + offset_y; ++j)
            {
                new_tex.SetPixel(i + offset_x, j, trans);
            }
        }

        //left
        //for (int j = 0; j < 100; ++j)
        //{
        //    int x = GetInterpolatedY(points, j);

        //    for (int i = 0; i < x + offset_x; ++i)
        //    {
        //        new_tex.SetPixel(i, j + offset_y, trans);
        //    }
        //}

        //left reverse
        for (int j = 0; j < 100; ++j)
        {
            int x = -GetInterpolatedY(points, j);

            for (int i = 0; i < x + offset_x; ++i)
            {
                new_tex.SetPixel(i, j + offset_y, trans);
            }
        }

        //up
        //for (int i = 0; i < 100; ++i)
        //{
        //    int y = -GetInterpolatedY(points, i);

        //    for (int j = 120 + y; j < 140; ++j)
        //    {
        //        new_tex.SetPixel(i + offset_x, j, trans);
        //    }
        //}

        // up reverse
        for (int i = 0; i < 100; ++i)
        {
            int y = GetInterpolatedY(points, i);

            for (int j = 120 + y; j < 140; ++j)
            {
                new_tex.SetPixel(i + offset_x, j, trans);
            }
        }

        //right
        //for (int j = 0; j < 100; ++j)
        //{
        //    int x = -GetInterpolatedY(points, j);

        //    for (int i = 120+x; i < 140; ++i)
        //    {
        //        new_tex.SetPixel(i, j + offset_y, trans);
        //    }
        //}
        //right reverse
        for (int j = 0; j < 100; ++j)
        {
            int x = GetInterpolatedY(points, j);

            for (int i = 120 + x; i < 140; ++i)
            {
                new_tex.SetPixel(i, j + offset_y, trans);
            }
        }

        new_tex.Apply();

        Sprite sprite = SpriteUtils.LoadNewSprite(new_tex, 0, 0, 140, 140);
        mSpriteRenderer.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
