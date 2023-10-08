
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


public sealed partial class EntryRandomConfig: Luban.BeanBase
{

    public EntryRandomConfig(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        EntryRandMinCount = _buf.ReadInt();
        EntryRandMaxCount = _buf.ReadInt();
        SpecialEntryRandMinCount = _buf.ReadInt();
        SpecialEntryRandMaxCount = _buf.ReadInt();
        EntryLevel = _buf.ReadInt();
        SpecialEntryLevel = _buf.ReadInt();
        PostInit();
    }

    public static EntryRandomConfig DeserializeEntryRandomConfig(ByteBuf _buf)
    {
        return new EntryRandomConfig(_buf);
    }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; }
    /// <summary>
    /// 随机词条最小数量
    /// </summary>
    public int EntryRandMinCount { get; }
    /// <summary>
    /// 随机词条最大数量
    /// </summary>
    public int EntryRandMaxCount { get; }
    /// <summary>
    /// 随机特殊词条最小数量
    /// </summary>
    public int SpecialEntryRandMinCount { get; }
    /// <summary>
    /// 随机特殊词条最大数量
    /// </summary>
    public int SpecialEntryRandMaxCount { get; }
    /// <summary>
    /// 随机词条等级
    /// </summary>
    public int EntryLevel { get; }
    /// <summary>
    /// 随机特殊词条等级
    /// </summary>
    public int SpecialEntryLevel { get; }

    public const int __ID__ = 2142959447;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<Type, IConfigSingleton> _tables)
    {
        PostResolve();
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "entryRandMinCount:" + EntryRandMinCount + ","
        + "entryRandMaxCount:" + EntryRandMaxCount + ","
        + "specialEntryRandMinCount:" + SpecialEntryRandMinCount + ","
        + "specialEntryRandMaxCount:" + SpecialEntryRandMaxCount + ","
        + "entryLevel:" + EntryLevel + ","
        + "specialEntryLevel:" + SpecialEntryLevel + ","
        + "}";
    }
}

}

