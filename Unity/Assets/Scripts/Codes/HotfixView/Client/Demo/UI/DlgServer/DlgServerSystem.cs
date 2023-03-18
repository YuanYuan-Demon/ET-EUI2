﻿using System;
using UnityEngine;

namespace ET.Client
{
    [FriendOf(typeof(DlgServer))]
    public static class DlgServerSystem
    {
        public static void RegisterUIEvent(this DlgServer self)
        {
            self.View.EB_ConfirmButton.AddListenerAsync(self.OnClickConfirm);
            self.View.EL_ServerListLoopVerticalScrollRect.AddItemRefreshListener((Transform transform, int index) => { self.OnScrollItemRefreshHandler(transform, index); });
        }

        public static void ShowWindow(this DlgServer self, Entity contextData = null)
        {
            int count = self.ClientScene().GetComponent<ServerInfosComponent>().ServerInfos.Count;
            self.AddUIScrollItems(ref self.ScrollItemServerInfos, count);
            self.View.EL_ServerListLoopVerticalScrollRect.SetVisible(true, count);
        }

        public static void HideWindow(this DlgServer self)
        {
            self.RemoveUIScrollItems(ref self.ScrollItemServerInfos);
        }

        public static void OnScrollItemRefreshHandler(this DlgServer self, Transform transform, int index)
        {
            var itemServerInfo = self.ScrollItemServerInfos[index].BindTrans(transform);
            ServerInfosComponent serverInfosComponent = self.ClientScene().GetComponent<ServerInfosComponent>();
            var info = serverInfosComponent.ServerInfos[index];
            itemServerInfo.EB_ServerImage.color = info.Id == serverInfosComponent.CurServerId ? Color.red : Color.cyan;
            itemServerInfo.ET_ServerNameText.SetText(info.ServerName);
            itemServerInfo.EB_ServerButton.AddListener(() => self.OnSelectServer(info.Id));
        }

        public static void OnSelectServer(this DlgServer self, long serverId)
        {
            self.ClientScene().GetComponent<ServerInfosComponent>().CurServerId = (int)serverId;
            Log.Debug($"当前选择的服务器 Id 是:{serverId}");
            self.View.EL_ServerListLoopVerticalScrollRect.RefillCells();
        }

        public static async ETTask OnClickConfirm(this DlgServer self)
        {
            bool isSelect = self.ClientScene().GetComponent<ServerInfosComponent>().CurServerId != 0;
            if (!isSelect)
            {
                Log.Error("请先选择区服");
                return;
            }

            try
            {
                int errorCode = await LoginHelper.GetRoles(self.ClientScene());
                if (errorCode != ErrorCode.ERR_Success)
                {
                    Log.Error(errorCode.ToString());
                    return;
                }

                self.ClientScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Roles);
                self.ClientScene().GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Server);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}