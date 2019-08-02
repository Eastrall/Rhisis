using Rhisis.Core.Common;
using Rhisis.Core.Exceptions;
using Rhisis.World.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhisis.World.Game.Core.Systems
{
    public class SystemManager : Singleton<SystemManager>
    {
        private readonly IDictionary<Type, ISystem> _notifiableSystems;

        private Action<IWorldEntity, SystemEventArgs> _updatableActions;

        /// <summary>
        /// Creates a new <see cref="SystemManager"/> instance.
        /// </summary>
        public SystemManager()
        {
            this._notifiableSystems = new Dictionary<Type, ISystem>();
        }

        /// <summary>
        /// Initialize and loads the game systems using reflection.
        /// </summary>
        public void Initialize()
        {
            IEnumerable<Type> systemTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetTypeInfo().GetCustomAttribute<SystemAttribute>() != null && typeof(ISystem).IsAssignableFrom(x));

            foreach (var systemType in systemTypes)
            {
                var attribute = systemType.GetTypeInfo().GetCustomAttribute<SystemAttribute>();

                if (attribute != null)
                {
                    var system = Activator.CreateInstance(systemType) as ISystem;

                    switch (attribute.Type)
                    {
                        case SystemType.Notifiable:
                            this._notifiableSystems.Add(systemType, system);
                            break;
                        case SystemType.Updatable:
                            this._updatableActions += (IWorldEntity entity, SystemEventArgs args) =>
                            {
                                if ((entity.Type & system.Type) == entity.Type)
                                    system.Execute(entity, args);
                            };
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Executes a notifiable system.
        /// </summary>
        /// <typeparam name="TSystem">System type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="args">System arguments</param>
        public void Execute<TSystem>(IWorldEntity entity, SystemEventArgs args) where TSystem : ISystem
        {
            if (this._notifiableSystems.TryGetValue(typeof(TSystem), out ISystem system))
                system.Execute(entity, args);
            else
                throw new RhisisException($"Cannot find notifiable system with type: {typeof(TSystem).FullName}.");
        }

        /// <summary>
        /// Executes all updatable systems.
        /// </summary>
        /// <param name="entity"></param>
        public void ExecuteUpdatable(IWorldEntity entity) => this._updatableActions?.Invoke(entity, null);
    }
}
