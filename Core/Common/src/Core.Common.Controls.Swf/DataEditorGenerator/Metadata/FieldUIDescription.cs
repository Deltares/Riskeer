using System;

namespace Core.Common.Controls.Swf.DataEditorGenerator.Metadata
{
    public class FieldUIDescription
    {
        private readonly Func<object, object> getValueFunc;
        private readonly Action<object, object> setValueFunc;
        private readonly Func<object, bool> isEnabledFunc;
        private readonly Func<object, bool> isVisibleFunc;

        public ICustomControlHelper CustomControlHelper;
        public string Name;
        public string Category;
        public string SubCategory;
        public string Label;

        public string ToolTip;
        public string DocUrl;

        public bool IsReadOnly;
        public Type ValueType;
        public bool AlwaysRefresh;

        //more.. eg Unit, etc
        public string UnitSymbol;

        public FieldUIDescription(Func<object, object> getValueFunc, Action<object, object> setValueFunc,
                                  Func<object, bool> isEnabledFunc = null, Func<object, bool> isVisibleFunc = null, Func<object, object, string> validationMethod = null)
        {
            this.getValueFunc = getValueFunc;
            this.setValueFunc = setValueFunc;
            this.isEnabledFunc = isEnabledFunc;
            this.isVisibleFunc = isVisibleFunc;
            ValidationMethod = validationMethod;
        }

        public Func<object, object, string> ValidationMethod { get; set; }

        public bool HasDependencyFunctions
        {
            get
            {
                return isEnabledFunc != null || isVisibleFunc != null;
            }
        }

        public object GetValue(object data)
        {
            if (getValueFunc == null)
            {
                throw new InvalidOperationException();
            }
            return getValueFunc(data);
        }

        public void SetValue(object data, object value)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("readonly");
            }

            if (setValueFunc == null)
            {
                throw new InvalidOperationException("setter not set");
            }

            setValueFunc(data, value);
        }

        public bool IsEnabled(object data)
        {
            if (IsReadOnly)
            {
                return false;
            }
            if (isEnabledFunc == null)
            {
                return true;
            }
            return isEnabledFunc(data);
        }

        public bool IsVisible(object data)
        {
            if (IsReadOnly)
            {
                return false;
            }
            if (isVisibleFunc == null)
            {
                return true;
            }
            return isVisibleFunc(data);
        }

        public bool Validate(object data, object value, out string message)
        {
            message = "";
            if (ValidationMethod == null)
            {
                return true;
            }

            message = ValidationMethod(data, value);
            return string.IsNullOrEmpty(message);
        }
    }
}