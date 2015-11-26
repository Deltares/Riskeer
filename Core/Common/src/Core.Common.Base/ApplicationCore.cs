using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Workflow;

namespace Core.Common.Base
{
    public class ApplicationCore : IDisposable
    {
        private readonly ActivityRunner activityRunner;
        private readonly List<ApplicationPlugin> plugins;

        public ApplicationCore()
        {
            plugins = new List<ApplicationPlugin>();
            activityRunner = new ActivityRunner();

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = ActivityRunner;
            }
        }

        public IEnumerable<ApplicationPlugin> Plugins
        {
            get
            {
                return plugins;
            }
        }

        public IActivityRunner ActivityRunner
        {
            get
            {
                return activityRunner;
            }
        }

        public void AddPlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Add(applicationPlugin);

            applicationPlugin.Activate();
        }

        public void RemovePlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Remove(applicationPlugin);

            applicationPlugin.Deactivate();
        }

        public void Dispose()
        {
            foreach (var plugin in Plugins.ToList())
            {
                RemovePlugin(plugin);
            }

            if (RunningActivityLogAppender.Instance != null)
            {
                RunningActivityLogAppender.Instance.ActivityRunner = null;
            }
        }
    }
}