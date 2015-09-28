namespace DelftTools.Controls.Swf.Test.Table.TestClasses
{
    public class SomeData
    {
        private double a;
        private int b;

        public SomeData() : this(0.0, 0)
        {
        }

        public SomeData(double a, int b)
        {
            this.a = a;
            this.b = b;
        }

        public double A
        {
            get { return a; }
            set { a = value; }
        }

        public int B
        {
            get { return b; }
            set { b = value; }
        }
    }
}