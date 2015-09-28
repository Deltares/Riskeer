using log4net;
using log4net.Config;
using log4net.Core;

namespace DelftTools.TestUtils
{
    public static class LogHelper
    {
        /// <summary>
        /// Sets logging level for all current loggers to the level provided in arguments.
        /// Note: use it only when you need more control on logging, e.g. in unit tests. Otherwise use configuration files.
        /// </summary>
        /// <param name="level"></param>
        public static void SetLoggingLevel(Level level)
        {
            var repositories = LogManager.GetAllRepositories();

            //Configure all loggers to be at the debug level.
            foreach (var repository in repositories)
            {
                repository.Threshold = repository.LevelMap[level.ToString()];
                var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)repository;
                var loggers = hierarchy.GetCurrentLoggers();
                foreach (var logger in loggers)
                {
                    ((log4net.Repository.Hierarchy.Logger)logger).Level = hierarchy.LevelMap[level.ToString()];
                }
            }

            //Configure the root logger.
            var h = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            var rootLogger = h.Root;
            rootLogger.Level = h.LevelMap[level.ToString()];
        }

        /// <summary>
        /// Configures logging. In case of log4net reads log4net section from app.config file.
        /// </summary>
        public static void ConfigureLogging(Level level = null)
        {
            ResetLogging();
            BasicConfigurator.Configure();
            SetLoggingLevel(level ?? Level.Error);
        }

        /// <summary>
        /// Resets logging configuration, no log messages are sent after that.
        /// </summary>
        public static void ResetLogging()
        {
            LogManager.ResetConfiguration();
            SetLoggingLevel(Level.Error);
        }
    }
}