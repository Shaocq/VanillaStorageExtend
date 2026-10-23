using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Duckov.Economy;
using ItemStatsSystem;
using UnityEngine;
using VanillaStorageExtend;

namespace vanillaStorageExtend;

public static class KamakotoDuckovTools
{
	public enum DecomMode
	{
		None,
		Free,
		Auto
	}

	public class ModifierDescriptionWithOffset
	{
		public ModifierDescription modifierDescription;

		public float modifierOffset;
	}

	public static List<string> addedFormulaIds = new List<string>();

	public static List<int> addedFormulaResults = new List<int>();

	public static List<int> addedDecomposeItemIds = new List<int>();

	public static int Metal01 = 91043;

	public static int Metal02 = 91044;

	public static int Metal03 = 91045;

	public static int Level1Plastic01 = 91046;

	public static int Level1Plastic02 = 91047;

	public static int Level1Plastic03 = 91048;

	public static (int id, long amount)[] FastInputFormula(params long[] amounts)
	{
		int[] array = new int[6] { Metal01, Metal02, Metal03, Level1Plastic01, Level1Plastic02, Level1Plastic03 };
		List<(int, long)> list = new List<(int, long)>();
		for (int i = 0; i < amounts.Length && i < array.Length; i++)
		{
			if (amounts[i] > 0)
			{
				list.Add((array[i], amounts[i]));
			}
		}
		return list.ToArray();
	}

	public static void AddCraftingFormula(string formulaId, long money, (int id, long amount)[] costItems, int resultItemId, int resultItemAmount, string[] tags = null, string requirePerk = "", bool unlockByDefault = true, bool hideInIndex = false, bool lockInDemo = false)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		CraftingFormulaCollection instance = CraftingFormulaCollection.Instance;
		FieldInfo field = typeof(CraftingFormulaCollection).GetField("list", BindingFlags.Instance | BindingFlags.NonPublic);
		List<CraftingFormula> list = (List<CraftingFormula>)field.GetValue(instance);
		foreach (CraftingFormula item2 in list)
		{
			if (item2.id == formulaId)
			{
				Debug.LogWarning((object)("[VAE] 配方ID已存在，跳过添加: " + formulaId));
				return;
			}
		}
		CraftingFormula item = new CraftingFormula
		{
			id = formulaId,
			unlockByDefault = unlockByDefault
		};
		Cost cost = new Cost
		{
			money = money
		};
		Duckov.Economy.Cost.ItemEntry[] array = new Duckov.Economy.Cost.ItemEntry[costItems.Length];
		for (int i = 0; i < costItems.Length; i++)
		{
			array[i] = new Duckov.Economy.Cost.ItemEntry
			{
				id = costItems[i].id,
				amount = costItems[i].amount
			};
		}
		cost.items = array;
		item.cost = cost;
		CraftingFormula.ItemEntry result = new CraftingFormula.ItemEntry
		{
			id = resultItemId,
			amount = resultItemAmount
		};
		item.result = result;
		item.requirePerk = requirePerk;
		item.tags = tags ?? new string[1] { "WorkBenchAdvanced" };
		item.hideInIndex = hideInIndex;
		item.lockInDemo = lockInDemo;
		list.Add(item);
		if (!addedFormulaIds.Contains(formulaId))
		{
			addedFormulaIds.Add(formulaId);
			addedFormulaResults.Add(resultItemId);
		}
		Debug.Log((object)("[VAE] 成功添加配方: " + formulaId));
		FieldInfo field2 = typeof(CraftingFormulaCollection).GetField("_entries_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
		field2.SetValue(instance, null);
	}

	public static void RemoveAllAddedFormulas()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			CraftingFormulaCollection instance = CraftingFormulaCollection.Instance;
			FieldInfo field = typeof(CraftingFormulaCollection).GetField("list", BindingFlags.Instance | BindingFlags.NonPublic);
			List<CraftingFormula> list = (List<CraftingFormula>)field.GetValue(instance);
			int num = 0;
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				if (addedFormulaIds.Contains(list[num2].id))
				{
					Debug.Log((object)("[VAE] 移除配方: " + list[num2].id));
					list.RemoveAt(num2);
					num++;
				}
			}
			addedFormulaIds.Clear();
			addedFormulaResults.Clear();
			FieldInfo field2 = typeof(CraftingFormulaCollection).GetField("_entries_ReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
			field2.SetValue(instance, null);
			Debug.Log((object)$"[VAE] 成功移除 {num} 个配方");
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"[VAE] 移除配方时出错: {arg}");
		}
	}

	public static (int id, long amount)[] AutoCalcDecomResult(ModBehaviour.ItemConfig config)
	{
		int value = config.Value;
		int quality = config.Quality;
		List<(int, long)> list = new List<(int, long)>();
		long num = 1L;
		if (value > 10000)
		{
			long num2 = value - 10000;
			long num3 = (num2 + 39999) / 40000;
			num = 1 + num3;
		}
		if (quality == 3)
		{
			list.Add((Metal01, 1L));
			list.Add((Level1Plastic01, 1L));
		}
		else if (quality == 4)
		{
			list.Add((Metal01, num * 2));
			list.Add((Metal02, num));
			list.Add((Level1Plastic01, num * 2));
			list.Add((Level1Plastic02, num));
		}
		else if (quality >= 5)
		{
			list.Add((Metal01, num * 3));
			list.Add((Metal02, num * 2));
			list.Add((Metal03, num));
			list.Add((Level1Plastic01, num * 3));
			list.Add((Level1Plastic02, num * 2));
			list.Add((Level1Plastic03, num));
		}
		return list.ToArray();
	}

	public static void AddDecomposeFormula(int itemId, long money, (int id, long amount)[] resultItems)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		DecomposeDatabase instance = DecomposeDatabase.Instance;
		FieldInfo field = typeof(DecomposeDatabase).GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);
		DecomposeFormula[] collection = (DecomposeFormula[])field.GetValue(instance);
		List<DecomposeFormula> list = new List<DecomposeFormula>(collection);
		foreach (DecomposeFormula item2 in list)
		{
			if (item2.item == itemId)
			{
				Debug.LogWarning((object)$"[VAE] 分解配方已存在，跳过添加: {itemId}");
				return;
			}
		}
		DecomposeFormula item = new DecomposeFormula
		{
			item = itemId,
			valid = true
		};
		Cost result = new Cost
		{
			money = money
		};
		Duckov.Economy.Cost.ItemEntry[] array = new Duckov.Economy.Cost.ItemEntry[resultItems.Length];
		for (int i = 0; i < resultItems.Length; i++)
		{
			array[i] = new Duckov.Economy.Cost.ItemEntry
			{
				id = resultItems[i].id,
				amount = resultItems[i].amount
			};
		}
		result.items = array;
		item.result = result;
		list.Add(item);
		field.SetValue(instance, list.ToArray());
		if (!addedDecomposeItemIds.Contains(itemId))
		{
			addedDecomposeItemIds.Add(itemId);
		}
		Debug.Log((object)$"[VAE] 成功添加分解配方: {itemId}");
		typeof(DecomposeDatabase).GetMethod("RebuildDictionary", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(instance, null);
	}

	public static void RemoveAllAddedDecomposeFormulas()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			DecomposeDatabase instance = DecomposeDatabase.Instance;
			FieldInfo field = typeof(DecomposeDatabase).GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);
			DecomposeFormula[] collection = (DecomposeFormula[])field.GetValue(instance);
			List<DecomposeFormula> list = new List<DecomposeFormula>(collection);
			int num = 0;
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				if (addedDecomposeItemIds.Contains(list[num2].item))
				{
					Debug.Log((object)$"[VAE] 移除分解配方: {list[num2].item}");
					list.RemoveAt(num2);
					num++;
				}
			}
			field.SetValue(instance, list.ToArray());
			addedDecomposeItemIds.Clear();
			typeof(DecomposeDatabase).GetMethod("RebuildDictionary", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(instance, null);
			Debug.Log((object)$"[VAE] 成功移除 {num} 个分解配方");
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"[VAE] 移除分解配方时出错: {arg}");
		}
	}

	public static ModifierDescription FindModify(Item item, int index)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		FieldInfo field = ((object)item.Modifiers).GetType().GetField("list", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		IList list = field.GetValue(item.Modifiers) as IList;
		return (ModifierDescription)list[index];
	}

	private static ModifierDescription CloneModifier(ModifierDescription source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		ModifierDescription val = new ModifierDescription();
		FieldInfo[] fields = typeof(ModifierDescription).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			fieldInfo.SetValue(val, fieldInfo.GetValue(source));
		}
		return val;
	}

	private static void SetModifierValue(ModifierDescription modifier, float value)
	{
		FieldInfo field = typeof(ModifierDescription).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(modifier, value);
		}
	}

	public static void ResetItemModifier(Item item, ModifierDescriptionWithOffset[] ModifierDescriptionWithOffset)
	{
		item.Modifiers.Clear();
		foreach (ModifierDescriptionWithOffset modifierDescriptionWithOffset in ModifierDescriptionWithOffset)
		{
			ModifierDescription val = CloneModifier(modifierDescriptionWithOffset.modifierDescription);
			SetModifierValue(val, modifierDescriptionWithOffset.modifierOffset);
			item.Modifiers.Add(val);
		}
	}
}
