using System;
using System.Drawing;

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
        private string name;
        private Image image;

        /// <summary>
        /// Method to be called when command will be executed.
        /// </summary>
        /// <param name="arguments">arguments to the command</param>
        protected abstract void OnExecute(params object[] arguments);

        /// <summary>
        /// Commands can be disabled when they should not be used.
        /// </summary>
        public abstract bool Enabled { get; }

        /// <summary>
        /// HACK: Commands can checked if they represent a (boolean) state.
        /// </summary>
        public virtual bool Checked { set; get; }

        #region ICommand Members

        /// <summary>
        /// Execute the command. 
        /// </summary>
        /// <param name="arguments">Arguments past to the command.</param>
        public void Execute(params object[] arguments)
        {
            OnExecute(arguments);
        }

        /// <summary>
        /// Undo the command.
        /// </summary>
        public void Unexecute()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// DisplayName of the command.
        /// </summary>
        public virtual string Name
        {
            get { return name; }

            protected set
            {
                string oldName = name;
                name = value;
            }
        }

        #region ICommand Members

        /// <summary>
        /// Image of the command
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                Image oldImage = image;
                image = value;
            }
        }

        #endregion
    }
}