﻿using System;
using System.Collections.Generic;

namespace ET
{
    public class C2A_CreateRoleHandler : AMRpcHandler<C2A_CreateRole, A2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2A_CreateRole request, A2C_CreateRole response, Action reply)
        {
            #region 校验

            //服务器类型校验
            if (!session.CheckSceneType(SceneType.Account))
                return;
            //重复请求校验
            if (session.GetComponent<SessionLoginComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                session.Disconnect();
                return;
            }

            //令牌校验
            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId.GetHashCode()); ;

            if (token is null || token != request.Token)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                session.Disconnect();
                return;
            }

            //角色名校验
            if (string.IsNullOrEmpty(request.Name))
            {
                response.Error = ErrorCode.ERR_RoleNameIsNull;
                reply();
                session.Disconnect();
                return;
            }

            #endregion 校验

            //创建角色
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId))
            {
                //角色名查重
                List<RoleInfo> roleInfos = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone())
                    .Query<RoleInfo>(r => r.ServerId == request.ServerId && r.Name == request.Name);
                if (roleInfos?.Count > 0)
                {
                    response.Error = ErrorCode.ERR_RoleNameSame;
                    reply();
                    return;
                }
                //保存数据库
                RoleInfo roleInfo = session.AddChildWithId<RoleInfo>(IdGenerater.Instance.GenerateUnitId(request.ServerId));
                roleInfo.Name = request.Name;
                roleInfo.ServerId = request.ServerId;
                roleInfo.Status = (int)RoleInfoStatus.Normal;
                roleInfo.AccountId = request.AccountId;
                roleInfo.CreateTime = TimeHelper.ServerNow();
                roleInfo.LastLoginTIme = 0;
                await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save(roleInfo);
                //发送响应
                response.NRoleInfo = roleInfo.ToNServerInfo();
                reply();
                roleInfo?.Dispose();
            }
        }
    }
}