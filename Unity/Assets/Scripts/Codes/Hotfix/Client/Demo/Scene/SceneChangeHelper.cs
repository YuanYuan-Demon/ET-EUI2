﻿using ET.EventType;

namespace ET.Client
{
    public static class SceneChangeHelper
    {
        // 场景切换协程
        public static async ETTask SceneChangeTo(Scene clientScene, string sceneName, long sceneInstanceId)
        {
            clientScene.RemoveComponent<AIComponent>();

            var currentScenesComponent = clientScene.GetComponent<CurrentScenesComponent>();
            currentScenesComponent.Scene?.Dispose(); // 删除之前的CurrentScene，创建新的
            var currentScene = SceneFactory.CreateCurrentScene(sceneInstanceId, clientScene.Zone, sceneName, currentScenesComponent);
            var unitComponent = currentScene.AddComponent<UnitComponent>();

            // 可以订阅这个事件中创建Loading界面
            EventSystem.Instance.Publish(clientScene, new SceneChangeStart());

            // 等待CreateMyUnit的消息
            var waitCreateMyUnit = await clientScene.GetComponent<ObjectWait>().Wait<Wait_CreateMyUnit>();
            var m2CCreateMyUnit = waitCreateMyUnit.Message;
            clientScene.GetComponent<PlayerComponent>().MyId = m2CCreateMyUnit.Unit.UnitId;

            var unit = UnitFactory.Create(currentScene, m2CCreateMyUnit.Unit);
            unitComponent.MyUnit = unit;

            clientScene.RemoveComponent<AIComponent>();

            EventSystem.Instance.Publish(currentScene, new SceneChangeFinish());

            // 通知等待场景切换的协程
            clientScene.GetComponent<ObjectWait>().Notify(new Wait_SceneChangeFinish());
        }
    }
}