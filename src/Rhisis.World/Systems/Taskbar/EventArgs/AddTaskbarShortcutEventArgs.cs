﻿using Rhisis.Core.Common;
using Rhisis.World.Game.Core.Systems;
using System;

namespace Rhisis.World.Systems.Taskbar.EventArgs
{
    public class AddTaskbarAppletEventArgs : SystemEventArgs
    {
        public int SlotIndex { get; }

        public ShortcutType Type { get; }

        public uint ObjId { get; }

        public ShortcutObjType ObjType { get; }

        public uint ObjIndex { get; }

        public uint UserId { get; }

        public uint ObjData { get; }

        public string Text { get; }

        public AddTaskbarAppletEventArgs(int slotIndex, ShortcutType type, uint objId, ShortcutObjType objType, uint objIndex, uint userId, uint objData, string text)
        {
            SlotIndex = slotIndex;
            Type = type;
            ObjId = objId;
            ObjType = objType;
            ObjIndex = objIndex;
            UserId = userId;
            ObjData = objData;
            Text = text;
        }

        public override bool CheckArguments()
        {
            return SlotIndex >= 0 && SlotIndex < TaskbarSystem.MaxTaskbarApplets;
        }
    }
}