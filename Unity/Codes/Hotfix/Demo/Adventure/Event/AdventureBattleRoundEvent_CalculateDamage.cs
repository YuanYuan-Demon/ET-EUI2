﻿using ET.EventType;

namespace ET
{
    [FriendClassAttribute(typeof(ET.AdventureComponent))]
    public class AdventureBattleRoundEvent_CalculateDamage : AEventAsync<AdventureBattleRoundAsync>
    {
        protected override async ETTask Run(AdventureBattleRoundAsync args)
        {
            if (!args.AttackUnit.IsAlive() || !args.TargetUnit.IsAlive())
            {
                return;
            }
            SRandom random = args.ZoneScene.CurrentScene().GetComponent<AdventureComponent>().Random;
            int damage = DamageCalcuateHelper.CalcuateDamageValue(args.AttackUnit, args.TargetUnit, ref random);
            int hp = args.TargetUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp) - damage;
            if (hp <= 0)
            {
                hp = 0;
                args.TargetUnit.SetAlive(false);
            }

            args.TargetUnit.GetComponent<NumericComponent>().Set(NumericType.Hp, hp);
            Log.Debug($"*************  {args.AttackUnit}造成伤害:{damage}  *************");
            Log.Debug($"*************  {args.TargetUnit.Type}剩余血量:{hp}  *************");
            Game.EventSystem.PublishAsync(new EventType.ShowDamageValueViewAsync()
            {
                ZoneScene = args.ZoneScene,
                TargetUnit = args.TargetUnit,
                DamageValue = damage,
            }).Coroutine();

            await ETTask.CompletedTask; ;
        }
    }
}