using System;
using System.Collections.Generic;

namespace Blazor.RCL.NLog.LogService.Interface
{
    public interface ILogService
    {
        Dictionary<string, string> DefaultAttributeValues { set; }

        void Debug(string message, params object[] args);
        void Debug(string message, Exception exception, params object[] args);

        void Info(string message, params object[] args);
        void Info(string message, Exception exeption, params object[] args);

        void Warn(string message, params object[] args);
        void Warn(string message, Exception exeption, params object[] args);

        void Error(string message, params object[] args);
        void Error(string message, Exception exception, params object[] args);

        void Fatal(string message, params object[] args);
        void Fatal(string message, Exception exception, params object[] args);

        void Trace(string message, params object[] args);
        void Trace(string message, Exception exception, params object[] args);

    }
}
