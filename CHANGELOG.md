# 更新日志

本项目遵循 [语义化版本](https://semver.org/lang/zh-CN/) 与 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 格式。

## [1.0.0] - 2026-10-23

首次开源发布（由反编译 DLL 重建为可编译源码工程后的首个版本）。

### 新增

- 由原 DLL 重建为完整的 C# 源码工程（netstandard2.1，程序集与命名空间保持 `VanillaStorageExtend`）
- 用 `parse_items.ps1` 与原版逐行比对，25 件物品的配置与配方完全一致

### 变更

- **饭盒**扩容为 12 格
- **收藏品箱**可存放任意物品
- 容器详情面板底部新增「点击关闭」按钮（`src/ContainerCloseButton.cs`）
- 原版**针包**（`InjectionBag`）与**钥匙包**（`KeyRing`）同样获得「点击关闭」按钮

### 说明

- 不依赖 Harmony
- 源自创意工坊 `3590896915` 的「原本存储更新-1.5」，源码由反编译重建；原作者版权归原作者所有，若原作者要求下架将立即删除
