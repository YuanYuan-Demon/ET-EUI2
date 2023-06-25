﻿namespace ET.Client
{
    public static class ItemFactory
    {
        public static Item Create(Scene self, ItemInfo itemInfo)
        {
            Item item = self.AddChild<Item, int>(itemInfo.ItemConfigId);
            item.FromMessage(itemInfo);
            return item;
        }

        public static Item Create(this BagComponent self, ItemInfo itemInfo)
        {
            Item item = self.AddChild<Item, int>(itemInfo.ItemConfigId);
            item.FromMessage(itemInfo);
            return item;
        }
    }
}