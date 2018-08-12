namespace Rhisis.Core.ISC.Packets
{
    public enum InterServerCode : uint
    {
        AUTH_SUCCESS = 0,
        AUTH_FAILED_UNKNOW_SERVER,
        AUTH_FAILED_CLUSTER_EXISTS,
        AUTH_FAILED_NO_CLUSTER,
        AUTH_FAILED_WORLD_EXISTS
    }
}
