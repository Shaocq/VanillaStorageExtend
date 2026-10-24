# VanillaStorageExtend 重建源码工程

[![游戏](https://img.shields.io/badge/游戏-Escape%20from%20Duckov-blue)](https://store.steampowered.com/app/3167020/)
[![协议](https://img.shields.io/badge/协议-MIT-yellow)](LICENSE)
[![版本](https://img.shields.io/badge/版本-1.1.1-brightgreen)](CHANGELOG.md)

> 在原版存储体系上做扩展：饭盒 12 格、收藏品箱可放任意物品，并为 mod 容器与原版针包 / 钥匙包加上「点击关闭」按钮。

| 项目 | 信息 |
| --- | --- |
| 游戏 | 逃离鸭科夫（Escape from Duckov，AppID `3167020`） |
| 框架 | `Duckov.Modding.ModBehaviour` |
| 前置 | 无（不依赖 Harmony） |
| 创意工坊 | — |
| 当前版本 | 1.1.1 |
| 开源协议 | [MIT](LICENSE) |

## 安装

把 `VanillaStorageExtend` 文件夹放入 `Duckov_Data\Mods\`。本 mod 不依赖 Harmony。

> **`lib/0Harmony.dll` 不在仓库内。** 需要从源码编译时，请把创意工坊前置 HarmonyLib 目录下的 `0Harmony.dll` 复制到 `lib/` 下。


由 `3590896915_存储拓展/vanillaStorageExtend.dll`（VanillaStorageExtend 1.5，作者 **Kamakoto**，Steam 创意工坊 ID `3590896915`）反编译重建，用于本地分析与自用修改。

> 这是他人作品。自用、做兼容补丁没问题；**重新分发需取得作者授权并署名**。

## 目录结构

```
存储拓展优化/（程序集名仍为 VanillaStorageExtend）
├─ src/                 反编译得到的 C# 源码
│  ├─ ModBehaviour.cs       主逻辑（25 件物品配置）
│  ├─ KamakotoDuckovTools.cs 配方/分解/模组工具
│  ├─ LanguageFileManager.cs 多语言文件管理
│  ├─ ContainerCloseButton.cs 容器详情面板的「点击关闭」按钮
│  └─ ItemExtensions.cs      反射读写私有字段
├─ resources/           24 张内嵌 PNG（原 23 张 + 关闭按钮圆角底图）
├─ language/            10 种语言文本（运行时从 language/ 子目录读取）
├─ info.ini             原 mod 元数据
├─ preview.png
└─ VanillaStorageExtend.csproj
```

## 构建

```bash
dotnet build VanillaStorageExtend.csproj -c Release
```

默认游戏路径为 `E:\Steam\steamapps\common\Escape from Duckov`，可覆盖：

```bash
dotnet build VanillaStorageExtend.csproj -c Release -p:DuckovPath="你的游戏目录"
```

产物：

- `bin/Release/vanillaStorageExtend.dll`
- `bin/Release/Mod/VanillaStorageExtend/` —— 可直接放进 `Duckov_Data\Mods\` 的完整 mod 文件夹

## 重建时的修正

反编译代码不能原样编译，已做 3 处等价修正：

1. `public class ModBehaviour : ModBehaviour` → `: Duckov.Modding.ModBehaviour`（基类型自引用）
2. 增加 `using Object = UnityEngine.Object;`（`System` 与 `UnityEngine` 的 `Object` 冲突）
3. `ItemEntry` 区分限定为 `Duckov.Economy.Cost.ItemEntry`（`Cost.items`）和 `CraftingFormula.ItemEntry`（`CraftingFormula.result`）

## 等价性验证

- 字符串常量集合：原版 189 条 = 重建版 189 条，完全一致
- 内嵌资源：23 个，名称一致
- `tools/parse_items.ps1` 解析出的 25 件物品配置：原版与重建版逐行完全一致

## 常见修改点

- 物品配置全部集中在 `src/ModBehaviour.cs` 的 `OnAfterSetup()` 中，每件物品一个 `ItemConfig`。
- 收藏品保险箱（ID 90024）当前配置：
  - `SlotsTags = new string[0]` + `SlotCount = 12` —— 不限品类，任意物品都能放入；
  - `SlotDisplayNameKey = "Tag_Luxury"` —— 通过反射设置 `Slot.overrideDisplayNameKey`，
    槽位下方仍显示「奢侈品」（`Slot.DisplayName` 优先使用该字段）；
  - `ExcludeTags = new string[1] { ContainerTagName }` —— 排除本 mod 的其他容器。
- 本 mod 的所有自定义物品在 `SetItemProperties` 中都会被打上私有标记 Tag
  `__VSE_Container`（运行时 `ScriptableObject.CreateInstance<Tag>()` 创建，不显示）。
  `TagCollection.Contains` 按名字哈希比较，因此该标记只用于槽位的 `excludeTags` 判断。
- 其他 `ItemConfig` 也可使用 `SlotDisplayNameKey` / `ExcludeTags` 两个新字段。

## 更新日志

完整版本记录见 [CHANGELOG.md](CHANGELOG.md)。

## 许可证

本项目以 [MIT 协议](LICENSE) 开源，可自由使用、修改与再发布，保留版权声明即可。

## 致谢

- 游戏与 mod 框架：TeamSoda《逃离鸭科夫》
- Harmony 补丁框架：[Harmony](https://github.com/pardeike/Harmony)（MIT）
- 本项目源自「**原本存储更新-1.5**」mod（创意工坊 `3590896915`），由反编译的 DLL 重建为可编译源码工程后加以修改。原作者版权归原作者所有；若原作者要求下架，将立即删除。
- 感谢所有在创意工坊反馈问题与建议的玩家
