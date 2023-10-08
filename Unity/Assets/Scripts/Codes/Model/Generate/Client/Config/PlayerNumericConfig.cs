
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ET.Luban;



namespace ET
{


public sealed partial class PlayerNumericConfig: Luban.BeanBase
{

    public PlayerNumericConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        IsNeedShow = _buf.ReadInt();
        IsPersent = _buf.ReadInt();
        PostInit();
    }

    public static PlayerNumericConfig DeserializePlayerNumericConfig(ByteBuf _buf)
    {
        return new PlayerNumericConfig(_buf);
    }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 是否用于展示
    /// </summary>
    public int IsNeedShow { get; }
    /// <summary>
    /// 是否是百分比
    /// </summary>
    public int IsPersent { get; }

    public const int __ID__ = -278520018;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<Type, IConfigSingleton> _tables)
    {
        PostResolve();
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "isNeedShow:" + IsNeedShow + ","
        + "isPersent:" + IsPersent + ","
        + "}";
    }
}

}

