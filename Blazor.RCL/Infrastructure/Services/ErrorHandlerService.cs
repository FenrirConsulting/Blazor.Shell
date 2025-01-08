using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.Services
{
    public class ErrorHandlerService
    {
        private readonly ILogHelper _logger;

        public event Action<Exception>? HandleError;

        public ErrorHandlerService(ILogHelper logger)
        {
            _logger = logger;
        }

        public void ProcessError(Exception exception)
        {
            _logger.LogError("An error occurred", exception);
            HandleError?.Invoke(exception);
            // You can add additional error handling logic here
            // For example, you could log to a database, send an email, etc.
        }
    }
}
