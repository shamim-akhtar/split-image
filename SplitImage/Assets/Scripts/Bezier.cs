using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    static class Constants
    {
        public const float BEZIER_FUZZY_EPSILON = 0.0001f;
        public const int BEZIER_DEFAULT_INTERVALS = 10;
        public const int BEZIER_DEFAULT_MAX_ITERATIONS = 15;
        public const float PI = 3.14159265358f;
    }

    static public class Math
    {
        public static int Binomial(int n, int k)
        {
            int val = 1;
            for (int i = 1; i <= k; i++)
            {
                val *= n + 1 - i;
                val /= i;
            }
            return val;
        }

        public static bool IsWithinZeroAndOne(float x)
        {
            return x >= -Constants.BEZIER_FUZZY_EPSILON && x <= (1.0 + Constants.BEZIER_FUZZY_EPSILON);
        }
    }

    public class BinomialCoefficients
    {
        private List<int> mCoefficients;
        private int N = 0;
        public BinomialCoefficients(int n)
        {
            N = n;
            int center = N / 2;
            int k = 0;

            mCoefficients = new List<int>(N + 1);
            for(int i = 0; i < N + 1; ++i)
            {
                mCoefficients.Add(0);
            }

            while (k <= center)
            {
                mCoefficients[k] = Math.Binomial(N, k);
                k++;
            }

            // Utilize the symmetrical nature of the binomial coefficients.
            while (k <= N)
            {
                mCoefficients[k] = mCoefficients[N - k];
                k++;
            }
        }

        public int size()
        {
            return N + 1;
        }

        public int this[int idx]
        {
            get { return mCoefficients[idx]; }
        }

    }

    public class PolynomialPair
    {
        public int t = 0;
        public int one_minus_t = 0;

        public float valueAt(float tt)
        {
            return (UnityEngine.Mathf.Pow(1.0f - tt, one_minus_t) * UnityEngine.Mathf.Pow(tt, (float)(t)));
        }
    };

    public class PolynomialCoefficients
    {
        private int N = 0;
        private List<PolynomialPair> mPolynomialPairs;
        public PolynomialCoefficients(int n)
        {
            N = n;
            mPolynomialPairs = new List<PolynomialPair>(N + 1);
            for (int i = 0; i < N + 1; ++i)
            {
                mPolynomialPairs.Add(new PolynomialPair());
            }
            for (int i = 0; i <= N; i++)
            {
                mPolynomialPairs[i].t = i;
                mPolynomialPairs[i].one_minus_t = N - i;
            }
        }

        public float valueAt(int pos, float t)
        {
            return mPolynomialPairs[pos].valueAt(t);
        }

        public PolynomialPair this[int idx]
        {
            get { return mPolynomialPairs[idx]; }
        }
    }

    public class Vec2
    {
        public float x = 0.0f;
        public float y = 0.0f;
        public Vec2()
        { }

        public Vec2(float xx, float yy)
        {
            x = xx;
            y = yy;
        }
        public Vec2(Vec2 other)
        {
            x = other.x;
            y = other.y;
        }

        public Vec2(float xx, float yy, bool normalize)
        {
            x = xx;
            y = yy;
            if (normalize)
            {
                Normalize();
            }
        }


        public void Set(float xx, float yy)
        {
            x = xx;
            y = yy;
        }

        public void Set(Vec2 other)
        {
            x = other.x;
            y = other.y;
        }

        public float Length()
        {
            return UnityEngine.Mathf.Sqrt(x * x + y * y);
        }

        public void Normalize()
        {
            float len = Length();
            x /= len;
            y /= len;
        }

        public void Translate(float dx, float dy)
        {
            x += dx;
            y += dy;
        }

        public void Translate(Vec2 distance)
        {
            x += distance.x;
            y += distance.y;
        }

        public void Rotate(float angle, Vec2 pivot)
        {
            float s = UnityEngine.Mathf.Sin(angle);
            float c = UnityEngine.Mathf.Cos(angle);

            x -= pivot.x;
            y -= pivot.y;

            float xnew = x * c - y * s;
            float ynew = x * s + y * c;

            x = xnew + pivot.x;
            y = ynew + pivot.y;
        }

        public float Angle()
        {
            return UnityEngine.Mathf.Atan2(y, x);
        }

        public float AngleDeg()
        {
            return Angle() * 180.0f / Constants.PI;
        }

        public float this[int axis]
        {
            get
            { 
                switch (axis)
                {
                case 0:
                    return x;
                case 1:
                    return y;
                default:
                    return 0;
                }
            }
            set
            {
                switch (axis)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        x = value;
                        break;
                }
            }
        }


        public bool FuzzyEquals(Vec2 other)
        {
            bool equals = true;
            for (int axis = 0; axis<Vec2.size; axis++)
            {
                if (UnityEngine.Mathf.Abs((this)[axis] - other [axis]) >= Constants.BEZIER_FUZZY_EPSILON)
                {
                    equals = false;
                    break;
                }
            }
            return equals;
        }


        public bool IsWithinZeroAndOne()
        {
            return Math.IsWithinZeroAndOne(x) && Math.IsWithinZeroAndOne(y);
        }

        static public Vec2 operator + (Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        static public Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        static public Vec2 operator *(Vec2 a, int val)
        {
            return new Vec2(a.x * val, a.y * val);
        }

        public static int size = 2;
    };

    public class ExtremeValue
    {
        private float t;
        private int axis;

        public ExtremeValue(float t_, int axis_)
        {
            t = t_;
            axis = axis_;
        }

        public bool FuzzyEquals(ExtremeValue other)
        {
            return axis == other.axis && Mathf.Abs(t - other.t) < Constants.BEZIER_FUZZY_EPSILON;
        }
    };

    //    class ExtremeValues
    //{
    //    public:
    //        bool add(float t, int axis)
    //    {
    //        return add(ExtremeValue(t, axis));
    //    }

    //    bool add(const ExtremeValue& val)
    //        {
    //            assert(Math::isWithinZeroAndOne(val.t));
    //            for (auto const& v : values)
    //            {
    //                if (val.fuzzyEquals(v))
    //                    return false;
    //            }
    //values.push_back(val);
    //            return true;
    //        }

    //        int size() const
    //        {
    //            return values.size();
    //        }

    //        ExtremeValue& operator[] (int idx)
    //{
    //    assert(idx < values.size());
    //    return values[idx];
    //}

    //ExtremeValue operator[](int idx) const
    //        {
    //            assert(idx<values.size());
    //            return values[idx];
    //        }

    //    private:
    //        std::vector<ExtremeValue> values;
    //    };

    //    class ExtremeVec2s
    //{
    //    public:
    //        bool add(float x, float y)
    //    {
    //        return add(Vec2(x, y));
    //    }

    //    bool add(const Vec2& extremeVec2)
    //        {
    //            for (auto const& ep : points)
    //            {
    //                if (extremeVec2.fuzzyEquals(ep))
    //                    return false;
    //            }
    //points.push_back(extremeVec2);
    //            return true;
    //        }

    //        int size() const
    //        {
    //            return points.size();
    //        }

    //        bool empty() const
    //        {
    //            return !size();
    //        }

    //        Vec2& operator[] (int idx)
    //{
    //    assert(idx < size());
    //    return points[idx];
    //}

    //Vec2 operator[](int idx) const
    //        {
    //            assert(idx<size());
    //            return points[idx];
    //        }

    //    private:
    //        std::vector<Vec2> points;
    //    };

    //    class AxisAlignedBoundingBox
    //{
    //    public:
    //        AxisAlignedBoundingBox(const Vec2& p0, const Vec2& p1, const Vec2& p2, const Vec2& p3)
    //            : points{ {p0
    //}, {p1}, {p2}, {p3} }
    //        {}

    //        AxisAlignedBoundingBox(const ExtremeVec2s& xVec2s)
    //{
    //    float minX = std::numeric_limits<float>::max();
    //    float maxX = -std::numeric_limits<float>::max();
    //    float minY = std::numeric_limits<float>::max();
    //    float maxY = -std::numeric_limits<float>::max();

    //    for (int i = 0; i < xVec2s.size(); i++)
    //    {
    //        if (xVec2s[i].x > maxX)
    //            maxX = xVec2s[i].x;
    //        if (xVec2s[i].x < minX)
    //            minX = xVec2s[i].x;
    //        if (xVec2s[i].y > maxY)
    //            maxY = xVec2s[i].y;
    //        if (xVec2s[i].y < minY)
    //            minY = xVec2s[i].y;
    //    }

    //    points[0].set(minX, minY);
    //    points[1].set(minX, maxY);
    //    points[2].set(maxX, maxY);
    //    points[3].set(maxX, minY);
    //}

    //static constexpr int size()
    //{
    //    return 4;
    //}

    //float minX() const
    //        {
    //            return points[0].x;
    //        }

    //        float maxX() const
    //        {
    //            return points[2].x;
    //        }

    //        float minY() const
    //        {
    //            return points[0].y;
    //        }

    //        float maxY() const
    //        {
    //            return points[2].y;
    //        }

    //        float width() const
    //        {
    //            return maxX() - minX();
    //        }

    //        float height() const
    //        {
    //            return maxY() - minY();
    //        }

    //        float area() const
    //        {
    //            return width() * height();
    //        }

    //        Vec2& operator[] (int idx)
    //{
    //    assert(idx < size());
    //    return points[idx];
    //}

    //Vec2 operator[](int idx) const
    //        {
    //            assert(idx<size());
    //            return points[idx];
    //        }

    //    private:
    //        Vec2 points[4]; // Starting in lower left corner, going clock-wise.
    //    };

    //    typedef AxisAlignedBoundingBox AABB;

    //    class TightBoundingBox
    //{
    //    public:
    //        // Takes the ExtremeVec2s of the Bezier curve moved to origo and rotated to align the x-axis
    //        // as arguments as well as the translation/rotation used to calculate it.
    //        TightBoundingBox(const ExtremeVec2s& xVec2s, const Vec2& translation, float rotation)
    //        {
    //            float minX = std::numeric_limits<float>::max();
    //    float maxX = -std::numeric_limits<float>::max();
    //    float minY = std::numeric_limits<float>::max();
    //    float maxY = -std::numeric_limits<float>::max();

    //            for (int i = 0; i<xVec2s.size(); i++)
    //            {
    //                if (xVec2s[i].x > maxX)
    //                    maxX = xVec2s[i].x;
    //                if (xVec2s[i].x<minX)
    //                    minX = xVec2s[i].x;
    //                if (xVec2s[i].y > maxY)
    //                    maxY = xVec2s[i].y;
    //                if (xVec2s[i].y<minY)
    //                    minY = xVec2s[i].y;
    //            }

    //points[0].set(minX, minY);
    //points[1].set(minX, maxY);
    //points[2].set(maxX, maxY);
    //points[3].set(maxX, minY);

    //            if (xVec2s.empty())
    //                return;

    //            for (int i = 0; i< 4; i++)
    //            {
    //                points[i].rotate(-rotation);
    //points[i].translate(-translation);
    //            }
    //        }

    //        static constexpr int size()
    //{
    //    return 4;
    //}

    //float minX() const
    //        {
    //            return std::min({ points[0].x, points[1].x, points[2].x, points[3].x });
    //        }

    //        float maxX() const
    //        {
    //            return std::max({ points[0].x, points[1].x, points[2].x, points[3].x });
    //        }

    //        float minY() const
    //        {
    //            return std::min({ points[0].y, points[1].y, points[2].y, points[3].y });
    //        }

    //        float maxY() const
    //        {
    //            return std::max({ points[0].y, points[1].y, points[2].y, points[3].y });
    //        }

    //        float area() const
    //        {
    //            return width() * height();
    //        }

    //        // Uses the two first points to calculate the "width".
    //        float width() const
    //        {
    //            float x = points[1].x - points[0].x;
    //float y = points[1].y - points[0].y;
    //            return sqrt(x* x + y* y);
    //        }

    //        // Uses the second and third points to calculate the "height".
    //        float height() const
    //        {
    //            float x = points[2].x - points[1].x;
    //float y = points[2].y - points[1].y;
    //            return sqrt(x* x + y* y);
    //        }

    //        Vec2& operator[] (int idx)
    //{
    //    assert(idx < size());
    //    return points[idx];
    //}

    //Vec2 operator[](int idx) const
    //        {
    //            assert(idx<size());
    //            return points[idx];
    //        }

    //    private:
    //        Vec2 points[4]; // The points are ordered in a clockwise manner.
    //    };

    //    typedef TightBoundingBox TBB;

    public class Bezier
    {
        public int N { get; set; } = 0;
        protected List<Vec2> mControlVec2s;
        BinomialCoefficients binomialCoefficients;
        PolynomialCoefficients polynomialCoefficients;
        //public class Split
        //{
        //    Split(List<Vec2> l, List<Vec2> r)
        //    {
        //        left = new Bezier()
        //            : left(l, M + 1)
        //            , right(r, M + 1)
        //    }

        //    Bezier left;
        //    Bezier right;
        //};

        public Bezier(int N_)
        {
            N = N_;
            mControlVec2s = new List<Vec2>(N + 1);
            binomialCoefficients = new BinomialCoefficients(N);
            polynomialCoefficients = new PolynomialCoefficients(N);
        }

        public Bezier(List<Vec2> controlVec2s)
        {
            N = controlVec2s.Count - 1;
            binomialCoefficients = new BinomialCoefficients(N);
            polynomialCoefficients = new PolynomialCoefficients(N);
            mControlVec2s = new List<Vec2>(N + 1);

            for (int i = 0; i < controlVec2s.Count; i++)
                mControlVec2s.Add(new Vec2(controlVec2s[i]));
        }

        public Bezier(Bezier other)
        {
            N = other.N;
            binomialCoefficients = new BinomialCoefficients(N);
            polynomialCoefficients = new PolynomialCoefficients(N);
            mControlVec2s = new List<Vec2>(N + 1);
            for (int i = 0; i < other.mControlVec2s.Count; i++)
                mControlVec2s.Add(new Vec2(other[i]));
        }

        public Vec2 this[int idx]
        {
            get { return mControlVec2s[idx]; }
        }

        // The order of the bezier curve.
        public int Order()
        {
            return N;
        }

        // Number of control points.
        public int Size()
        {
            return N + 1;
        }

        public Bezier Derivative()
        {
            // Note: derivative weights/control points are not actual control points.
            List<Vec2> derivativeWeights = new List<Vec2>(N);
            for (int i = 0; i < N; i++)
                derivativeWeights.Add(new Vec2((mControlVec2s[i + 1] - mControlVec2s[i]) * N));

            return new Bezier(derivativeWeights);
        }

        public float ValueAt(float t, int axis)
        {
            float sum = 0;
            for (int n = 0; n < N + 1; n++)
            {
                sum += binomialCoefficients[n] * polynomialCoefficients[n].valueAt(t) * mControlVec2s[n][axis];
            }
            return sum;
        }

        public Vec2 ValueAt(float t)
        {
            Vec2 p = new Vec2();
            p.x = (float)ValueAt(t, 0);
            p.y = (float)ValueAt(t, 1);
            return p;
        }
    }

//        Tangent tangentAt(float t, bool normalize = true)
//        {
//            Vec2 p;
//Bezier<N - 1> derivative = this->derivative();
//p.set(derivative.valueAt(t));
//            if (normalize)
//                p.normalize();
//            return p;
//        }

//        Normal normalAt(float t, bool normalize = true) const
//        {
//            Vec2 tangent = tangentAt(t, normalize);
//            return Normal(-tangent.y, tangent.x, normalize);
//        }

//        void translate(const Vec2& distance)
//{
//    for (int i = 0; i < N + 1; i++)
//    {
//        mControlVec2s[i].translate(distance);
//    }
//}

//void translate(float dx, float dy)
//{
//    for (int i = 0; i < N + 1; i++)
//    {
//        mControlVec2s[i].translate(dx, dy);
//    }
//}

//void rotate(float angle, Vec2 pivot = Vec2(0, 0))
//{
//    for (int i = 0; i < N + 1; i++)
//    {
//        mControlVec2s[i].rotate(angle, pivot);
//    }
//}

//// Note: This is a brute force length calculation. If more precision is needed,
//// use something like https://pomax.github.io/bezierinfo/#arclength
//float length(int intervals = 100) const
//        {
//            float length = 0.0f;

//            if (intervals > 0)
//            {
//                float t = 0.0f;
//const float dt = 1.0f / (float)intervals;

//Vec2 p1 = valueAt(t);
//Vec2 p2;

//                for (int i = 0; i<intervals; i++)
//                {
//                    p2 = valueAt(t + dt);
//float x = p2.x - p1.x;
//float y = p2.y - p1.y;
//length += sqrt(x* x + y* y);
//p1.set(p2);
//                    t += dt;
//                }
//            }

//            return length;
//        }

//        Split<N> split(float t) const
//        {
//            Vec2 l[N + 1];
//Vec2 r[N + 1];
//l[0] = mControlVec2s[0];
//            r[0] = mControlVec2s[N];

//            std::array<Vec2, N + 1> prev = mControlVec2s;
//std::array<Vec2, N + 1> curr;

//// de Casteljau: https://pomax.github.io/bezierinfo/#splitting
//int subs = 0;
//            while (subs<N)
//            {
//                for (int i = 0; i<N - subs; i++)
//                {
//                    curr[i].x = (1.0f - t) * prev[i].x + t* prev[i + 1].x;
//                    curr[i].y = (1.0f - t) * prev[i].y + t* prev[i + 1].y;
//                    if (i == 0)
//                        l[subs + 1].set(curr[i]);
//                    if (i == (N - subs - 1))
//                        r[subs + 1].set(curr[i]);
//                }
//                std::swap(prev, curr);
//                subs++;
//            }

//            return Split<N>(l, r);
//        }

//        Split<N> split() const
//        {
//            return split(0.5f);
//        }

//        float archMidVec2(const float epsilon = 0.001f, const int maxDepth = 100) const
//        {
//            float t = 0.5f;
//float s = 0.5f; // Binary search split value

//int iter = 0;
//            while (iter<maxDepth)
//            {
//                auto split = this->split(t);
//float low = split.left.length();
//float high = split.right.length();
//float diff = low - high;

//                if (std::abs(diff) <= epsilon)
//                {
//                    break;
//                }

//                s *= 0.5f;
//                t += (diff > 0 ? -1 : 1) * s;
//iter++;
//            }

//            return t;
//        }

//        ExtremeValues derivativeZero(int intervals = BEZIER_DEFAULT_INTERVALS,
//            float epsilon = BEZIER_FUZZY_EPSILON,
//            int maxIterations = BEZIER_DEFAULT_MAX_ITERATIONS) const
//        {
//            switch (N)
//            {
//            case 1:
//                return derivativeZero1();
//            case 2:
//                return derivativeZero2();
//            case 3:
//                //                    return derivativeZero3();
//                return newtonRhapson(intervals, epsilon, maxIterations);
//            default:
//                return newtonRhapson(intervals, epsilon, maxIterations);
//            }
//        }

//        ExtremeVec2s extremeVec2s() const
//        {
//            ExtremeValues xVals = derivativeZero();
//xVals.add(0.0f, 0);
//            xVals.add(1.0f, 0);

//            ExtremeVec2s xVec2s;
//            for (int i = 0; i<xVals.size(); i++)
//                xVec2s.add(valueAt(xVals[i].t));

//            return xVec2s;
//        }

//        AxisAlignedBoundingBox aabb() const
//        {
//            return AxisAlignedBoundingBox(extremeVec2s());
//        }

//        AxisAlignedBoundingBox aabb(const ExtremeVec2s& xVec2s) const
//        {
//            return AxisAlignedBoundingBox(xVec2s);
//        }

//        TightBoundingBox tbb() const
//        {
//            Bezier<N> bezier = *this;

//// Translate last control point (highest order) to origo.
//Vec2 translation(-bezier[N]);
//bezier.translate(translation);

//            // Rotate bezier to align the first control point (lowest order) with the x-axis
//            float angle = -bezier[0].angle();
//bezier.rotate(angle);

//            return TightBoundingBox(bezier.extremeVec2s(), translation, angle);
//        }

//    public:
//        Vec2& operator [] (int idx)
//{
//    assert(idx < size());
//    return mControlVec2s[idx];
//}

//    private:
//        ExtremeValues derivativeZero1() const
//        {
//            assert(N == 1);
//            return ExtremeValues();
//        }

//        ExtremeValues derivativeZero2() const
//        {
//            assert(N == 2);
//ExtremeValues xVals;
//Vec2 roots = (mControlVec2s[0] - mControlVec2s[1]) / (mControlVec2s[0] - mControlVec2s[1] * 2 + mControlVec2s[2]);
//            if (Math::isWithinZeroAndOne(roots[0]))
//                xVals.add(roots[0], 0);
//            if (Math::isWithinZeroAndOne(roots[1]))
//                xVals.add(roots[1], 1);
//            return xVals;
//        }

//        ExtremeValues derivativeZero3() const
//        {
//            // Note: NOT IMPLMENTED YET
//            assert(N == 3);
//            return ExtremeValues();
//        }

//        ExtremeValues newtonRhapson(int intervals = BEZIER_DEFAULT_INTERVALS,
//            float epsilon = BEZIER_FUZZY_EPSILON,
//            int maxIterations = BEZIER_DEFAULT_MAX_ITERATIONS) const
//        {
//            assert(N >= 2);
//ExtremeValues xVals;
//const float dt = 1.0f / (float)intervals;
//const float absEpsilon = fabs(epsilon);
//const Bezier<N - 1> db = derivative();
//const Bezier<N - 2> ddb = db.derivative();

//            for (int i = 0; i<Vec2::size; i++)
//            {
//                float t = 0;

//                while (t <= 1.0)
//                {
//                    float zeroVal = t;
//int current_iter = 0;

//                    while (current_iter<maxIterations)
//                    {
//                        float dbVal = db.valueAt(zeroVal, i);
//float ddbVal = ddb.valueAt(zeroVal, i);
//float nextZeroVal = zeroVal - (dbVal / ddbVal);

//                        if (fabs(nextZeroVal - zeroVal) < absEpsilon)
//                        {
//                            if (Math::isWithinZeroAndOne(nextZeroVal))
//                            {
//                                xVals.add(nextZeroVal, i);
//                                break;
//                            }
//                        }

//                        zeroVal = nextZeroVal;
//                        current_iter++;
//                    }

//                    t += dt;
//                }
//            }

//            return xVals;
//        }

//    public:

//private:
//    };

//    template<int N>
//    const BinomialCoefficients<N> Bezier<N>::binomialCoefficients = BinomialCoefficients<N>();

//    template<int N>
//    const PolynomialCoefficients<N> Bezier<N>::polynomialCoefficients = PolynomialCoefficients<N>();

} 
// namespace Bezier