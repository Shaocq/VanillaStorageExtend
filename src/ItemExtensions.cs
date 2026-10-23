using System.Reflection;
using ItemStatsSystem;
using UnityEngine;

public static class ItemExtensions
{
	public static bool SetPrivateField(this Item item, string fieldName, object value)
	{
		FieldInfo field = typeof(Item).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(item, value);
			return true;
		}
		Debug.LogWarning((object)("[ItemExtensions] 未找到字段 '" + fieldName + "'，可能结构已变"));
		return false;
	}

	public static object GetPrivateField(this Item item, string fieldName)
	{
		FieldInfo field = typeof(Item).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			return field.GetValue(item);
		}
		Debug.LogWarning((object)("[ItemExtensions] 未找到字段 '" + fieldName + "'"));
		return null;
	}
}
