namespace ET.Server
{
    [ActorMessageHandler(SceneType.LoginCenter)]
    public class A2L_LoginAccountRequestHandler: AMActorRpcHandler<Scene, A2L_LoginAccountRequest, L2A_LoginAccountResponse>
    {
        protected override async ETTask Run(Scene scene, A2L_LoginAccountRequest request, L2A_LoginAccountResponse response)
        {
            var accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock, accountId.GetHashCode()))
            {
                //当前帐号未登录,可正常登录
                var loginInfoRecordComponent = scene.GetComponent<LoginInfoRecordComponent>();
                if (!loginInfoRecordComponent.IsExist(accountId))
                    return;

                //当前帐号已登录,需强制下线
                var zone = loginInfoRecordComponent.Get(accountId);
                var gateConfig = RealmGateAddressHelper.GetGate(zone);
                var g2L_DisconnectGateUnit = await MessageHelper.CallActor(gateConfig.InstanceId,
                    new L2G_DisconnectGateUnit() { AccountId = accountId }) as G2L_DisconnectGateUnit;

                response.Error = g2L_DisconnectGateUnit.Error;
            }
        }
    }
}