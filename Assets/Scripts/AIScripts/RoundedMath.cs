
using System.Collections;

public static class RoundedMath
{

    static public float Add(float a, float b)
    {
        return (float)System.Math.Round(a + b, 1);
    }

    static public float Sub(float a, float b)
    {
        return (float)System.Math.Round(a - b, 1);
    }
    static public float Mult(float a, float b)
    {
        return (float)System.Math.Round(a * b, 1);
    }
    static public float Div(float a, float b)
    {
        return (float)System.Math.Round(a/b,1);
    }
}
