using Microsoft.Extensions.Logging;
using Rhisis.Core.Common;
using Rhisis.Core.Common.Game.Structures;
using Rhisis.Core.DependencyInjection;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Systems.Taskbar.EventArgs;
using System;

namespace Rhisis.World.Systems.Taskbar
{
    [System(SystemType.Notifiable)]
    public class TaskbarSystem : ISystem
    {
        private static ILogger Logger => DependencyContainer.Instance.Resolve<ILogger<TaskbarSystem>>();

        public const int MaxTaskbarApplets = 18;
        public const int MaxTaskbarItems = 9;
        public const int MaxTaskbarItemLevels = 8;
        public const int MaxTaskbarQueue = 5;

        public WorldEntityType Type => WorldEntityType.Player;

        public void Execute(IWorldEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
                return;

            if (!args.GetCheckArguments())
            {
                Logger.LogWarning("Invalid arguments received.");
                return;
            }

            switch (args)
            {
                case AddTaskbarAppletEventArgs e:
                    HandleAddAppletTaskbarShortcut(player, e);
                    break;
                case RemoveTaskbarAppletEventArgs e:
                    HandleRemoveAppletTaskbarShortcut(player, e);
                    break;
                case AddTaskbarItemEventArgs e:
                    HandleAddItemTaskbarShortcut(player, e);
                    break;
                case RemoveTaskbarItemEventArgs e:
                    HandleRemoveItemTaskbarShortcut(player, e);
                    break;
                case TaskbarSkillEventArgs e:
                    HandleActionSlot(player, e);
                    break;
            }
        }

        private void HandleAddAppletTaskbarShortcut(IPlayerEntity player, AddTaskbarAppletEventArgs e)
        {
            player.Taskbar.Applets.CreateShortcut(new Shortcut(e.SlotIndex, e.Type, e.ObjId, e.ObjectType, e.ObjIndex, e.UserId, e.ObjData, e.Text));
            Logger.LogDebug("Created Shortcut of type {0} on slot {1} for player {2} on the Applet Taskbar", Enum.GetName(typeof(ShortcutType), e.Type), e.SlotIndex, player.Object.Name);
        }

        private void HandleRemoveAppletTaskbarShortcut(IPlayerEntity player, RemoveTaskbarAppletEventArgs e)
        {
            player.Taskbar.Applets.RemoveShortcut(e.SlotIndex);
            Logger.LogDebug("Removed Shortcut on slot {0} of player {1} on the Applet Taskbar", e.SlotIndex, player.Object.Name);
        }

        private void HandleAddItemTaskbarShortcut(IPlayerEntity player, AddTaskbarItemEventArgs e)
        {
            player.Taskbar.Items.CreateShortcut(new Shortcut(e.SlotIndex, e.Type, e.ObjId, e.ObjectType, e.ObjIndex, e.UserId, e.ObjData, e.Text), e.SlotLevelIndex);
            Logger.LogDebug("Created Shortcut of type {0} on slot {1} for player {2} on the Item Taskbar", Enum.GetName(typeof(ShortcutType), e.Type), e.SlotIndex, player.Object.Name);
        }

        private void HandleRemoveItemTaskbarShortcut(IPlayerEntity player, RemoveTaskbarItemEventArgs e)
        {
            player.Taskbar.Items.RemoveShortcut(e.SlotLevelIndex, e.SlotIndex);
            Logger.LogDebug("Removed Shortcut on slot {0}-{1} of player {2} on the Item Taskbar", e.SlotLevelIndex, e.SlotIndex, player.Object.Name);
        }

        private void HandleActionSlot(IPlayerEntity player, TaskbarSkillEventArgs e)
        {
            player.Taskbar.Queue.ClearQueue();
            player.Taskbar.Queue.CreateShortcuts(e.Skills);
            Logger.LogDebug("Handled Actionslot Shortcuts of player {0}", player.Object.Name);
        }
    }
}