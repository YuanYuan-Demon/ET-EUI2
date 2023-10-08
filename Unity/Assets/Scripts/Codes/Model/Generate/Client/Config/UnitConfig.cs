
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


public sealed partial class UnitConfig: Luban.BeanBase
{

    public UnitConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        UnitType = (ET.UnitType)_buf.ReadInt();
        Class = (RoleClass)_buf.ReadInt();
        Prefab = _buf.ReadString();
        Desc = _buf.ReadString();
        AIType = _buf.ReadString();
        Height = _buf.ReadLong();
        Radius = _buf.ReadLong();
        {int n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);Attributes = new System.Collections.Generic.Dictionary<NumericType, long>(n0 * 3 / 2);for(var i0 = 0 ; i0 < n0 ; i0++) { NumericType _k0;  _k0 = (NumericType)_buf.ReadInt(); long _v0;  _v0 = _buf.ReadLong();     Attributes.Add(_k0, _v0);}}
        PostInit();
    }

    public static UnitConfig DeserializeUnitConfig(ByteBuf _buf)
    {
        return new UnitConfig(_buf);
    }

    /// <summary>
    /// id
    /// </summary>
    public int Id { get; }
    public string Name { get; }
    /// <summary>
    /// 类型
    /// </summary>
    public ET.UnitType UnitType { get; }
    /// <summary>
    /// 职业
    /// </summary>
    public RoleClass Class { get; }
    /// <summary>
    /// 模型资源
    /// </summary>
    public string Prefab { get; }
    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; }
    /// <summary>
    /// AI策略
    /// </summary>
    public string AIType { get; }
    /// <summary>
    /// 身高(单位:0.1毫米)
    /// </summary>
    public long Height { get; }
    /// <summary>
    /// 体积半径
    /// </summary>
    public long Radius { get; }
    public System.Collections.Generic.Dictionary<NumericType, long> Attributes { get; }

    public const int __ID__ = -568528378;
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
        + "unitType:" + UnitType + ","
        + "class:" + Class + ","
        + "prefab:" + Prefab + ","
        + "desc:" + Desc + ","
        + "aIType:" + AIType + ","
        + "height:" + Height + ","
        + "radius:" + Radius + ","
        + "attributes:" + Luban.StringUtil.CollectionToString(Attributes) + ","
        + "}";
    }
}

}

