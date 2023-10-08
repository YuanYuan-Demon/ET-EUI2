using System;

namespace ET.Server
{
    [FriendOf(typeof (SessionStatusComponent))]
    [MessageHandler(SceneType.Gate)]
    [FriendOf(typeof (RoleInfo))]
    public class C2G_EnterGameHandler: AMRpcHandler<C2G_EnterGame, G2C_EnterGame>
    {
        private static async ETTask<long> EnterWorldChatServer(Unit unit)
        {
            var startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(unit.DomainZone(), "Chat");
            var response = await MessageHelper.CallActor(startSceneConfig.InstanceId,
                new G2Chat_EnterChat()
                {
                    UnitId = unit.Id,
                    Name = unit.GetComponent<RoleInfo>().Name,
                    GateSessionActorId = unit.GetComponent<UnitGateComponent>().GateSessionActorId,
                }) as Chat2G_EnterChat;

            return response.ChatInfoUnitInstanceId;
        }

        private static async ETTask<long> EnterFriendServer(Unit unit)
        {
            var startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(unit.DomainZone(), "Friend");
            var response = await MessageHelper.CallActor(startSceneConfig.InstanceId,
                new G2F_EnterFriend()
                {
                    UnitId = unit.Id,
                    Name = unit.GetComponent<RoleInfo>().Name,
                    GateSessionActorId = unit.GetComponent<UnitGateComponent>().GateSessionActorId,
                }) as F2G_EnterFriend;

            return response.FriendUnitInstanceId;
        }

        protected override async ETTask Run(Session session, C2G_EnterGame request, G2C_EnterGame response)
        {
            var scene = session.DomainScene();

#region 校验

            //避免重复请求
            if (session.GetComponent<SessionLoginComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                return;
            }

            //检测是否存在Session到Player的映射
            var sessionPlayerComponent = session.GetComponent<SessionPlayerComponent>();
            if (sessionPlayerComponent is null)
            {
                response.Error = ErrorCode.ERR_SessionPlayerError;
                return;
            }

            //校验Player是否在线
            var player = Root.Instance.Get(sessionPlayerComponent.PlayerInstanceId) as Player;
            if (player is null || player.IsDisposed)
            {
                response.Error = ErrorCode.ERR_NonePlayerError;
                return;
            }

#endregion 校验

            var instanceId = session.InstanceId;
            using (session.AddComponent<SessionLoginComponent>())
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, player.AccountId))
            {
                if (instanceId != session.InstanceId || player.IsDisposed)
                {
                    response.Error = ErrorCode.ERR_SessionPlayerError;
                    return;
                }

                var sessionStatusComponent = session.GetComponent<SessionStatusComponent>();
                if (sessionStatusComponent?.Status == SessionStatus.Game)
                {
                    response.Error = ErrorCode.ERR_SessionStatusError;
                    return;
                }

                //当前帐号已有角色在线
                if (player.PlayerState == PlayerState.Game)
                {
                    try
                    {
                        var reqEnter = await MessageHelper.CallLocationActor(player.UnitId, new G2M_RequestEnterGameStatus());
                        if (reqEnter.Error == ErrorCode.ERR_Success)
                            return;

                        Log.Error($"二次登陆失败: {reqEnter.Error} | {reqEnter.Message}");
                        response.Error = reqEnter.Error;
                        await DisconnectHelper.KickPlayer(player, true);
                        session.Disconnect();
                    }
                    catch (Exception e)
                    {
                        Log.Error($"二次登陆失败: {e}");
                        response.Error = ErrorCode.ERR_ReEnterGameError2;
                        await DisconnectHelper.KickPlayer(player, true);
                        session.Disconnect();
                    }

                    return;
                }

                //正常登录角色
                try
                {
                    //添加网关到游戏逻辑服的映射
                    //player.AddComponent<GateMapComponent>();
                    //gateMapComponent.Scene = await SceneFactory.Create(gateMapComponent, "GateMap", SceneType.Map);

                    //从数据库或缓存中加载出Unit实体及其相关组件
                    var (isNewPlayer, unit) = await UnitCacheHelper.LoadUnit(player);
                    unit.AddComponent<UnitGateComponent, long>(session.InstanceId);
                    // unit.AddComponent<UnitGateComponent, long>(player.InstanceId);

                    player.ChatUnitInstanceId = await EnterWorldChatServer(unit);
                    player.FriendUnitInstanceId = await EnterFriendServer(unit);

                    //玩家Unit上线后的初始化操作
                    await UnitHelper.InitUnit(unit, isNewPlayer);
                    response.UnitId = unit.Id;
                    //session.Send(response);
                    //reply();
                    //创建游戏对象(unit.Id = player.Id = roleId)
                    //Unit unit = UnitFactory.Create(gateMapComponent.Scene, player.Id, UnitType.Player);
                    //unit.AddComponent<UnitGateComponent, long>(session.InstanceId);

                    //将游戏对象传送至游戏逻辑服
                    var unitId = unit.Id;
                    var startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.DomainZone(), "MainCity");
                    await TransferHelper.Transfer(unit, startSceneConfig.InstanceId, startSceneConfig.Name);

                    player.UnitId = unitId;
                    //response.UnitId = unitId;
                    //reply();

                    //添加当前会话状态组件-更改当前会话状态
                    sessionStatusComponent = session.GetComponent<SessionStatusComponent>();
                    sessionStatusComponent ??= session.AddComponent<SessionStatusComponent>();
                    sessionStatusComponent.Status = SessionStatus.Game;
                    player.PlayerState = PlayerState.Game;
                }
                catch (Exception e)
                {
                    Log.Error($"角色进入游戏逻辑服出错: 帐号ID:{player.AccountId} 角色ID:{player.Id} 异常: {e}");
                    response.Error = ErrorCode.ERR_EnterGameError;
                    await DisconnectHelper.KickPlayer(player, true);
                    session.Disconnect();
                }
            }
        }
    }
}