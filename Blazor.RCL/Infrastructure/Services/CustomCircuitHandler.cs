using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Blazor.RCL.Infrastructure.Services
{
    public class CustomCircuitHandler : CircuitHandler
    {
        private readonly ILogHelper _logger;

        public CustomCircuitHandler(ILogHelper logger)
        {
            _logger = logger;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogMessage($"Circuit opened. Id: {circuit.Id}");
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogMessage($"Circuit closed. Id: {circuit.Id}");
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogWarn($"Circuit connection down. Id: {circuit.Id}");
            return base.OnConnectionDownAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogMessage($"Circuit connection up. Id: {circuit.Id}");
            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }
    }
}
