using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A city data object.
    /// </summary>
    public class City : IData
    {
        internal string name;
        internal double x1, y1, x2, y2;
        internal Area[] areas;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            i.Get(nameof(areas), ref areas);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            o.Put(nameof(areas), areas);
        }

        public Area[] Areas => areas;

        public bool Contains(double x, double y)
        {
            return true;
        }

        public Area LocateArea(double x, double y)
        {
            return default(Area);
        }

        public override string ToString()
        {
            return name;
        }
    }

    public struct Area : IData
    {
        internal string name;
        internal double x1, y1, x2, y2;
        internal string @char;

        public void Read(IDataInput i, int proj = 255)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            i.Get(nameof(@char), ref @char);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 255) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            o.Put(nameof(@char), @char);
        }

        public override string ToString()
        {
            return name;
        }
    }
}