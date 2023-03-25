﻿using System.Collections.Generic;

namespace ET.Client
{
    public static class EUIModelViewHelper
    {
        public static void AddUIScrollItems<Dlg, Item>(this Dlg self, ref List<Item> list, int count) where Dlg : Entity, IUILogic where Item : Entity, IAwake, IUIScrollItem
        {
            list ??= new();

            if (count <= 0)
            {
                return;
            }

            foreach (var item in list)
            {
                item.Dispose();
            }
            list.Clear();
            for (int i = 0; i <= count; i++)
            {
                Item itemServer = self.AddChild<Item>(true);
                list.Add(itemServer);
            }
        }

        public static void AddUIScrollItems<Dlg, Item>(this Dlg self, ref Dictionary<int, Item> dict, int count) where Dlg : Entity, IUILogic where Item : Entity, IAwake, IUIScrollItem
        {
            dict ??= new();

            if (count <= 0)
            {
                return;
            }

            foreach (var item in dict)
            {
                item.Value.Dispose();
            }
            dict.Clear();
            for (int i = 0; i <= count; i++)
            {
                Item itemServer = self.AddChild<Item>(true);
                dict.Add(i, itemServer);
            }
        }
    }
}