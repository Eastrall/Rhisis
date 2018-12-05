using Ether.Network.Server;
using Rhisis.World.Game.Entities;

namespace Rhisis.World
{
    public interface IWorldServer : INetServer
    {
        IPlayerEntity GetPlayerEntity(int id);
        IPlayerEntity GetPlayerEntity(string name);
    }
}
