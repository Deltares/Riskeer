using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using DelftTools.Utils.Editing;
using PostSharp.Aspects;

namespace DelftTools.Utils.Aop
{
    /// <summary>
    /// Used to mark methods which can modify object state. Changes may include multiple property and/or collection changes.
    /// This attribute allows to skips these methods in some performance and consistency critical situations (e.g. save, load, undo/redo).
    /// </summary>
    [Serializable]
    [Synchronization]
    public class EditActionAttribute : MethodInterceptionAspect
    {
        public EditActionAttribute()
        {
        }

        private readonly Type editActionType;

        public EditActionAttribute(Type editActionType)
        {
            if (!(typeof(IEditAction).IsAssignableFrom(editActionType)))
            {
                throw new ArgumentException(string.Format("Type {0} is not of type IEditAction",
                                                          editActionType));
            }

            this.editActionType = editActionType;
        }
        
        /// <summary>
        /// Fired before any CollectionChanging or PropertyChanging events occur.
        /// </summary>
        public static Action<object, bool> BeforeEventCall;

        /// <summary>
        /// Fired after any CollectionChanged or PropertyChanged events occur. When collection Changing is cancelled - this event is still fired.
        /// </summary>
        public static Action<object, bool, bool> AfterEventCall;
        
        public static event Action<MethodInterceptionArgs> BeforeEdit;

        public static event Action<MethodInterceptionArgs> AfterEdit;
        
        [ThreadStatic]
        private static int editActionsInProgress;

        internal static string Indent
        {
            get { return GetIndent(editActionsInProgress); }
        }

        private static string GetIndent(int num)
        {
            return Enumerable.Repeat("  ", Math.Max(0, num)).Aggregate("", (s1, s2) => s1 + s2);
        }

        public static void FireBeforeEventCall(object sender, bool isPropertyChange)
        {
            if (BeforeEventCall != null)
            {
                BeforeEventCall(sender, isPropertyChange);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="isPropertyChange"></param>
        /// <param name="isCancelled">For example, when exception occurs during set/add/...</param>
        public static void FireAfterEventCall(object sender, bool isPropertyChange, bool isCancelled)
        {
            if (AfterEventCall != null)
            {
                AfterEventCall(sender, isPropertyChange, isCancelled);
            }
        }

        public sealed override void OnInvoke(MethodInterceptionArgs eventArgs)
        {
            if (EditActionSettings.Disabled && !(EditActionSettings.AllowRestoreActions && editActionType != null))
            {
                return;
            }

            IEditAction editAction = null;
            try
            {
                editActionsInProgress++;

                if (EventSettings.EnableLogging)
                {
                    //log.DebugFormat(Indent + ">> Entering edit action {0} (enabled:{1}) {2}",
                     //               (eventArgs.Instance != null ? eventArgs.Instance.GetType().Name : "static") + "." + eventArgs.Method.Name, !Disabled, editActionsInProgress);
                }

                if (BeforeEdit != null)
                {
                    BeforeEdit(eventArgs);
                }

                var editableObject = eventArgs.Instance as IEditableObject;
                if (EditActionSettings.SupportEditableObject && editActionType != null)
                {
                    if (editableObject == null)
                    {
                        throw new InvalidOperationException("Cannot apply EditAction attribute with EditActionType if target is not IEditableObject");
                    }

                    editAction = EditActionBase.Create(editActionType);

                    if (editAction.HandlesRestore)
                    {
                        editAction.Instance = eventArgs.Instance;
                        editAction.Arguments = eventArgs.Arguments.ToArray();
                        editAction.BeforeChanges();
                    }

                    editableObject.BeginEdit(editAction);
                }

                var exception = false;
                try
                {
                    eventArgs.Proceed();
                }
                catch(Exception)
                {
                    exception = true;
                    
                    throw;
                }
                finally
                {
                    if (EditActionSettings.SupportEditableObject && editAction != null)
                    {
                        if (exception)
                        {
                            editableObject.CancelEdit();
                        }
                        else
                        {
                            if (editAction.HandlesRestore)
                            {
                                editAction.ReturnValue = eventArgs.ReturnValue;
                            }
                            editableObject.EndEdit();
                        }
                    }

                    if (AfterEdit != null)
                    {
                        AfterEdit(eventArgs);
                    }

                    if (EventSettings.EnableLogging)
                    {
                        //log.DebugFormat(Indent + "<< Exiting edit action {0} {1}",
                        //                (eventArgs.Instance != null ? eventArgs.Instance.GetType().Name : "static") + "." + eventArgs.Method.Name, editActionsInProgress);
                    }
                }
            }
            finally
            {
                editActionsInProgress--;
            }
        }
    }

    /// <summary>
    /// These settings are in a seperate class so that we don't need a reference to PostSharp at callers
    /// </summary>
    public static class EditActionSettings
    {
        public static bool Disabled;
        public static bool AllowRestoreActions;
        public static bool SupportEditableObject;
    }

}
