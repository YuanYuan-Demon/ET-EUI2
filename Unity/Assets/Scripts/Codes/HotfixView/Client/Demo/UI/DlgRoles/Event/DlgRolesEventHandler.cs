﻿namespace ET.Client
{
    [FriendOf(typeof (WindowCoreData))]
    [FriendOf(typeof (UIBaseWindow))]
    [AUIEvent(WindowID.WindowID_Roles)]
    public class DlgRolesEventHandler: IAUIEventHandler
    {
        public void OnInitWindowCoreData(UIBaseWindow uiBaseWindow) => uiBaseWindow.WindowData.windowType = UIWindowType.Normal;

        public void OnInitComponent(UIBaseWindow uiBaseWindow) => uiBaseWindow.AddComponent<DlgRoles>().AddComponent<DlgRolesViewComponent>();

        public void OnRegisterUIEvent(UIBaseWindow uiBaseWindow) => uiBaseWindow.GetComponent<DlgRoles>().RegisterUIEvent();

        public void OnShowWindow(UIBaseWindow uiBaseWindow, ShowWindowData windowData = null) =>
                uiBaseWindow.GetComponent<DlgRoles>().ShowWindow(windowData);

        public void OnHideWindow(UIBaseWindow uiBaseWindow)
        {
        }

        public void BeforeUnload(UIBaseWindow uiBaseWindow)
        {
        }
    }
}