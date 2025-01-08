using Blazor.RCL.NLog.LogService.Interface;
using System;
using System.Collections.Generic;
using System.Web;

namespace Blazor.RCL.NLog.LogService
{
    public class LogHelper : ILogHelper
    {
        private ILogService _logger;

        public LogHelper(ILogService logger)
        {
            _logger = logger;
        }

        public void SetDefaultAtributes<T>(T source) where T : class
        {
            var _defaultAttributeValue = new Dictionary<string, string>();

            foreach (var item in LogConstants.LogEventProperies.Split(','))
            {
                //HttpContext.Current.Request.Headers[item.Split('|')[0]] = source.GetType()?.GetProperty(item.Split('|')[1])?.GetValue(source)?.ToString() ?? string.Empty;
                _defaultAttributeValue[item.Split('|')[0]] = source.GetType()?.GetProperty(item.Split('|')[1])?.GetValue(source)?.ToString() ?? string.Empty;
            }
            _logger.DefaultAttributeValues = _defaultAttributeValue;
        }

        public void LogTrace(string message, LogConstants.TraceObjectType tracetype, object traceobject, params object[] args)
        {
            var items = new Dictionary<string, object>()
            {
                  {tracetype.ToString(), traceobject }
            };

            _logger.Trace(message, items, args);
        }

        public void LogDebug(string message, object debuginfo, params object[] args)
        {
            var items = new Dictionary<string, object>()
            {
                {"debuginfo",debuginfo}
            };

            _logger.Debug(message, items, args);
        }

        public void LogInfo(string message, string appstatus, object itemdata, params object[] args)
        {
            var items = new Dictionary<string, object>()
            {
                {"appstatus",appstatus},
                {"timestarted",DateTime.Now.ToLocalTime().ToString("u")},
                {"itemdata",itemdata}
            };

            _logger.Info(message, items, args);
        }

        public void LogWarn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        public void LogError(string message, Exception exception, params object[] args)
        {
            var items = new Dictionary<string, object>()
            {
                {"exception",exception},
            };

            _logger.Error(message, items, args);
        }

        public void LogErrorFatal(string message, Exception exception, params object[] args)
        {

            _logger.Fatal(message, exception, args);
        }

        public void LogMessage(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

    }
}
