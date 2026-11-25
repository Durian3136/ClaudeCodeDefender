using System.Net.NetworkInformation;

namespace ClaudeCodeDefender.Services;

public sealed class VpnMonitor : IVpnMonitor
{
    private readonly string _interfaceId;

    public VpnMonitor(string interfaceId)
    {
        if (string.IsNullOrWhiteSpace(interfaceId))
            throw new ArgumentException("Interface id must be provided.", nameof(interfaceId));
        _interfaceId = interfaceId;
    }

    public bool IsVpnAlive() =>
        NetworkInterface.GetAllNetworkInterfaces()
            .Any(ni => ni.Id == _interfaceId &&
                       ni.OperationalStatus == OperationalStatus.Up);
}