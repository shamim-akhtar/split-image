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
        // use bezier curve.
        Bezier bez = new Bezier(mCurvyCoords.OfType<Vec2>().ToList());

        // show the bezier curve.
        mBezierCurve.material = new Material(Shader.Find("Sprites/Default"));
        mBezierCurve.startWidth = 0.1f;
        mBezierCurve.endWidth = 0.1f;

        List<Vector3> points = new List<Vector3>();

        Vector3 offset = new Vector3(20.0f, 20.0f, 0.0f);
        for (int i = 0; i < 100; i++)
        {
            Vec2 bp = bez.ValueAt(i / 100.0f);
            Vector3 p = new Vector3(bp.x, bp.y, 0.0f);
            p = p + offset;
            points.Add(p);
            mBezierCurve.SetPosition(i, p);
        }

        Texture2D texture = SpriteUtils.LoadTexture("Images/" + mImageFilename);
        if (!texture.isReadable)
        {
            Debug.Log("Texture is not readable");
        }
        // align the bezier points to continuous pixels.
        for (int i = 20; i < 120; ++i)
        {
            int y = GetInterpolatedY(points, i);
            Debug.Log("Y: " + y);
            for (int j = 0; j < y; ++j)
            {
                Color color = texture.GetPixel(i, j);
                color.r = 1.0f;
                color.g = 0.0f;
                color.b = 0.0f;
                color.a = 1.0f;
                texture.SetPixel(i, j, color);
            }
        }

        texture.Apply();

        Sprite sprite = SpriteUtils.LoadNewSprite(texture, 0, 0, 140, 140);
        mSpriteRenderer.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
