using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace VanillaStorageExtend
{
    /// <summary>
    /// 在容器详情面板的最下方注入「全部移出」和「点击关闭」两个按钮。
    ///
    /// 「点击关闭」通过公开的 <see cref="ItemUIUtilities.Select"/> 走与再次点击物品
    /// 相同的取消选中流程，面板会按原版逻辑淡出；
    /// 「全部移出」按角色背包的空闲格数，把容器槽位里的物品尽量转移进背包，
    /// 全部转移成功后再自动关闭详情面板。
    ///
    /// 仅在选中「本 mod 的容器物品」或原版钥匙包、注射器收纳包时显示，
    /// 其他物品保持原样。
    /// </summary>
    internal sealed class ContainerCloseButton : MonoBehaviour
    {
        private const string RowName = "VSE_ButtonRow";
        private const string MoveAllButtonName = "VSE_MoveAllButton";
        private const string MoveAllButtonLabel = "全部移出";
        private const string CloseButtonName = "VSE_CloseButton";
        private const string CloseButtonLabel = "点击关闭";

        /// <summary>两个按钮之间的水平间距。</summary>
        private const float ButtonGap = 8f;

        /// <summary>按钮行高度（含上下留白）。</summary>
        private const float RowHeight = 64f;

        /// <summary>按钮在行内的上下留白，视觉高度为 RowHeight - 16 = 48。</summary>
        private const float VerticalMargin = 8f;

        /// <summary>按下时相对基准尺寸的偏移，与原版「分割」按钮的 (-8, -8) 一致。</summary>
        private const float PressOffset = 8f;

        private static readonly Vector2 PressDelta = new Vector2(-PressOffset, -PressOffset);

        // 与「物品防吞」按钮一致的绿色
        private static readonly Color32 ButtonGreen = new Color32(46, 160, 67, 255);

        /// <summary>原版「分割」按钮使用的圆角九宫格底图（内嵌资源）。</summary>
        private const string BackgroundResourceName = "vanillaStorageExtend.CloseButtonBg.png";

        /// <summary>底图四边边框，取自原版 Sprite「Rect16」（64x64，四边 17 像素）。</summary>
        private const float BackgroundBorder = 17f;

        /// <summary>底图 PPU，与原版 Sprite 一致。</summary>
        private const float BackgroundPixelsPerUnit = 100f;

        /// <summary>原版钥匙包（KeyRing）的 TypeID。</summary>
        private const int VanillaKeyRingTypeId = 836;

        /// <summary>原版注射器收纳包（InjectionBag）的 TypeID。</summary>
        private const int VanillaInjectionBagTypeId = 882;

        private Coroutine refreshRoutine;
        private Sprite buttonBackground;
        private ItemDetailsDisplay currentDisplay;
        private GameObject currentRow;
        private readonly List<ButtonEntry> currentButtons = new List<ButtonEntry>();

        /// <summary>记录按钮的 RectTransform 与未按下时的尺寸，用于隐藏面板时复位。</summary>
        private struct ButtonEntry
        {
            public RectTransform Rect;
            public Vector2 IdleSize;
        }

        private float nextFallbackCheck;

        private void OnEnable()
        {
            ItemUIUtilities.OnSelectionChanged += OnSelectionChanged;
            Diag.Log("ContainerCloseButton.OnEnable：已订阅选中事件");

            // mod 重新启用时可能已有选中物品，主动刷新一次
            OnSelectionChanged();
        }

        private void OnDisable()
        {
            ItemUIUtilities.OnSelectionChanged -= OnSelectionChanged;
            Diag.Log("ContainerCloseButton.OnDisable：已取消订阅");
            Cleanup();
        }

        /// <summary>
        /// 兜底：个别界面可能只更新详情面板而不触发选中事件，
        /// 每 0.5 秒检查一次，发现按钮缺失就主动补上。
        /// </summary>
        private void Update()
        {
            if (Time.unscaledTime < nextFallbackCheck)
            {
                return;
            }
            nextFallbackCheck = Time.unscaledTime + 0.5f;

            try
            {
                Item selected = ItemUIUtilities.SelectedItem;
                if (!IsSupportedContainer(selected))
                {
                    return;
                }

                // 已经是正确的面板且按钮在显示时无需处理
                if (currentRow != null && currentRow.activeSelf
                    && currentDisplay != null && currentDisplay.Target == selected
                    && currentDisplay.gameObject.activeInHierarchy)
                {
                    return;
                }

                Diag.Log("兜底检查：按钮缺失，主动刷新");
                if (refreshRoutine != null)
                {
                    StopCoroutine(refreshRoutine);
                }
                refreshRoutine = StartCoroutine(RefreshNextFrame());
            }
            catch (Exception e)
            {
                Diag.Log("兜底检查异常：" + e);
            }
        }

        /// <summary>mod 卸载时立即清掉按钮，避免界面里残留失效对象。</summary>
        internal void Cleanup()
        {
            if (refreshRoutine != null)
            {
                StopCoroutine(refreshRoutine);
                refreshRoutine = null;
            }
            DestroyRow();
            DestroyBackground();
            currentDisplay = null;
        }

        private void OnSelectionChanged()
        {
            Diag.Log($"选中事件：{Describe(ItemUIUtilities.SelectedItem)}");

            // 选中事件可能连续触发，只保留最后一次刷新
            if (refreshRoutine != null)
            {
                StopCoroutine(refreshRoutine);
            }
            refreshRoutine = StartCoroutine(RefreshNextFrame());
        }

        /// <summary>把物品描述成便于排查的文本。</summary>
        private static string Describe(Item item)
        {
            if (item == null)
            {
                return "null";
            }
            bool hasContainerTag = item.Tags != null && item.Tags.Contains(ModBehaviour.ContainerTagName);
            return $"「{item.DisplayName}」TypeID={item.TypeID} 槽位={item.Slots?.Count ?? -1} 容器标签={hasContainerTag}";
        }

        /// <summary>把节点及其直接子节点列成一行，方便排查布局根。</summary>
        private static string DescribeHierarchy(Transform root)
        {
            if (root == null)
            {
                return "null";
            }
            List<string> children = new List<string>();
            for (int i = 0; i < root.childCount && i < 20; i++)
            {
                Transform child = root.GetChild(i);
                children.Add(child.name + (child.gameObject.activeSelf ? string.Empty : "(隐藏)"));
            }
            return root.name + " -> [" + string.Join(", ", children) + "]";
        }

        private IEnumerator RefreshNextFrame()
        {
            // 等一帧，确保 LootView / InventoryView 已经用新物品调用过
            // ItemDetailsDisplay.Setup，再决定按钮的显示与挂载位置。
            yield return null;
            refreshRoutine = null;

            try
            {
                Refresh();
            }
            catch (Exception e)
            {
                Diag.Log("刷新容器操作按钮异常：" + e);
                Debug.LogWarning((object)("[VanillaStorageExtend] 刷新容器操作按钮失败：" + e));
                HideCurrentRow();
            }
        }

        private void Refresh()
        {
            Item selected = ItemUIUtilities.SelectedItem;
            View activeView = View.ActiveView;
            Diag.Log(
                "Refresh：选中=" + Describe(selected)
                + "，支持=" + IsSupportedContainer(selected)
                + "，ActiveView=" + (activeView == null ? "null" : activeView.GetType().Name));
            if (!IsSupportedContainer(selected))
            {
                HideCurrentRow();
                return;
            }

            ItemDetailsDisplay display = FindDisplay(selected);
            if (display == null)
            {
                ItemDetailsDisplay[] all = Object.FindObjectsOfType<ItemDetailsDisplay>(true);
                List<string> info = new List<string>();
                for (int i = 0; i < all.Length; i++)
                {
                    ItemDetailsDisplay candidate = all[i];
                    info.Add(candidate == null
                        ? "null"
                        : $"{candidate.name}:{Describe(candidate.Target)}:激活={candidate.gameObject.activeInHierarchy}");
                }
                Diag.Log($"Refresh：未找到展示该物品的 ItemDetailsDisplay，候选 {all.Length} 个 -> {string.Join(" | ", info)}");
                HideCurrentRow();
                return;
            }
            Diag.Log($"Refresh：找到详情面板「{display.name}」（激活={display.gameObject.activeInHierarchy}）");

            // 切换了展示面板（例如从背包换到战利品界面）时，在目标面板上重建按钮
            if (currentRow == null || currentDisplay != display)
            {
                Diag.Log("Refresh：在目标面板上重建按钮行");
                DestroyRow();
                currentDisplay = display;
                currentRow = CreateRow(display);
            }
            else
            {
                Diag.Log("Refresh：复用已有按钮行");
            }

            if (currentRow != null)
            {
                // 复位可能残留的按下尺寸，再显示
                ResetButtonSize();
                currentRow.SetActive(true);
                MarkLayoutDirty(currentRow);
            }
        }

        private void HideCurrentRow()
        {
            if (currentRow != null && currentRow.activeSelf)
            {
                ResetButtonSize();
                currentRow.SetActive(false);
                MarkLayoutDirty(currentRow);
            }
        }

        private void ResetButtonSize()
        {
            foreach (ButtonEntry entry in currentButtons)
            {
                if (entry.Rect != null)
                {
                    entry.Rect.sizeDelta = entry.IdleSize;
                }
            }
        }

        /// <summary>
        /// 识别需要「点击关闭」按钮的容器：
        /// 本 mod 的扩展容器，以及原版钥匙包（KeyRing）、注射器收纳包（InjectionBag）。
        /// 都要求物品本身带有槽位。
        /// </summary>
        private static bool IsSupportedContainer(Item item)
        {
            if (item == null || item.Slots == null || item.Slots.Count <= 0)
            {
                return false;
            }

            // 本 mod 创建并注册的扩展容器
            if (item.Tags != null && item.Tags.Contains(ModBehaviour.ContainerTagName))
            {
                return true;
            }

            // 原版钥匙包 / 注射器收纳包，TypeID 来自运行时导出的 vanilla_stack_limits.csv
            int typeId = item.TypeID;
            return typeId == VanillaKeyRingTypeId || typeId == VanillaInjectionBagTypeId;
        }

        /// <summary>
        /// 找到当前正在展示该物品的详情面板。
        /// LootView 与 InventoryView 各自持有一个 ItemDetailsDisplay，
        /// 关闭的视图不会注销旧的 Target，因此优先在「当前打开的视图」
        /// （View.ActiveView）里查找，避免把按钮建到已隐藏的界面上。
        /// </summary>
        private static ItemDetailsDisplay FindDisplay(Item item)
        {
            View activeView = View.ActiveView;
            if (activeView != null)
            {
                ItemDetailsDisplay[] ownDisplays = activeView.GetComponentsInChildren<ItemDetailsDisplay>(true);
                foreach (ItemDetailsDisplay display in ownDisplays)
                {
                    if (display != null && display.Target == item && display.gameObject.activeInHierarchy)
                    {
                        return display;
                    }
                }
            }

            // 兜底：活动视图之外（例如浮动详情面板）里已经激活的详情面板。
            // 不再返回未激活的面板：关闭的视图会残留旧的 Target，
            // 把按钮建在隐藏面板上用户永远看不到。
            ItemDetailsDisplay[] displays = Object.FindObjectsOfType<ItemDetailsDisplay>(true);
            foreach (ItemDetailsDisplay display in displays)
            {
                if (display == null || display.Target != item || !display.gameObject.activeInHierarchy)
                {
                    continue;
                }
                return display;
            }
            return null;
        }

        private GameObject CreateRow(ItemDetailsDisplay display)
        {
            Transform layoutRoot = FindLayoutRoot(display);
            if (layoutRoot == null)
            {
                Diag.Log($"[创建失败] 未找到详情面板的垂直布局：面板「{display.name}」的子节点={DescribeHierarchy(display.transform)}");
                Debug.LogWarning((object)"[VanillaStorageExtend] 未找到详情面板的垂直布局，无法放置关闭按钮");
                return null;
            }
            Diag.Log($"创建按钮行：布局根「{layoutRoot.name}」，现有子节点数={layoutRoot.childCount}");

            // 行容器：由 VerticalLayoutGroup 负责拉伸到整行宽度，并排在内容最下方
            GameObject row = new GameObject(RowName, typeof(RectTransform), typeof(LayoutElement));
            row.SetActive(false);
            RectTransform rowRect = (RectTransform)row.transform;
            rowRect.SetParent(layoutRoot, false);
            rowRect.localScale = Vector3.one;
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = Vector2.zero;
            rowRect.sizeDelta = new Vector2(0f, RowHeight);

            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.minWidth = -1f;
            rowLayout.preferredWidth = -1f;
            rowLayout.flexibleWidth = 1f;
            rowLayout.minHeight = RowHeight;
            rowLayout.preferredHeight = RowHeight;
            rowLayout.flexibleHeight = 0f;
            row.transform.SetAsLastSibling();

            // 左右两个按钮各占整行一半，中间留出 ButtonGap 的间距。
            // 拉伸锚点配合负数 sizeDelta 得到固定高度，按下时由 SizeDeltaToggle 播放缩放。
            CreateActionButton(
                display,
                rowRect,
                MoveAllButtonName,
                MoveAllButtonLabel,
                new Vector2(0f, 0f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, VerticalMargin),
                new Vector2(-ButtonGap * 0.5f, -VerticalMargin),
                OnMoveAllClicked);

            CreateActionButton(
                display,
                rowRect,
                CloseButtonName,
                CloseButtonLabel,
                new Vector2(0.5f, 0f),
                new Vector2(1f, 1f),
                new Vector2(ButtonGap * 0.5f, VerticalMargin),
                new Vector2(0f, -VerticalMargin),
                OnCloseClicked);

            row.SetActive(true);
            MarkLayoutDirty(row);
            Diag.Log("按钮行创建成功：已添加「全部移出」与「点击关闭」两个按钮");
            Debug.Log((object)"[VanillaStorageExtend] 已为容器详情面板添加「全部移出」和「点击关闭」按钮");
            return row;
        }

        /// <summary>
        /// 创建一个与「点击关闭」颜色、字体、动画完全一致的绿色按钮。
        /// </summary>
        private void CreateActionButton(
            ItemDetailsDisplay display,
            RectTransform rowRect,
            string name,
            string label,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax,
            Action onClick)
        {
            GameObject buttonObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button));
            buttonObject.SetActive(false);

            RectTransform buttonRect = (RectTransform)buttonObject.transform;
            buttonRect.SetParent(rowRect, false);
            buttonRect.localScale = Vector3.one;
            buttonRect.anchorMin = anchorMin;
            buttonRect.anchorMax = anchorMax;
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.offsetMin = offsetMin;
            buttonRect.offsetMax = offsetMax;

            Image image = buttonObject.GetComponent<Image>();
            image.color = ButtonGreen;
            image.raycastTarget = true;

            // 仿照原版「分割」按钮：圆角九宫格底图，Sliced 拉伸
            Sprite background = GetButtonBackground();
            if (background != null)
            {
                image.sprite = background;
                image.type = Image.Type.Sliced;
                image.pixelsPerUnitMultiplier = 1f;
                image.preserveAspect = false;
                image.fillCenter = true;
            }

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            // 颜色过渡参数与原版「分割」按钮完全一致
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f, 1f);
            colors.pressedColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 1f);
            colors.selectedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f, 1f);
            colors.disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5019608f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            button.onClick.AddListener(() => onClick());

            CreateLabel(display, buttonRect, label);
            ConfigurePressAnimation(buttonObject, buttonRect);
            AddOriginalShadow(buttonObject);

            currentButtons.Add(new ButtonEntry
            {
                Rect = buttonRect,
                IdleSize = buttonRect.sizeDelta
            });
            buttonObject.SetActive(true);
        }

        private static Transform FindLayoutRoot(ItemDetailsDisplay display)
        {
            // ItemDisplayCore 是详情面板里的垂直布局根节点，
            // 槽位显示组件就挂在它的子节点 ItemSlotsDisplay 上。
            ItemSlotCollectionDisplay slotDisplay = display.SlotCollectionDisplay;
            if (slotDisplay != null)
            {
                Transform parent = slotDisplay.transform.parent;
                if (parent != null)
                {
                    return parent;
                }
            }

            // 兜底：按预制体中的固定层级查找
            Transform panel = display.transform.Find("Panel");
            if (panel != null)
            {
                Transform core = panel.Find("ItemDisplayCore");
                if (core != null)
                {
                    return core;
                }
            }
            return null;
        }

        /// <summary>
        /// 加载原版「分割」按钮的圆角九宫格底图。
        /// 失败时返回 null，按钮退化为原来的纯色方块，不影响功能。
        /// </summary>
        private Sprite GetButtonBackground()
        {
            if (buttonBackground != null)
            {
                return buttonBackground;
            }

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(BackgroundResourceName))
                {
                    if (stream == null)
                    {
                        Debug.LogWarning((object)("[VanillaStorageExtend] 找不到按钮底图资源：" + BackgroundResourceName));
                        return null;
                    }

                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (!ImageConversion.LoadImage(texture, data))
                    {
                        Debug.LogWarning((object)"[VanillaStorageExtend] 按钮底图不是有效的 PNG");
                        Object.Destroy(texture);
                        return null;
                    }

                    texture.filterMode = FilterMode.Bilinear;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.Apply();

                    // 四边 17 像素的边框与原版 Sprite「Rect16」一致，拉伸时角保持圆滑
                    buttonBackground = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        BackgroundPixelsPerUnit,
                        0u,
                        SpriteMeshType.FullRect,
                        new Vector4(BackgroundBorder, BackgroundBorder, BackgroundBorder, BackgroundBorder));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning((object)("[VanillaStorageExtend] 加载按钮底图失败：" + e));
            }

            return buttonBackground;
        }

        private void DestroyBackground()
        {
            if (buttonBackground == null)
            {
                return;
            }

            Texture2D texture = buttonBackground.texture as Texture2D;
            Object.Destroy(buttonBackground);
            if (texture != null)
            {
                Object.Destroy(texture);
            }
            buttonBackground = null;
        }

        private static void CreateLabel(ItemDetailsDisplay display, RectTransform buttonRect, string label)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform));
            RectTransform textRect = (RectTransform)textObject.transform;
            textRect.SetParent(buttonRect, false);
            textRect.localScale = Vector3.one;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 0f);
            textRect.offsetMax = new Vector2(-8f, 0f);

            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();

            // 沿用面板里的字体，避免出现豆腐块或字体风格不一致
            TMP_Text template = display.GetComponentInChildren<TMP_Text>(true);
            if (template != null && template.font != null)
            {
                text.font = template.font;
            }
            else if (TMP_Settings.defaultFontAsset != null)
            {
                text.font = TMP_Settings.defaultFontAsset;
            }

            text.text = label;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            // 字号与原版「分割」按钮一致（24），不使用自动缩放
            text.enableAutoSizing = false;
            text.fontSize = 24f;
            text.fontStyle = FontStyles.Normal;
            text.margin = Vector4.zero;
            text.raycastTarget = false;
        }

        /// <summary>
        /// 给按钮加上原版「分割」按钮上的 TrueShadow（LeTai.TrueShadow），参数完全一致。
        /// 用反射而不是编译期引用：即使游戏未附带该库，也只跳过阴影，不会让按钮类加载失败。
        /// </summary>
        private static void AddOriginalShadow(GameObject buttonObject)
        {
            try
            {
                Type shadowType = Type.GetType("LeTai.TrueShadow.TrueShadow, LeTai.TrueShadow");
                if (shadowType == null)
                {
                    return;
                }

                Component shadow = buttonObject.AddComponent(shadowType);
                SetShadowProperty(shadow, "Size", 32f);
                SetShadowProperty(shadow, "Spread", 0f);
                SetShadowProperty(shadow, "OffsetAngle", 90f);
                SetShadowProperty(shadow, "OffsetDistance", 12f);
                SetShadowProperty(shadow, "Color", new Color(0f, 0f, 0f, 0.6f));
                SetShadowProperty(shadow, "Inset", false);
                SetShadowProperty(shadow, "UseCasterAlpha", true);
                SetShadowProperty(shadow, "IgnoreCasterColor", false);
                SetShadowProperty(shadow, "DisableFitCompensation", false);

                PropertyInfo bleedMode = shadowType.GetProperty(
                    "ColorBleedMode",
                    BindingFlags.Public | BindingFlags.Instance);
                if (bleedMode != null && bleedMode.CanWrite && bleedMode.PropertyType.IsEnum)
                {
                    // 原版序列化值为 2（Black）
                    bleedMode.SetValue(shadow, Enum.ToObject(bleedMode.PropertyType, 2), null);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning((object)("[VanillaStorageExtend] 添加按钮阴影失败：" + e));
            }
        }

        private static void SetShadowProperty(object shadow, string name, object value)
        {
            PropertyInfo property = shadow.GetType().GetProperty(
                name,
                BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(shadow, value, null);
            }
        }

        /// <summary>
        /// 复刻原版「分割」按钮的按压缩放动画：
        /// 尺寸在 0.1 秒内变化，动画曲线带一次过冲回弹。
        /// 曲线关键点直接取自游戏内 SizeDeltaToggle 的序列化数据。
        /// </summary>
        private static void ConfigurePressAnimation(GameObject buttonObject, RectTransform buttonRect)
        {
            SizeDeltaToggle toggle = buttonObject.AddComponent<SizeDeltaToggle>();
            Vector2 idle = buttonRect.sizeDelta;
            toggle.idleSizeDelta = idle;
            toggle.activeSizeDelta = idle + PressDelta;
            toggle.duration = 0.1f;
            toggle.animationCurve = CreatePressCurve();

            PressToggleBridge bridge = buttonObject.AddComponent<PressToggleBridge>();
            bridge.Configure(toggle);
        }

        private static AnimationCurve CreatePressCurve()
        {
            AnimationCurve curve = new AnimationCurve(
                new Keyframe(0f, 0f, 2f, 2f),
                new Keyframe(0.6662211f, 1.527896f, 0f, 0f, 1f / 3f, 1f / 3f),
                new Keyframe(1f, 1f, 0f, 0f));
            curve.preWrapMode = WrapMode.Clamp;
            curve.postWrapMode = WrapMode.Clamp;
            return curve;
        }

        private static void MarkLayoutDirty(GameObject row)
        {
            if (row == null)
            {
                return;
            }

            // 行所在的垂直布局和上层带 ContentSizeFitter 的面板都要重算高度
            Transform parent = row.transform.parent;
            int guard = 0;
            while (parent != null && guard++ < 4)
            {
                RectTransform rect = parent as RectTransform;
                if (rect != null)
                {
                    LayoutRebuilder.MarkLayoutForRebuild(rect);
                }
                parent = parent.parent;
            }
        }

        private void DestroyRow()
        {
            if (currentRow != null)
            {
                currentRow.SetActive(false);
                Object.Destroy(currentRow);
            }
            currentRow = null;
            currentButtons.Clear();
        }

        private static void OnCloseClicked()
        {
            try
            {
                Diag.Log("点击关闭：取消选中");
                // 与再次点击物品时的处理完全一致：取消选中，详情面板淡出
                ItemUIUtilities.Select(null);
            }
            catch (Exception e)
            {
                Debug.LogWarning((object)("[VanillaStorageExtend] 关闭容器详情失败：" + e));
            }
        }

        /// <summary>
        /// 「全部移出」：把当前容器里能搬的物品按背包空闲格数尽量转移到角色主背包。
        /// 只有容器里的物品全部转移成功才自动关闭详情面板。
        /// </summary>
        private static void OnMoveAllClicked()
        {
            try
            {
                Item container = ItemUIUtilities.SelectedItem;
                Diag.Log("点击全部移出：" + Describe(container));
                if (!IsSupportedContainer(container))
                {
                    return;
                }

                bool movedAll = TryMoveAllToBackpack(container);
                Diag.Log("全部移出结果：" + (movedAll ? "已全部移出" : "未全部移出"));
                if (movedAll)
                {
                    // 全部移出后走与「点击关闭」相同的取消选中流程
                    ItemUIUtilities.Select(null);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning((object)("[VanillaStorageExtend] 全部移出失败：" + e));
            }
        }

        /// <summary>
        /// 将容器槽位里的物品依次移入角色主背包。
        /// 背包空闲格数即本次最多可移出的物品数；全部成功返回 true。
        /// </summary>
        private static bool TryMoveAllToBackpack(Item container)
        {
            Inventory backpack = LevelManager.Instance?.MainCharacter?.CharacterItem?.Inventory;
            if (backpack == null)
            {
                Debug.LogWarning((object)"[VanillaStorageExtend] 全部移出：找不到角色背包");
                return false;
            }

            // 先做快照：移动过程中容器槽位会发生变化，不能一边遍历一边修改
            List<Item> items = new List<Item>();
            SlotCollection slots = container.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                Slot slot = slots[i];
                if (slot != null && slot.Content != null)
                {
                    items.Add(slot.Content);
                }
            }

            if (items.Count == 0)
            {
                return true;
            }

            int freeSlots = CountFreeSlots(backpack);
            int moved = 0;

            foreach (Item item in items)
            {
                // 有多少个空闲格就最多移出多少件
                if (moved >= freeSlots)
                {
                    break;
                }

                Slot originalSlot = item.PluggedIntoSlot;
                item.Detach();

                if (backpack.AddAndMerge(item))
                {
                    moved++;
                    continue;
                }

                // 理论上不会发生；万一失败，把物品放回原槽位，避免悬空丢失
                if (originalSlot != null && originalSlot.Content == null && originalSlot.CanPlug(item))
                {
                    Item unpluggedItem;
                    originalSlot.Plug(item, out unpluggedItem);
                }
                Debug.LogWarning((object)("[VanillaStorageExtend] 全部移出：物品放入背包失败，已中止：" + item.DisplayName));
                break;
            }

            Diag.Log($"全部移出：容器「{container.DisplayName}」共 {items.Count} 件，背包空闲 {freeSlots} 格，成功移出 {moved} 件");
            Debug.Log((object)(
                $"[VanillaStorageExtend] 全部移出：容器「{container.DisplayName}」共 {items.Count} 件，背包空闲 {freeSlots} 格，成功移出 {moved} 件"));
            return moved == items.Count;
        }

        /// <summary>统计背包中还能放入物品的格子数量。</summary>
        private static int CountFreeSlots(Inventory inventory)
        {
            if (inventory == null)
            {
                return 0;
            }

            int free = 0;
            int capacity = inventory.Capacity;
            for (int i = 0; i < capacity; i++)
            {
                if (inventory.GetItemAt(i) == null)
                {
                    free++;
                }
            }
            return free;
        }
    }

    /// <summary>把指针按下/抬起转发给 SizeDeltaToggle，驱动按压缩放动画。</summary>
    internal sealed class PressToggleBridge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private SizeDeltaToggle toggle;

        public void Configure(SizeDeltaToggle value)
        {
            toggle = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetToggle(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetToggle(false);
        }

        /// <summary>按住后把指针移出按钮时也要复位，避免卡在按下状态。</summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            SetToggle(false);
        }

        private void SetToggle(bool value)
        {
            try
            {
                if (toggle != null)
                {
                    toggle.SetToggle(value);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning((object)("[VanillaStorageExtend] 播放按钮按下动画失败：" + e));
            }
        }
    }
}
