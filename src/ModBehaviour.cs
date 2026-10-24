using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Duckov.Modding;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;
using vanillaStorageExtend;
using Object = UnityEngine.Object;

namespace VanillaStorageExtend;

public class ModBehaviour : Duckov.Modding.ModBehaviour
{
	private class ResourceHolder : MonoBehaviour
	{
		public Texture2D iconTexture;

		public Sprite iconSprite;
	}

	public struct ItemConfig
	{
		public int OriginalItemId = 0;

		public int NewItemId = 0;

		public string DisplayName = null;

		public string LocalizationKey = null;

		public string LocalizationDescValue = null;

		public float Weight = 0f;

		public int Value = 0;

		public string[] SlotsTags = null;

		public int SlotCount = 0;

		// 槽位下方显示文字的本地化 key（为空则沿用 requireTags[0] 的显示名）
		public string SlotDisplayNameKey = null;

		// 这些 Tag 会加入该物品每个槽位的 excludeTags；"__VSE_Container" 表示本 mod 的容器标记
		public string[] ExcludeTags = null;

		public string EmbeddedSpritePath = null;

		public int Quality = 0;

		public string[] Tags = null;

		public KamakotoDuckovTools.ModifierDescriptionWithOffset[] ModifierDescriptionWithOffset = null;

		public bool EasyFormula = false;

		public (int id, long amount)[] FormulaInputs = null;

		public int FormulaMoney = 0;

		public int DecomMoney = 0;

		public KamakotoDuckovTools.DecomMode DecomMode = KamakotoDuckovTools.DecomMode.Auto;

		public (int id, long amount)[] DecomResult = null;

		public int StackNumber = -1;

		public string LocalizationDesc => LocalizationKey + "_Desc";

		public ItemConfig()
		{
		}
	}

	public static int EazyFormulaStart = 720000;

	public static int ItemKeyStart = 1;

	// 本 mod 容器的私有标记 Tag 名（不会与游戏自带 Tag 重名）
	public const string ContainerTagName = "__VSE_Container";

	private static Tag containerTag;

	private ContainerCloseButton containerCloseButton;

	protected override void OnAfterSetup()
	{
		Diag.Log($"OnAfterSetup 开始，程序集={Assembly.GetExecutingAssembly().Location}");
		int item = 50;
		int item2 = 49;
		int item3 = 48;
		int item4 = 764;
		int item5 = 362;
		ItemConfig itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90001;
		itemConfig.DisplayName = "便携式武器箱Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用小扩容箱改进的便携式武器箱MK1，经过轻量化设计，容量降低但易于携带，可以收纳3把武器。";
		itemConfig.Weight = 2.5f;
		itemConfig.Value = 2000;
		itemConfig.SlotsTags = new string[1] { "Weapon" };
		itemConfig.SlotCount = 3;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Weapen01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 1500;
		itemConfig.FormulaInputs = new (int, long)[4]
		{
			(item, 1L),
			(367, 10L),
			(item5, 10L),
			(item4, 10L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item, 1L) };
		ItemConfig config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90002;
		itemConfig.DisplayName = "便携式武器箱Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用中扩容箱改进的便携式武器箱MK2。经过轻量化设计，容量降低但易于携带，可以收纳6把武器。";
		itemConfig.Weight = 4f;
		itemConfig.Value = 5000;
		itemConfig.SlotsTags = new string[1] { "Weapon" };
		itemConfig.SlotCount = 6;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Weapen02.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 4000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90001, 1L),
			(item2, 1L),
			(662, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90003;
		itemConfig.DisplayName = "武器存储单元Mk3";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量武器。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 10000;
		itemConfig.SlotsTags = new string[1] { "Weapon" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Weapen03.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 7000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90002, 1L),
			(item3, 1L),
			(663, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90021;
		itemConfig.DisplayName = "便携式装备箱Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用小扩容箱改进的便携式装备箱MK1，经过轻量化设计，容量降低但易于携带，可以收纳3种不同的装备。";
		itemConfig.Weight = 2.5f;
		itemConfig.Value = 3000;
		itemConfig.SlotsTags = new string[1] { "Equipment" };
		itemConfig.SlotCount = 3;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.BoxS.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 1500;
		itemConfig.FormulaInputs = new (int, long)[4]
		{
			(item, 1L),
			(743, 10L),
			(item5, 10L),
			(item4, 10L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90022;
		itemConfig.DisplayName = "便携式装备箱Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用中扩容箱改进的便携式装备箱MK2。经过轻量化设计，容量降低但易于携带，可以收纳6种不同的装备。";
		itemConfig.Weight = 4f;
		itemConfig.Value = 8000;
		itemConfig.SlotsTags = new string[1] { "Equipment" };
		itemConfig.SlotCount = 6;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.BoxM.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 3500;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90021, 1L),
			(item2, 1L),
			(1170, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90023;
		itemConfig.DisplayName = "装备存储单元Mk3";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量装备。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 18000;
		itemConfig.SlotsTags = new string[1] { "Equipment" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.EquipBox.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 6000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90022, 1L),
			(item3, 1L),
			(1171, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90004;
		itemConfig.DisplayName = "便携式弹药箱Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用小扩容箱改进的弹药箱，经过轻量化设计，容量降低但易于携带，可以收纳5组弹药。用之前需要手动将弹药卸下。";
		itemConfig.Weight = 2.5f;
		itemConfig.Value = 3500;
		itemConfig.SlotsTags = new string[1] { "Bullet" };
		itemConfig.SlotCount = 5;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Ammo01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 1800;
		itemConfig.FormulaInputs = new (int, long)[4]
		{
			(item, 1L),
			(367, 10L),
			(item5, 10L),
			(item4, 10L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90014;
		itemConfig.DisplayName = "便携式弹药箱Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用中扩容箱改进的弹药箱，经过轻量化设计，容量降低但易于携带，可以收纳8组弹药。用之前需要手动将弹药卸下。";
		itemConfig.Weight = 4f;
		itemConfig.Value = 8000;
		itemConfig.SlotsTags = new string[1] { "Bullet" };
		itemConfig.SlotCount = 8;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Ammo01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 4000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90004, 1L),
			(item2, 1L),
			(662, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90005;
		itemConfig.DisplayName = "弹药存储单元Mk3";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量弹药。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 18000;
		itemConfig.SlotsTags = new string[1] { "Bullet" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Ammo02.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 7000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90014, 1L),
			(item3, 1L),
			(663, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90007;
		itemConfig.DisplayName = "便携式医疗箱Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用小扩容箱改进的医疗箱，经过轻量化设计，容量降低但易于携带，可以收纳5种不同的医疗用品。用之前需要手动将药品卸下。";
		itemConfig.Weight = 1.5f;
		itemConfig.Value = 4000;
		itemConfig.SlotsTags = new string[1] { "Medic" };
		itemConfig.SlotCount = 5;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.MedBox01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 2000;
		itemConfig.FormulaInputs = new (int, long)[4]
		{
			(item, 1L),
			(743, 10L),
			(item5, 10L),
			(item4, 10L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90015;
		itemConfig.DisplayName = "便携式医疗箱Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用中扩容箱改进的医疗箱，经过轻量化设计，容量降低但易于携带，可以收纳8种不同的医疗用品。用之前需要手动将药品卸下。";
		itemConfig.Weight = 2.5f;
		itemConfig.Value = 9000;
		itemConfig.SlotsTags = new string[1] { "Medic" };
		itemConfig.SlotCount = 8;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.MedBox01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 4500;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90007, 1L),
			(item2, 1L),
			(1170, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90016;
		itemConfig.DisplayName = "医疗物资存储单元Mk3";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量医疗物资。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 10000;
		itemConfig.SlotsTags = new string[1] { "Medic" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.MedBox.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 10000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90015, 1L),
			(item3, 1L),
			(1171, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90017;
		itemConfig.DisplayName = "投掷物收纳包Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用小扩容箱改进的旅行包。不知道橘子是如何把箱子塞到包里的，但总之可以收纳4种不同的投掷物。用之前需要手动将东西拿出来。";
		itemConfig.Weight = 2.5f;
		itemConfig.Value = 3500;
		itemConfig.SlotsTags = new string[1] { "Explosive" };
		itemConfig.SlotCount = 4;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Boom.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 1800;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(item, 1L),
			(38, 1L),
			(367, 10L),
			(item5, 10L),
			(item4, 10L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90008;
		itemConfig.DisplayName = "投掷物收纳包Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用中扩容箱改进的旅行包。不知道橘子是如何把箱子塞到包里的，但总之可以收纳8种不同的投掷物。用之前需要手动将东西拿出来。";
		itemConfig.Weight = 4f;
		itemConfig.Value = 8000;
		itemConfig.SlotsTags = new string[1] { "Explosive" };
		itemConfig.SlotCount = 8;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Boom.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 4000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90017, 1L),
			(item2, 1L),
			(662, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90018;
		itemConfig.DisplayName = "投掷物存储单元Mk3";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量投掷物。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 10000;
		itemConfig.SlotsTags = new string[1] { "Explosive" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.BoomBox.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 10000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90008, 1L),
			(item3, 1L),
			(663, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90013;
		itemConfig.DisplayName = "便携式配件胸挂Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "由三级防弹衣改进而成的胸挂，可以容纳6个武器配件。";
		itemConfig.Weight = 3.5f;
		itemConfig.Value = 4000;
		itemConfig.SlotsTags = new string[1] { "Accessory" };
		itemConfig.SlotCount = 6;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.AccBag01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 4000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(item2, 1L),
			(2, 1L),
			(662, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90019;
		itemConfig.DisplayName = "武器配件存储单元Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量武器配件。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 10000;
		itemConfig.SlotsTags = new string[1] { "Accessory" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.AccBox.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 10000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90013, 1L),
			(item3, 1L),
			(663, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90011;
		itemConfig.DisplayName = "特殊物质存储罐Mk1";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "得益于空间风暴的进一步研究。在对各种容器进行测试后，小明发现可以通过对金属桶进行改良从而使其可以容纳特殊物质。可以容纳6组不同的特殊物品。";
		itemConfig.Weight = 3.5f;
		itemConfig.Value = 5000;
		itemConfig.SlotsTags = new string[1] { "Special" };
		itemConfig.SlotCount = 6;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.GasBox02.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 5000;
		itemConfig.FormulaInputs = new (int, long)[4]
		{
			(item2, 1L),
			(1170, 10L),
			(item5, 15L),
			(item4, 15L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90020;
		itemConfig.DisplayName = "特殊物质存储单元Mk2";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "可以容纳大量特殊物品。";
		itemConfig.Weight = 50f;
		itemConfig.Value = 10000;
		itemConfig.SlotsTags = new string[1] { "Special" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.SpecBox.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 10000;
		itemConfig.FormulaInputs = new (int, long)[5]
		{
			(90011, 1L),
			(item3, 1L),
			(1171, 10L),
			(item5, 20L),
			(item4, 20L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90006;
		itemConfig.DisplayName = "饭盒";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "一个用纸盒改造的饭盒，出门在外也要好好吃饭。差不多就回来。";
		itemConfig.Weight = 1.5f;
		itemConfig.Value = 1000;
		itemConfig.SlotsTags = new string[1] { "Food" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.FanBox01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 1500;
		itemConfig.FormulaInputs = new (int, long)[4]
		{
			(item, 1L),
			(881, 2L),
			(888, 2L),
			(428, 2L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90009;
		itemConfig.DisplayName = "技术手册";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "把图纸夹在书里面不就好了？";
		itemConfig.Weight = 2f;
		itemConfig.Value = 3000;
		itemConfig.SlotsTags = new string[1] { "Formula" };
		itemConfig.SlotCount = 10;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.BluePrint.png";
		itemConfig.EasyFormula = true;
		// 技术手册只在开局有用，制造改为纯 100 元、不需要材料
		itemConfig.FormulaMoney = 100;
		itemConfig.FormulaInputs = new (int, long)[0];
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		// 分解回自身，避免「100 元造出来再拆成材料」的刷钱循环
		itemConfig.DecomResult = new (int, long)[1] { (90009, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90010;
		itemConfig.DisplayName = "菜篮子";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "不知道为什么装不进去足球，可能它不是日常用品吧。";
		itemConfig.Weight = 3f;
		itemConfig.Value = 1500;
		itemConfig.SlotsTags = new string[1] { "Daily" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 4;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Basket.png";
		itemConfig.EasyFormula = true;
		// 菜篮子只在开局有用，制造改为纯 100 元、不需要材料
		itemConfig.FormulaMoney = 100;
		itemConfig.FormulaInputs = new (int, long)[0];
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		// 分解回自身，避免「100 元造出来再拆成材料」的刷钱循环
		itemConfig.DecomResult = new (int, long)[1] { (90010, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90012;
		itemConfig.DisplayName = "TOMSUNG Pro Max";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "TOMSUNG会社推出最新型号手机。配备了128MB的超大内存，同时集成了TUTOM数字扫描功能，可以将实体图腾变成图腾.zip文件存入到手机中。";
		itemConfig.Weight = 5f;
		itemConfig.Value = 5000;
		itemConfig.SlotsTags = new string[1] { "Totem" };
		itemConfig.SlotCount = 12;
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.TomSung01.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 5000;
		itemConfig.FormulaInputs = new (int, long)[7]
		{
			(item3, 1L),
			(114, 2L),
			(339, 2L),
			(116, 2L),
			(765, 2L),
			(833, 2L),
			(834, 2L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 0;
		itemConfig.NewItemId = 90025;
		itemConfig.DisplayName = "木头模特";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "用木头和胶水粘起来的架子，可以把它打扮的和你一样帅。";
		itemConfig.Weight = 5f;
		itemConfig.Value = 5000;
		itemConfig.SlotsTags = new string[0];
		itemConfig.SlotCount = 0;
		itemConfig.Quality = 5;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.Fakeman.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 5000;
		itemConfig.FormulaInputs = new (int, long)[7]
		{
			(item2, 1L),
			(361, 10L),
			(332, 4L),
			(335, 4L),
			(765, 2L),
			(833, 2L),
			(834, 2L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item2, 1L) };
		config = itemConfig;
		CreateCustomItem(config);
		itemConfig = new ItemConfig();
		itemConfig.OriginalItemId = 1255;
		itemConfig.NewItemId = 90024;
		itemConfig.DisplayName = "收藏品保险箱";
		itemConfig.LocalizationKey = $"VanillaStorageExtend{ItemKeyStart}";
		itemConfig.LocalizationDescValue = "虽然说是保险箱，但实际上并没有锁。里面只能放你的奢侈品收藏，这样丢了才会心疼。";
		itemConfig.Weight = 10f;
		itemConfig.Value = 10000;
		itemConfig.SlotsTags = new string[0];
		itemConfig.SlotCount = 12;
		// 槽位不限品类，但显示文字仍沿用「奢侈品」
		itemConfig.SlotDisplayNameKey = "Tag_Luxury";
		// 拒绝放入本 mod 的其他容器（包括其他收藏品保险箱）
		itemConfig.ExcludeTags = new string[1] { ContainerTagName };
		itemConfig.Quality = 6;
		itemConfig.Tags = new string[0];
		itemConfig.EmbeddedSpritePath = "vanillaStorageExtend.LuxuryBox.png";
		itemConfig.EasyFormula = true;
		itemConfig.FormulaMoney = 10000;
		itemConfig.FormulaInputs = new (int, long)[7]
		{
			(item3, 1L),
			(388, 2L),
			(item5, 20L),
			(item4, 20L),
			(765, 2L),
			(833, 2L),
			(834, 2L)
		};
		itemConfig.DecomMode = KamakotoDuckovTools.DecomMode.Free;
		itemConfig.DecomMoney = 0;
		itemConfig.DecomResult = new (int, long)[1] { (item3, 1L) };
		config = itemConfig;
		CreateCustomItem(config);

		// 容器详情面板的「点击关闭」按钮。不依赖 Harmony，只监听游戏的选中事件
		try
		{
			containerCloseButton = GetComponent<ContainerCloseButton>();
			if (containerCloseButton == null)
			{
				containerCloseButton = gameObject.AddComponent<ContainerCloseButton>();
			}
			Diag.Log($"容器操作按钮组件已挂载：{(containerCloseButton != null ? "成功" : "失败")}");
		}
		catch (Exception e)
		{
			Diag.Log("注入容器操作按钮组件失败：" + e);
			Debug.LogWarning((object)("[VanillaStorageExtend] 注入「点击关闭」按钮失败：" + e));
		}

		Diag.Log("OnAfterSetup 完成");
	}

	protected override void OnBeforeDeactivate()
	{
		Diag.Log("OnBeforeDeactivate：开始卸载");
		if (containerCloseButton != null)
		{
			containerCloseButton.Cleanup();
			Object.Destroy((Object)(object)containerCloseButton);
			containerCloseButton = null;
		}
		KamakotoDuckovTools.RemoveAllAddedFormulas();
		Debug.Log((object)"[VanillaStorageExtend] MOD 即将卸载");
	}

	private Sprite LoadEmbeddedSprite(string resourceName, int NEW_ITEM_ID)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected Obj, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected Obj, but got Unknown
		try
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resourceName);
			if (manifestResourceStream == null)
			{
				Debug.LogError((object)("[VanillaStorageExtend] 嵌入资源未找到: " + resourceName));
				executingAssembly = Assembly.GetExecutingAssembly();
				string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
				string[] array = manifestResourceNames;
				foreach (string text in array)
				{
					Debug.Log((object)text);
				}
				return null;
			}
			byte[] array2 = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read(array2, 0, array2.Length);
			manifestResourceStream.Close();
			Texture2D val = new Texture2D(2, 2, (TextureFormat)4, false);
			if (!ImageConversion.LoadImage(val, array2))
			{
				Debug.LogError((object)"[VanillaStorageExtend] 图片加载失败（非 PNG/JPG？）");
				return null;
			}
			((Texture)val).filterMode = (FilterMode)1;
			val.Apply();
			Sprite val2 = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 100f);
			GameObject val3 = new GameObject($"IconHolder_{NEW_ITEM_ID}");
			Object.DontDestroyOnLoad((Object)(object)val3);
			ResourceHolder resourceHolder = val3.AddComponent<ResourceHolder>();
			resourceHolder.iconTexture = val;
			resourceHolder.iconSprite = val2;
			return val2;
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"[VanillaStorageExtend] 加载图标出错: {arg}");
			return null;
		}
	}

	private void AddSlotToItem(Item item, Tag slot)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected Obj, but got Unknown
		Slot val = item.Slots[0];
		Slot val2 = new Slot(val.Key);
		val2.SlotIcon = val.SlotIcon;
		typeof(Slot).GetField("key", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(val2, $"Key{item.Slots.Count}");
		val2.requireTags.Add(slot);
		item.Slots.Add(val2);
	}

	public void CreateCustomItem(ItemConfig config, bool addTags = false)
	{
		try
		{
			Item prefab = ItemAssetsCollection.GetPrefab(config.OriginalItemId);
			if ((Object)(object)prefab == (Object)null)
			{
				Debug.LogError((object)$"[VanillaStorageExtend] 原始物品 ID {config.OriginalItemId} 不存在！");
				return;
			}
			GameObject val = Object.Instantiate<GameObject>(((Component)prefab).gameObject);
			((Object)val).name = $"ClonedItem_{config.NewItemId}";
			Object.DontDestroyOnLoad((Object)(object)val);
			Item component = val.GetComponent<Item>();
			SetItemProperties(component, config);
			LanguageFileManager.SetLocalizationTexts(config);
			// 标签为空但仍有槽位数时也要重建槽位（空标签列表 = 不限品类）
			if (config.SlotsTags.Count() != 0 || config.SlotCount > 0)
			{
				List<Tag> list = new List<Tag>();
				string[] slotsTags = config.SlotsTags;
				foreach (string tagName in slotsTags)
				{
					list.Add(GetTargetTag(tagName));
				}
				if (addTags)
				{
					foreach (Tag item in list)
					{
						AddSlotToItem(component, item);
					}
				}
				else
				{
					ConfigureSlots(component, list, config.SlotCount);
				}
			}
			ApplySlotRestrictions(component, config);
			SetItemIcon(component, prefab, config);
			RegisterItem(component, config.NewItemId, val);
			switch (config.DecomMode)
			{
			case KamakotoDuckovTools.DecomMode.Free:
				KamakotoDuckovTools.AddDecomposeFormula(config.NewItemId, config.DecomMoney, config.DecomResult);
				break;
			case KamakotoDuckovTools.DecomMode.Auto:
				KamakotoDuckovTools.AddDecomposeFormula(config.NewItemId, config.DecomMoney, KamakotoDuckovTools.AutoCalcDecomResult(config));
				break;
			}
			if (config.EasyFormula)
			{
				KamakotoDuckovTools.AddCraftingFormula(EazyFormulaStart.ToString(), config.FormulaMoney, config.FormulaInputs, config.NewItemId, 1, new string[1] { "WorkBenchAdvanced" });
				EazyFormulaStart++;
			}
			ItemKeyStart++;
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"[VanillaStorageExtend] 初始化出错: {arg}");
		}
	}

	private void SetItemProperties(Item item, ItemConfig config)
	{
		item.SetPrivateField("typeID", config.NewItemId);
		item.SetPrivateField("weight", config.Weight);
		item.SetPrivateField("value", config.Value);
		item.SetPrivateField("displayName", config.LocalizationKey);
		item.SetPrivateField("quality", config.Quality);
		item.SetPrivateField("order", 0);
		item.Tags.Clear();
		string[] tags = config.Tags;
		foreach (string tagName in tags)
		{
			item.Tags.Add(GetTargetTag(tagName));
		}
		// 本 mod 的所有自定义物品都打上私有容器标记，供槽位 excludeTags 排斥
		item.Tags.Add(GetOrCreateContainerTag());
	}

	// 创建供本 mod 内部使用的容器标记 Tag（不显示、纯占位）
	private static Tag GetOrCreateContainerTag()
	{
		if ((Object)containerTag == null)
		{
			containerTag = ScriptableObject.CreateInstance<Tag>();
			containerTag.name = ContainerTagName;
		}
		return containerTag;
	}

	// 按配置设置槽位显示文字与排除标签
	private void ApplySlotRestrictions(Item item, ItemConfig config)
	{
		if (!string.IsNullOrEmpty(config.SlotDisplayNameKey))
		{
			FieldInfo field = typeof(Slot).GetField("overrideDisplayNameKey", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				foreach (Slot slot in item.Slots)
				{
					field.SetValue(slot, config.SlotDisplayNameKey);
				}
			}
			else
			{
				Debug.LogWarning((object)"[VanillaStorageExtend] 未找到 Slot.overrideDisplayNameKey");
			}
		}
		if (config.ExcludeTags == null)
		{
			return;
		}
		foreach (string tagName in config.ExcludeTags)
		{
			Tag tag = ((tagName == ContainerTagName) ? GetOrCreateContainerTag() : GetTargetTag(tagName));
			if ((Object)tag == null)
			{
				Debug.LogWarning((object)$"[VanillaStorageExtend] 排除标签 {tagName} 不存在，已跳过");
				continue;
			}
			foreach (Slot slot in item.Slots)
			{
				slot.excludeTags.Add(tag);
			}
		}
	}

	private Tag GetTargetTag(string tagName)
	{
		Tag[] source = Resources.FindObjectsOfTypeAll<Tag>();
		Debug.Log((object)$"共找到 {source.Count()} Tags");
		return source.FirstOrDefault((Tag t) => ((Object)t).name == tagName);
	}

	private void ConfigureSlots(Item item, List<Tag> slots, int slotCount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected Obj, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected Obj, but got Unknown
		Slot val = new Slot();
		if (item.Slots.Count != 0)
		{
			val = item.Slots[0];
			item.Slots.Clear();
		}
		if (slots.Count > 1)
		{
			foreach (Tag slot in slots)
			{
				Slot val2 = new Slot(val.Key);
				val2.SlotIcon = val.SlotIcon;
				typeof(Slot).GetField("key", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(val2, $"Key{slots.IndexOf(slot)}");
				val2.requireTags.Add(slot);
				item.Slots.Add(val2);
			}
			return;
		}
		for (int i = 0; i < slotCount; i++)
		{
			Slot val3 = new Slot(val.Key);
			val3.SlotIcon = val.SlotIcon;
			typeof(Slot).GetField("key", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(val3, $"Key{i}");
			// 空标签列表时不添加 requireTags，TagCollection.Check 会直接放行任意物品
			if (slots.Count > 0)
			{
				val3.requireTags.Add(slots[0]);
			}
			item.Slots.Add(val3);
		}
	}

	private void SetItemIcon(Item newItem, Item original, ItemConfig config)
	{
		FieldInfo field = typeof(Item).GetField("icon", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			Sprite val = LoadEmbeddedSprite(config.EmbeddedSpritePath, config.NewItemId);
			field.SetValue(newItem, val ?? original.Icon);
		}
	}

	private static void RegisterItem(Item item, int itemId, GameObject clonedGO)
	{
		if (ItemAssetsCollection.AddDynamicEntry(item))
		{
			Diag.Log($"注册新物品成功：ID={itemId}");
			Debug.Log((object)$"[VanillaStorageExtend] 成功注册新物品！ID: {itemId}");
			return;
		}
		Diag.Log($"注册新物品失败：ID={itemId}");
		Debug.LogError((object)"[VanillaStorageExtend] 注册失败！");
		Object.Destroy((Object)(object)clonedGO);
	}
}
