using System;
using Core.Common.Base.Properties;
using log4net;

namespace Core.Common.Base.Workflow
{
    public class ActivityRunner
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ActivityRunner));

        public static void RunActivity(IActivity activity)
        {
            try
            {
                activity.Initialize();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Initialization_of_0_has_failed, activity.Name));
                }

                while (activity.Status != ActivityStatus.Done)
                {
                    if (activity.Status == ActivityStatus.Cancelled)
                    {
                        log.WarnFormat(Resources.ActivityRunner_RunActivity_Execution_of_0_has_been_canceled, activity.Name);
                        break;
                    }

                    if (activity.Status != ActivityStatus.WaitingForData)
                    {
                        activity.Execute();
                    }

                    if (activity.Status == ActivityStatus.Failed)
                    {
                        throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Execution_of_0_has_failed, activity.Name));
                    }
                }

                if (activity.Status != ActivityStatus.Cancelled)
                {
                    activity.Finish();

                    if (activity.Status == ActivityStatus.Failed)
                    {
                        throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Finishing_of_0_has_failed, activity.Name));
                    }
                }

                activity.Cleanup();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Clean_up_of_0_has_failed, activity.Name));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message); //for build server debugging
                log.Error(exception.Message);
            }
            finally
            {
                try
                {
                    if (activity.Status != ActivityStatus.Cleaned)
                    {
                        activity.Cleanup();
                    }
                }
                catch (Exception)
                {
                    log.ErrorFormat(Resources.ActivityRunner_RunActivity_Clean_up_of_0_has_failed, activity.Name);
                }
            }
        }
    }
}