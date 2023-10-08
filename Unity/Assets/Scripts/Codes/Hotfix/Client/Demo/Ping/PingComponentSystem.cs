using System;

namespace ET.Client
{
    [ObjectSystem]
    public class PingComponentAwakeSystem: AwakeSystem<PingComponent>
    {
        protected override void Awake(PingComponent self) => PingAsync(self).Coroutine();

        private static async ETTask PingAsync(PingComponent self)
        {
            var session = self.GetParent<Session>();
            var instanceId = self.InstanceId;

            while (true)
            {
                if (self.InstanceId != instanceId)
                    return;

                var time1 = TimeHelper.ClientNow();
                try
                {
                    var response = await session.Call(new C2G_Ping()) as G2C_Ping;

                    if (self.InstanceId != instanceId)
                        return;

                    var time2 = TimeHelper.ClientNow();
                    self.Ping = time2 - time1;

                    TimeInfo.Instance.ServerMinusClientTime = response.Time + (time2 - time1) / 2 - time2;

                    await TimerComponent.Instance.WaitAsync(2000);
                }
                catch (RpcException e)
                {
                    // session断开导致ping rpc报错，记录一下即可，不需要打成error
                    Log.Info($"ping error: {self.Id} {e.Error}");
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"ping error: \n{e}");
                }
            }
        }
    }

    [ObjectSystem]
    public class PingComponentDestroySystem: DestroySystem<PingComponent>
    {
        protected override void Destroy(PingComponent self) => self.Ping = default;
    }
}