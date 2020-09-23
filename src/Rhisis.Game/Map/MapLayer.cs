﻿using Microsoft.Extensions.DependencyInjection;
using Rhisis.Game.Abstractions.Entities;
using Rhisis.Game.Abstractions.Map;
using Rhisis.Game.Abstractions.Systems;
using Rhisis.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Game.Map
{
    public class MapLayer : IMapLayer
    {
        private const int VisibilityRange = 75; // TODO: make a configuration for this
        private readonly List<IPlayer> _players;
        private readonly List<IMonster> _monsters;
        private readonly List<INpc> _npcs;
        private readonly List<IMapItem> _items;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMobilitySystem _mobilitySystem;
        private readonly IVisibilitySystem _visibilitySystem;

        public int Id { get; }

        public IMap ParentMap { get; }

        public IEnumerable<IMapItem> Items => _items;

        public IEnumerable<IMonster> Monsters => _monsters;

        public IEnumerable<INpc> Npcs => _npcs;

        public IEnumerable<IPlayer> Players => _players;

        public MapLayer(IMap map, int id, IServiceProvider serviceProvider)
        {
            ParentMap = map;
            Id = id;
            _serviceProvider = serviceProvider;
            _players = new List<IPlayer>();
            _monsters = new List<IMonster>();
            _npcs = new List<INpc>();
            _items = new List<IMapItem>();
            _mobilitySystem = _serviceProvider.GetService<IMobilitySystem>();
            _visibilitySystem = _serviceProvider.GetService<IVisibilitySystem>();
        }

        public void AddItem(IMapItem mapItem)
        {
            if (mapItem == null)
            {
                return;
            }

            if (!_items.Contains(mapItem))
            {
                lock (_items)
                {
                    if (!_items.Contains(mapItem))
                    {
                        _items.Add(mapItem);
                    }
                }
            }
        }

        public void AddMonster(IMonster monster)
        {
            if (monster == null)
            {
                return;
            }

            if (!_monsters.Contains(monster))
            {
                lock (_monsters)
                {
                    if (!_monsters.Contains(monster))
                    {
                        _monsters.Add(monster);
                    }
                }
            }
        }

        public void AddNpc(INpc npc)
        {
            if (npc == null)
            {
                return;
            }

            if (!_npcs.Contains(npc))
            {
                lock (_npcs)
                {
                    if (!_npcs.Contains(npc))
                    {
                        _npcs.Add(npc);
                    }
                }
            }
        }

        public void AddPlayer(IPlayer player)
        {
            if (player == null)
            {
                return;
            }

            if (!_players.Contains(player))
            {
                lock (_players)
                {
                    if (!_players.Contains(player))
                    {
                        _players.Add(player);
                    }
                }
            }
        }

        public void RemoveItem(IMapItem mapItem)
        {
            throw new NotImplementedException();
        }

        public void RemoveMonster(IMonster monster)
        {
            throw new NotImplementedException();
        }

        public void RemoveNpc(INpc npc)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(IPlayer player)
        {
            if (_players.Contains(player))
            {
                lock (_players)
                {
                    if (_players.Contains(player))
                    {
                        _players.Remove(player);
                    }
                }
            }
        }

        public void Process()
        {
            if (!_players.Any())
            {
                return;
            }

            lock (_players)
            {
                if (_players.Any())
                {
                    foreach (IPlayer player in _players)
                    {
                        player.Behavior.Update();
                        _mobilitySystem.Execute(player);
                        _visibilitySystem.Execute(player);
                    }
                }
            }

            if (_monsters.Any())
            {
                lock (_monsters)
                {
                    if (_monsters.Any())
                    {
                        foreach (IMonster monster in _monsters)
                        {
                            monster.Behavior.Update();
                            _mobilitySystem.Execute(monster);
                        }
                    }
                }
            }

            if (_npcs.Any())
            {
                lock (_npcs)
                {
                    if (_npcs.Any())
                    {
                        foreach (INpc npc in _npcs)
                        {
                            if (npc.VisibleObjects.Any())
                            {
                                npc.Behavior.Update();
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<IWorldObject> GetVisibleObjects(IWorldObject worldObject)
        {
            var objects = new List<IWorldObject>();

            lock (_players)
            {
                objects.AddRange(GetVisibleObjects(worldObject, _players));
            }

            lock (_monsters)
            {
                objects.AddRange(GetVisibleObjects(worldObject, _monsters));
            }

            lock (_npcs)
            {
                objects.AddRange(GetVisibleObjects(worldObject, _npcs));
            }

            lock (_items)
            {
                objects.AddRange(GetVisibleObjects(worldObject, _items));
            }

            return objects;
        }

        private IEnumerable<TObjects> GetVisibleObjects<TObjects>(IWorldObject worldObject, IEnumerable<TObjects> objects)
            where TObjects : IWorldObject
        {
            return objects.Where(x => x.Id != worldObject.Id && x.Spawned && x.Position.IsInRange(worldObject.Position, VisibilityRange));
        }
    }
}
