using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
	public partial class AIConfigCategory
	{
		[ProtoIgnore]
		[BsonIgnore]
		public Dictionary<int, SortedDictionary<int, AIConfig>> AIConfigs = new Dictionary<int, SortedDictionary<int, AIConfig>>();

		public SortedDictionary<int, AIConfig> GetAI(int aiConfigId)
		{
			return this.AIConfigs[aiConfigId];
		}

		 partial  void PostInit()
		{
			foreach (var (id,aiConfig) in this.GetAll())
			{
				SortedDictionary<int, AIConfig> aiNodeConfig;
				if (!this.AIConfigs.TryGetValue(aiConfig.AIConfigId, out aiNodeConfig))
				{
					aiNodeConfig = new();
					this.AIConfigs.Add(aiConfig.AIConfigId, aiNodeConfig);
				}
				aiNodeConfig.Add(id, aiConfig);
			}
		}
	}
}