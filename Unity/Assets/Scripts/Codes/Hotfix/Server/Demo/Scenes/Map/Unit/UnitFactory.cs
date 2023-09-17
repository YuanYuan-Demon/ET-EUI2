﻿using System;
using Unity.Mathematics;

namespace ET.Server
{
    public static class UnitFactory
    {
        public static Unit Create(Scene scene, long id, UnitType unitType)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            switch (unitType)
            {
                case UnitType.Player:
                {
                    Unit unit = unitComponent.AddChildWithId<Unit, int>(id, 1001);
                    NumericComponent numericComponent = unit.AddComponent<NumericComponent>();
                    numericComponent.Set(NumericType.Speed, 6f); // 速度是6米每秒
                    numericComponent.Set(NumericType.AOI, 15000); // 视野15米
                    foreach (var config in PlayerNumericConfigCategory.Instance.GetAll().Values)
                    {
                        if (config.BaseValue == 0)
                            continue;
                        if (config.Id < 3000)   //小于3000的值都用加成属性推导
                        {
                            int baseKey = config.Id * 10 + 1;
                            numericComponent.SetNoEvent(baseKey, config.BaseValue);
                        }
                        else
                        {
                            //大于3000的值 直接使用
                            numericComponent.SetNoEvent(config.Id, config.BaseValue);
                        }
                    }
                    unit.Position = new float3(50, 8.2f, 40);
                    unit.AddComponent<BagComponent>();

                    #region 背包测试

                    //添加装备
                    for (int i = 0; i < 10; i++)
                    {
                        int equipId = RandomHelper.RandomInt32(1, 8) + 1000 * RandomHelper.RandomInt32(1, 4) + 10 * RandomHelper.RandomInt32(0, 2);
                        if (!BagHelper.AddItemByConfigId(unit, equipId, isSync: false))
                        {
                            Log.Error("增加背包物品失败");
                        }
                    }

                    //添加道具
                    for (int i = 0; i < 30; i++)
                    {
                        int itemId = RandomHelper.RandomInt32(1, 11);
                        if (!BagHelper.AddItemByConfigId(unit, itemId, isSync: false))
                        {
                            Log.Error("增加背包物品失败");
                        }
                    }

                    #endregion 背包测试

                    unit.AddComponent<EquipmentsComponent>();
                    //Undone: AddComponent<ForgeComponent>()
                    //unit.AddComponent<ForgeComponent>();
                    //Undone: AddComponent<TasksComponent>()
                    //unit.AddComponent<TasksComponent>();

                    unitComponent.Add(unit);
                    return unit;
                }
                default:
                    throw new Exception($"not such unit type: {unitType}");
            }
        }

        public static Unit CreateMonster(Scene scene, int configId)
        {
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.Instance.GenerateId(), configId);
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>();

            numericComponent.SetNoEvent(NumericType.MaxHp, unit.Config.MaxHP);
            numericComponent.SetNoEvent(NumericType.Hp, unit.Config.MaxHP);
            numericComponent.SetNoEvent(NumericType.AD, unit.Config.DamageValue);
            numericComponent.SetNoEvent(NumericType.IsAlive, 1);

            unitComponent.Add(unit);
            return unit;
        }
    }
}