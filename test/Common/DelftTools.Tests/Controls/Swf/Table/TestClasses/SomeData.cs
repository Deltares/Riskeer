namespace DelftTools.Controls.Swf.Test.Table.TestClasses
{
    public class SomeData
    {
        public SomeData() : this(0.0, 0) {}

        public SomeData(double a, int b)
        {
            this.A = a;
            this.B = b;
        }

        public double A { get; set; }

        public int B { get; set; }
    }
}