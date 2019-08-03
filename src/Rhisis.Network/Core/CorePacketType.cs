namespace Rhisis.Network.Core
{
    /// <summary>
    /// Defines the core server/client packet types.
    /// </summary>
    public enum CorePacketType : int
    {
        Welcome,
        Authenticate,
        AuthenticationResult,
        Disconnect,
        UpdateClusterWorldsList
    }
}
