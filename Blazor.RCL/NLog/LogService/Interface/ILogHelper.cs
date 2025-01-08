using System;

namespace Blazor.RCL.NLog.LogService.Interface
{
    public interface ILogHelper
    {
        void SetDefaultAtributes<T>(T source) where T : class;

        void LogTrace(string message, LogConstants.TraceObjectType tracetype, object traceobject, params object[] args);

        void LogDebug(string message, object debuginfo, params object[] args);

        void LogInfo(string message, string appstatus, object itemdata, params object[] args);

        void LogWarn(string message, params object[] args);

        void LogError(string message, Exception exception, params object[] args);

        void LogErrorFatal(string message, Exception exception, params object[] args);

        void LogMessage(string message, params object[] args);
    }
}
