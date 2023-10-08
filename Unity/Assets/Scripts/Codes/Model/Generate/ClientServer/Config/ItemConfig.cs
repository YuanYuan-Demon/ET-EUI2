
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


public sealed partial class ItemConfig: Luban.BeanBase
{

    public ItemConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        Name = _buf.ReadString();
        Desc = _buf.ReadString();
        Type = (ItemType)_buf.ReadInt();
        LimitClass = (RoleClass)_buf.ReadInt();
        Level = _buf.ReadInt();
        Icon = _buf.ReadString();
        StackLimit = _buf.ReadInt();
        Price = _buf.ReadInt();
        SellPrice = _buf.ReadInt();
        PostInit();
    }

    public static ItemConfig DeserializeItemConfig(ByteBuf _buf)
    {
        return new ItemConfig(_buf);
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
    /// 描述
    /// </summary>
    public string Desc { get; }
    /// <summary>
    /// 物品类型
    /// </summary>
    public ItemType Type { get; }
    public RoleClass LimitClass { get; }
    /// <summary>
    /// 道具等级
    /// </summary>
    public int Level { get; }
    /// <summary>
    /// c图标
    /// </summary>
    public string Icon { get; }
    /// <summary>
    /// 堆叠限制
    /// </summary>
    public int StackLimit { get; }
    /// <summary>
    /// 购买价格
    /// </summary>
    public int Price { get; }
    /// <summary>
    /// 销售价格
    /// </summary>
    public int SellPrice { get; }

    public const int __ID__ = -764023723;
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
        + "desc:" + Desc + ","
        + "type:" + Type + ","
        + "limitClass:" + LimitClass + ","
        + "level:" + Level + ","
        + "icon:" + Icon + ","
        + "stackLimit:" + StackLimit + ","
        + "price:" + Price + ","
        + "sellPrice:" + SellPrice + ","
        + "}";
    }
}

}

