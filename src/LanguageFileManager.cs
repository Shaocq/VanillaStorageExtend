using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SodaCraft.Localizations;
using UnityEngine;
using VanillaStorageExtend;

namespace vanillaStorageExtend;

public static class LanguageFileManager
{
	private static Dictionary<SystemLanguage, List<string>> languageCache;

	private static string languageDirectory;

	public static int LocationIndex;

	private static bool DevMode;

	private static Dictionary<SystemLanguage, string> languageFileNames;

	static LanguageFileManager()
	{
		languageCache = new Dictionary<SystemLanguage, List<string>>();
		LocationIndex = 0;
		DevMode = false;
		languageFileNames = new Dictionary<SystemLanguage, string>
		{
			{
				(SystemLanguage)40,
				"zh-CN.txt"
			},
			{
				(SystemLanguage)41,
				"zh-TW.txt"
			},
			{
				(SystemLanguage)10,
				"en-US.txt"
			},
			{
				(SystemLanguage)22,
				"ja-JP.txt"
			},
			{
				(SystemLanguage)23,
				"ko-KR.txt"
			},
			{
				(SystemLanguage)30,
				"ru-RU.txt"
			},
			{
				(SystemLanguage)34,
				"es-ES.txt"
			},
			{
				(SystemLanguage)14,
				"fr-FR.txt"
			},
			{
				(SystemLanguage)15,
				"de-DE.txt"
			},
			{
				(SystemLanguage)28,
				"pt-PT.txt"
			}
		};
		string location = Assembly.GetExecutingAssembly().Location;
		string directoryName = Path.GetDirectoryName(location);
		languageDirectory = Path.Combine(directoryName, "language");
	}

	public static void InitializeLanguageFiles()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		DevMode = true;
		if (!Directory.Exists(languageDirectory))
		{
			Directory.CreateDirectory(languageDirectory);
		}
		string path = Path.Combine(languageDirectory, languageFileNames[(SystemLanguage)40]);
		File.WriteAllText(path, "");
		foreach (SystemLanguage key in languageFileNames.Keys)
		{
			if ((int)key != 40)
			{
				string path2 = Path.Combine(languageDirectory, languageFileNames[key]);
				if (!File.Exists(path2))
				{
					File.WriteAllText(path2, "");
				}
			}
		}
	}

	public static void AddLineToChinese(string text)
	{
		string path = Path.Combine(languageDirectory, languageFileNames[(SystemLanguage)40]);
		File.AppendAllText(path, text + Environment.NewLine);
	}

	public static string GetText(SystemLanguage language, int lineIndex)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!languageCache.ContainsKey(language))
		{
			LoadLanguageFile(language);
		}
		if (languageCache[language] != null && lineIndex < languageCache[language].Count)
		{
			return languageCache[language][lineIndex];
		}
		return null;
	}

	private static void LoadLanguageFile(SystemLanguage language)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!languageFileNames.ContainsKey(language))
		{
			languageCache[language] = new List<string>();
			return;
		}
		string path = Path.Combine(languageDirectory, languageFileNames[language]);
		if (File.Exists(path))
		{
			string[] collection = File.ReadAllLines(path);
			languageCache[language] = new List<string>(collection);
		}
		else
		{
			languageCache[language] = new List<string>();
		}
	}

	public static void SetLocalizationTexts(ModBehaviour.ItemConfig config)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		SystemLanguage currentLanguage = LocalizationManager.CurrentLanguage;
		string value = config.DisplayName;
		string text = config.LocalizationDescValue;
		if (DevMode)
		{
			AddLineToChinese(config.DisplayName);
			AddLineToChinese(config.LocalizationDescValue);
		}
		if ((int)currentLanguage != 40)
		{
			string text2 = GetText(currentLanguage, LocationIndex * 2);
			string text3 = GetText(currentLanguage, LocationIndex * 2 + 1);
			if (!string.IsNullOrEmpty(text2))
			{
				value = text2;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text = text3;
			}
		}
		LocalizationManager.overrideTexts.Add(config.LocalizationKey, value);
		LocalizationManager.overrideTexts.Add(config.LocalizationDesc, text + "\n\n<color=grey>[Mod:VanillaStorageExtend]</color>");
		LocationIndex++;
	}
}
