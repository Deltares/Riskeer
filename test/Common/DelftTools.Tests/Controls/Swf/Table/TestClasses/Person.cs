using System;

namespace DelftTools.Tests.Controls.Swf.Table
{
    public class Person
    {
        private string name;

    
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime DateOfDeath { get; set; }
    }
}