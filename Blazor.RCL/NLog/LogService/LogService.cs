using Blazor.RCL.NLog.LogService.Interface;
using NLog;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Blazor.RCL.NLog.LogService
{
    public class LogService : ILogService
    {
        private ILogger _logger;

        private Dictionary<string, string>? _defaultAttributeValues;
        public Dictionary<string, string>? DefaultAttributeValues { set => _defaultAttributeValues = value; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(IHttpContextAccessor httpContextAccessor)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _httpContextAccessor = httpContextAccessor;
        }

        #region public methods

        /// <summary>
        /// This method writes the Debug details to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Debug(string message, params object[] args)
        {
            if (_logger.IsDebugEnabled)
            {
                WriteToLog(LogLevel.Debug, message, args);
            }
        }
        /// <summary>
        /// This method writes the Debug details with exception to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Debug(string message, Exception exception, params object[] args)
        {
            if (_logger.IsFatalEnabled)
            {
                WriteToLogWithEx(LogLevel.Debug, message, exception, args);
            }
        }

        /// <summary>
        /// This method writes the information to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Info(string message, params object[] args)
        {
            if (_logger.IsInfoEnabled)
            {
                WriteToLog(LogLevel.Info, message, args);
            }
        }
        /// <summary>
        /// This method writes the info with exception details to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Info(string message, Exception exception, params object[] args)
        {
            if (_logger.IsFatalEnabled)
            {
                WriteToLogWithEx(LogLevel.Info, message, exception, args);
            }
        }

        /// <summary>
        /// This method writes the warning information to log file
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args"></param>
        public void Warn(string message, params object[] args)
        {
            if (_logger.IsWarnEnabled)
            {
                WriteToLog(LogLevel.Warn, message, args);
            }
        }
        /// <summary>
        /// This method writes the warning information with exception details to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Warn(string message, Exception exception, params object[] args)
        {
            if (_logger.IsFatalEnabled)
            {
                WriteToLogWithEx(LogLevel.Warn, message, exception, args);
            }
        }

        /// <summary>
        /// This method writes the error information to log file
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="args"></param>
        public void Error(string message, params object[] args)
        {
            if (_logger.IsWarnEnabled)
            {
                WriteToLog(LogLevel.Error, message, args);
            }
        }
        /// <summary>
        /// This method writes the error information with exception details to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Error(string message, Exception exception, params object[] args)
        {
            if (_logger.IsErrorEnabled)
            {
                WriteToLogWithEx(LogLevel.Error, message, exception, args);
            }
        }

        /// <summary>
        /// This method writes the fatal exception information to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Fatal(string message, params object[] args)
        {
            if (_logger.IsFatalEnabled)
            {
                WriteToLog(LogLevel.Fatal, message, args);
            }
        }
        /// <summary>
        /// This method writes the fatal exception information to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Fatal(string message, Exception exception, params object[] args)
        {
            if (_logger.IsFatalEnabled)
            {
                WriteToLogWithEx(LogLevel.Fatal, message, exception, args);
            }
        }

        /// <summary>
        ///  This method writes the trace information to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Trace(string message, params object[] args)
        {
            if (_logger.IsTraceEnabled)
            {
                WriteToLog(LogLevel.Trace, message, args);
            }
        }
        /// <summary>
        /// This method writes the trace information with exception details to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="args"></param>
        public void Trace(string message, Exception exception, params object[] args)
        {
            if (_logger.IsFatalEnabled)
            {
                WriteToLogWithEx(LogLevel.Trace, message, exception, args);
            }
        }


        #endregion

        #region private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        private void WriteToLog(LogLevel level, string message, params object[] args)
        {
            LogEventInfo logEvent = new LogEventInfo(level, _logger.Name, null, message, args);

            SetupLogEventProperties(ref logEvent, true, args);

            _logger.Log(typeof(LogService), logEvent);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="args"></param>
        private void WriteToLogWithEx(LogLevel level, string message, Exception exception, params object[] args)
        {
            LogEventInfo le = new LogEventInfo(level, _logger.Name, null, message, args)
            {
                Exception = exception
            };

            SetupLogEventProperties(ref le, false, args);

            _logger.Log(typeof(LogService), le);
        }

        /// <summary>
        /// This methods setups the default attribures 
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="loadDefaults"></param>
        /// <param name="args"></param>
        private void SetupLogEventProperties(ref LogEventInfo logEvent, bool loadDefaults = false, params object[] args)
        {
            if (loadDefaults)
            {
                foreach (var item in LogConstants.LogEventProperies.Split(','))
                {
                    var loggerEvent = new Dictionary<string, object>();
                    var headerKey = item.Split('|')[0];

                    if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(headerKey))
                    {
                        loggerEvent[headerKey] = _httpContextAccessor.HttpContext.Request.Headers[headerKey];
                    }
                }
            }

            if (args != null)
            {
                int index = 0;

                for (int i = 0; i < args.Length; i++)
                {
                    var detail = args[i];

                    if (detail is Dictionary<string, object>)
                    {
                        foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)detail)
                        {
                            logEvent.Properties[kvp.Key] = kvp.Value;
                        }
                    }
                    else if (detail is object[])
                    {
                        foreach (object item in (object[])detail)
                        {
                            logEvent.Properties["additionaldetail_" + index] = item;
                            index++;
                        }
                    }
                    else
                    {
                        logEvent.Properties["additionaldetail_" + index] = detail;
                        index++;
                    }
                }
            }
        }

        #endregion
    }
}
