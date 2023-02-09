﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    [FriendClassAttribute(typeof(ET.AdventureComponent))]
    public static class AdventureComponentSystem
    {
        public static void ResetAdventure(this AdventureComponent self)
        {
            for (int i = 0; i < self.EnemyIdList.Count; i++)
            {
                self.ZoneScene().CurrentScene().GetComponent<UnitComponent>().Remove(self.EnemyIdList[i]);
            }
            TimerComponent.Instance?.Remove(ref self.BattleTimer);
            self.BattleTimer = 0;
            self.Round = 0;
            self.EnemyIdList.Clear();
            self.AliveEnemyIdList.Clear();

            Unit unit = UnitHelper.GetMyUnitFromZoneScene(self.ZoneScene());
            int maxHp = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.MaxHp);
            unit.GetComponent<NumericComponent>().Set(NumericType.Hp, maxHp);
            unit.GetComponent<NumericComponent>().Set(NumericType.IsAlive, 1);

            Game.EventSystem.PublishAsync(new EventType.AdventureRoundReset() { ZoneScene = self.ZoneScene() }).Coroutine();
        }

        public static async ETTask StartAdventure(this AdventureComponent self)
        {
            self.ResetAdventure();
            await self.CreateAdventureEnemy();
            self.ShowAdventureHpBarInfo(true);
            self.BattleTimer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 500, TimerType.BattleRound, self);
        }

        public static async ETTask CreateAdventureEnemy(this AdventureComponent self)
        {
            //获取玩家所在关卡配置
            Unit unit = UnitHelper.GetMyUnitFromZoneScene(self.ZoneScene());
            int levelId = unit.GetComponent<NumericComponent>().GetAsInt(NumericType.AdventureStatus);
            var battleLevelData = BattleLevelConfigCategory.Instance.Get(levelId);

            //根据关卡配置创建怪物
            for (int i = 0; i < battleLevelData.MonsterIds.Length; i++)
            {
                Unit monsterUnit = await UnitFactory.CreateMonsterAsync(self.ZoneScene().CurrentScene(), battleLevelData.MonsterIds[i]);
                monsterUnit.Position = new UnityEngine.Vector3(1.5f, -2 + i, 0);
                self.EnemyIdList.Add(monsterUnit.Id);
            }
        }

        public static async ETTask PlayOneBattleRound(this AdventureComponent self)
        {
            UnitComponent unitComponent = self.ZoneScene().CurrentScene().GetComponent<UnitComponent>();
            Unit unit = UnitHelper.GetMyUnitFromZoneScene(self.ZoneScene());
            Unit monsterUnit;

            if (self.Round % 2 == 0)
            {
                //玩家回合
                monsterUnit = self.GetTargetMonsterUnit();
                Game.EventSystem.PublishAsync(new EventType.AdventureBattleRoundViewAsync()
                {
                    ZoneScene = self.ZoneScene(),
                    AttackUnit = unit,
                    TargetUnit = monsterUnit
                }).Coroutine();
                await Game.EventSystem.PublishAsync(new EventType.AdventureBattleRoundAsync()
                {
                    ZoneScene = self.ZoneScene(),
                    AttackUnit = unit,
                    TargetUnit = monsterUnit
                });
                await TimerComponent.Instance.WaitAsync(1000);
            }
            else
            {
                //敌人回合
                for (int i = 0; i < self.EnemyIdList.Count && unit.IsAlive(); i++)
                {
                    monsterUnit = unitComponent.Get(self.EnemyIdList[i]);
                    if (!monsterUnit.IsAlive())
                    {
                        continue;
                    }
                    Game.EventSystem.PublishAsync(new EventType.AdventureBattleRoundViewAsync()
                    {
                        ZoneScene = self.ZoneScene(),
                        AttackUnit = unit,
                        TargetUnit = monsterUnit
                    }).Coroutine();
                    await Game.EventSystem.PublishAsync(new EventType.AdventureBattleRoundAsync()
                    {
                        ZoneScene = self.ZoneScene(),
                        AttackUnit = unit,
                        TargetUnit = monsterUnit
                    });
                    await TimerComponent.Instance.WaitAsync(1000);
                }
            }

            self.BattleRoundOver();
        }

        public static void BattleRoundOver(this AdventureComponent self)
        {
            ++self.Round;
            //计算战斗回合结果
            BattleRoundResult battleRoundResult = self.GetBattleRoundResult();
            Log.Debug($"当前回合结果:{battleRoundResult}");
            switch (battleRoundResult)
            {
                case BattleRoundResult.Keep:
                    self.BattleTimer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 500, TimerType.BattleRound, self);
                    break;

                case BattleRoundResult.Win:
                    Unit unit = UnitHelper.GetMyUnitFromZoneScene(self.ZoneScene());
                    Game.EventSystem.PublishAsync(new EventType.AdventureBattleOverAsync()
                    {
                        ZoneScene = self.ZoneScene(),
                        WinUnit = unit,
                    }).Coroutine();
                    break;

                case BattleRoundResult.Lose:
                    UnitComponent unitComponent = self.ZoneScene().CurrentScene().GetComponent<UnitComponent>();
                    for (int i = 0; i < self.EnemyIdList.Count; i++)
                    {
                        Unit monsterUnit = unitComponent.Get(self.EnemyIdList[i]);
                        if (!monsterUnit.IsAlive())
                        {
                            continue;
                        }
                        Game.EventSystem.PublishAsync(new EventType.AdventureBattleOverAsync()
                        {
                            ZoneScene = self.ZoneScene(),
                            WinUnit = monsterUnit,
                        }).Coroutine();
                    }
                    break;
            }

            //发送战报事件
            Game.EventSystem.PublishAsync(new EventType.AdventureBattleReportAsync()
            {
                Round = self.Round,
                BattleRoundResult = battleRoundResult,
                ZoneScene = self.ZoneScene()
            }).Coroutine();
        }

        /// <summary>
        /// 获取回合结果
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static BattleRoundResult GetBattleRoundResult(this AdventureComponent self)
        {
            Unit unit = UnitHelper.GetMyUnitFromZoneScene(self.ZoneScene());
            if (!unit.IsAlive())
            {
                return BattleRoundResult.Lose;
            }
            Unit monsterUnit = self.GetTargetMonsterUnit();
            if (monsterUnit == null)
            {
                return BattleRoundResult.Win;
            }
            return BattleRoundResult.Keep;
        }

        public static Unit GetTargetMonsterUnit(this AdventureComponent self)
        {
            self.AliveEnemyIdList.Clear();
            UnitComponent unitComponent = self.ZoneScene().CurrentScene().GetComponent<UnitComponent>();
            for (int i = 0; i < self.EnemyIdList.Count; i++)
            {
                Unit monsterUnit = unitComponent.Get(self.EnemyIdList[i]);
                if (monsterUnit.IsAlive())
                {
                    self.AliveEnemyIdList.Add(monsterUnit.Id);
                }
            }
            return self.AliveEnemyIdList.Count > 0
                ? unitComponent.Get(self.AliveEnemyIdList[0])
                : null;
        }

        public static void ShowAdventureHpBarInfo(this AdventureComponent self, bool show)
        {
        }
    }

    [Timer(TimerType.BattleRound)]
    public class AdventureBattleRoundTimer : ATimer<AdventureComponent>
    {
        public override void Run(AdventureComponent t)
        {
            t?.PlayOneBattleRound().Coroutine();
        }
    }
}