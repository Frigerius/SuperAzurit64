
using System.Collections;

public class MapToAreaPointer
{

    private PlatformArea area;
    private SquareType type;
    private int rangeToNextAbove;

    public MapToAreaPointer()
        : this(null, SquareType.NOTSET, -1)
    {

    }

    public MapToAreaPointer(SquareType t)
        : this(null, t, -1)
    {
    }

    public MapToAreaPointer(PlatformArea n, SquareType t, int r)
    {
        area = n;
        type = t;
        rangeToNextAbove = r;
    }

    public void SetArea(PlatformArea a)
    {
        this.Area = a;
    }

    public PlatformArea Area
    {
        get { return area; }
        set { area = value; }
    }

    public SquareType Type
    {
        get { return type; }
        set { type = value; }
    }

    public int RangeToNextAbove
    {
        get { return rangeToNextAbove; }
        set { rangeToNextAbove = value; }
    }
}
