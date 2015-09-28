
namespace DelftTools.Utils.Data
{
    //todo describe purpose of NameableEntity, it is not used.
    public abstract class NameableEntity: Unique<long>, INameable
    {
        private string name;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
