
using System.Collections.Generic;
namespace SharpMap.UI.Snapping
{
    class SnapStrategies : List<ISnapStrategy>, ISnapStrategies
    {
        #region ISnapStrategies Members

        public ISnapStrategy this[string name]
        {
            get
            {
                foreach (ISnapStrategy snapStrategy in this)
                {
                    if (snapStrategy.Layer == name)
                        return snapStrategy;
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Count; i++)
                {
                    ISnapStrategy snapStrategy = this[i];
                    if (snapStrategy.Layer == name)
                    {
                        snapStrategy = value;
                        return;
                    }
                }
                Add(value);
            }
        }

        #endregion
    }
}
