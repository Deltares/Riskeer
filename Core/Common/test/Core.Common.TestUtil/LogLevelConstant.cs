using System;
using log4net.Core;

namespace Core.Common.TestUtil
{
    public enum LogLevelConstant
    {
        Off,
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    public static class LogLevelConstantExtensions
    {
        public static Level ToLog4NetLevel(this LogLevelConstant level)
        {
            switch (level)
            {
                case LogLevelConstant.Off:
                    return Level.Off;
                case LogLevelConstant.Fatal:
                    return Level.Fatal;
                case LogLevelConstant.Error:
                    return Level.Error;
                case LogLevelConstant.Warn:
                    return Level.Warn;
                case LogLevelConstant.Info:
                    return Level.Info;
                case LogLevelConstant.Debug:
                    return Level.Debug;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}