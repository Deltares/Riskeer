namespace DelftTools.Controls
{
    /// <summary>
    /// Linking command to button. <seealso href="http://jasonkemp.ca/archive/2006/05/16/UsingtheCommandPatterninWindowsFormsclients.aspx">Using the Command Pattern in Windows Forms clients.</seealso>
    /// </summary>
    /// <example>
    /// This example demonstrates how to create a Button, hook it up to a Command with a minimum of code and let the power of data binding deal with turning the command on and off:
    /// <code>
    /// Button button = new Button();
    /// Command command = new ArbitraryCommand("My Data");
    /// button.DataBindings.Add("Text", command, "DisplayName");
    /// button.DataBindings.Add("Enabled", command, "Enabled");
    /// button.Click += delegate { command.Execute();  };
    /// </code>
    /// </example>
    public abstract class Command : ICommand
    {
        public abstract bool Enabled { get; }

        public virtual bool Checked { set; get; }

        public void Execute(params object[] arguments)
        {
            OnExecute(arguments);
        }

        protected abstract void OnExecute(params object[] arguments);
    }
}