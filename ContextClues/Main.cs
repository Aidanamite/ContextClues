using HarmonyLib;
using SRML;
using SRML.Console;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using static AssetsLib.TextureUtils;
using static AssetsLib.GameObjectUtils;

namespace ContextClues
{
    public class Main : ModEntryPoint
    {
        internal static Assembly modAssembly = Assembly.GetExecutingAssembly();
        internal static string modName = $"{modAssembly.GetName().Name}";
        internal static string modDir = $"{System.Environment.CurrentDirectory}\\SRML\\Mods\\{modName}";
        public static Main instance;

        public Main() => instance = this;

        public static Transform container;
        public static ContextClue itemPrefab;
        public static Sprite[] background = new Sprite[9];
        public static Sprite[] background2 = new Sprite[9];
        public override void PreLoad()
        {
            var t = LoadImage("back.png");
            var t2 = LoadImage("back2.png");
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 3; x++)
                {
                    background[x + y * 3] = Sprite.Create(t, Rect.MinMaxRect(t.width * Math.Max(x * 0.5f - 0.25f, 0), t.height * Math.Max(y * 0.5f - 0.25f, 0), t.width * Math.Min(x * 0.5f + 0.25f, 1), t.height * Math.Min(y * 0.5f + 0.25f, 1)), new Vector2(0.5f, 0.5f));
                    background2[x + y * 3] = Sprite.Create(t2, Rect.MinMaxRect(t2.width * Math.Max(x * 0.5f - 0.25f, 0), t2.height * Math.Max(y * 0.5f - 0.25f, 0), t2.width * Math.Min(x * 0.5f + 0.25f, 1), t2.height * Math.Min(y * 0.5f + 0.25f, 1)), new Vector2(0.5f, 0.5f));
                }
            SRML.SR.SRCallbacks.OnActorSpawn += (x, y, z) =>
            {
                if (x == Identifiable.Id.PLAYER && !container)
                {
                    var anchor = new GameObject("ContextCluesAnchor", typeof(RectTransform)).GetComponent<RectTransform>();
                    anchor.SetParent(HudUI.Instance.uiContainer.transform, false);
                    anchor.anchorMin = anchor.anchorMax = new Vector2(0, 0.5f);
                    anchor.offsetMin = anchor.offsetMax = Vector2.zero;
                    var box = new GameObject("OuterBox", typeof(RectTransform), typeof(SmoothResize), typeof(Image)).GetComponent<SmoothResize>();
                    box.Self.SetParent(anchor, false);
                    box.GetComponent<Image>().sprite = background[4];
                    for (int h = 0; h < 3; h++)
                        for (int w = 0; w < 3; w++)
                            if (h != 1 || w != 1)
                            {
                                var border = new GameObject("Border", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                                border.transform.SetParent(box.transform, false);
                                border.sprite = background[w + h * 3];
                                border.rectTransform.anchorMin = new Vector2(w > 1 ? 1 : 0, h > 1 ? 1 : 0);
                                border.rectTransform.anchorMax = new Vector2(w > 0 ? 1 : 0, h > 0 ? 1 : 0);
                                border.rectTransform.offsetMin = new Vector2(w == 0 ? -20 : 0, h == 0 ? -20 : 0);
                                border.rectTransform.offsetMax = new Vector2(w == 2 ? 20 : 0, h == 2 ? 20 : 0);
                            }
                    var cparent = new GameObject("ContentParent", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
                    cparent.SetParent(box.transform,false);
                    cparent.anchorMin = cparent.offsetMin = cparent.offsetMax = Vector2.zero;
                    cparent.anchorMax = Vector2.one;
                    container = box.Follow = new GameObject("ContentSize", typeof(RectTransform), typeof(ContentSizeFitter), typeof(VerticalLayoutGroup)).GetComponent<RectTransform>();
                    container.SetParent(cparent, false);
                    box.Follow.anchorMin = box.Follow.anchorMax = box.Follow.pivot = new Vector2(0, 1);
                    var content = container.gameObject.AddComponent<ContentSizeFitter>();
                    content.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    content.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    var layout = container.GetComponent<VerticalLayoutGroup>();
                    layout.spacing = 25;
                    layout.childControlHeight = false;
                    layout.childControlWidth = false;
                    layout.padding = new RectOffset(20,10,10,10);
                    layout.childAlignment = TextAnchor.UpperLeft;
                    if (!itemPrefab && SRSingleton<HudUI>.Instance)
                    {
                        itemPrefab = CreateEmptyPrefab("ContextClue").AddComponent<ContextClue>();
                        itemPrefab.gameObject.AddComponent<RectTransform>();
                        for (int h = 0; h < 3; h++)
                            for (int w = 0; w < 3; w++)
                                if (h != 1 || w != 1)
                                {
                                    var border = new GameObject("Border", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                                    border.transform.SetParent(itemPrefab.transform, false);
                                    border.sprite = background2[w + h * 3];
                                    border.rectTransform.anchorMin = new Vector2(w > 1 ? 1 : 0, h > 1 ? 1 : 0);
                                    border.rectTransform.anchorMax = new Vector2(w > 0 ? 1 : 0, h > 0 ? 1 : 0);
                                    border.rectTransform.offsetMin = new Vector2(w == 0 ? -10 : 0, h == 0 ? -10 : 0);
                                    border.rectTransform.offsetMax = new Vector2(w == 2 ? 10 : -0, h == 2 ? 10 : -0);
                                }
                        itemPrefab.text = Object.Instantiate(SRSingleton<HudUI>.Instance.currencyText,itemPrefab.transform,false).GetComponent<TMP_Text>();
                        itemPrefab.text.name = "Text";
                        itemPrefab.width = 300;
                        itemPrefab.text.rectTransform.anchorMin = itemPrefab.text.rectTransform.offsetMin = itemPrefab.text.rectTransform.offsetMax = Vector2.zero;
                        itemPrefab.text.rectTransform.anchorMax = Vector2.one;
                        var styler = itemPrefab.text.gameObject.GetComponent<MeshTextStyler>();
                        itemPrefab.text.SetField("m_rectTransform",itemPrefab.text.GetComponent<RectTransform>());
                        styler.OnEnable();
                        Object.Destroy(styler);
                        itemPrefab.text.lineSpacing = 1;
                        itemPrefab.text.autoSizeTextContainer = true;
                        itemPrefab.text.enableWordWrapping = true;
                        itemPrefab.text.fontSize /= 2;
                        itemPrefab.text.fontSizeMin /= 2;
                        itemPrefab.text.fontSizeMax /= 2;
                        itemPrefab.gameObject.AddComponent<Image>().sprite = background2[4];
                    }
                }
            };
        }
        public static void Log(string message) => instance.ConsoleInstance.Log($"[{modName}]: " + message);
        public static void LogError(string message) => instance.ConsoleInstance.LogError($"[{modName}]: " + message);
        public static void LogWarning(string message) => instance.ConsoleInstance.LogWarning($"[{modName}]: " + message);
        public static void LogSuccess(string message) => instance.ConsoleInstance.LogSuccess($"[{modName}]: " + message);

        GameObject clue;
        GameObject clue2;
        public override void Update()
        {
            if (SRSingleton<SceneContext>.Instance && SRSingleton<SceneContext>.Instance.PlayerState && SRSingleton<SceneContext>.Instance.PlayerState.Targeting && SRSingleton<SceneContext>.Instance.PlayerState.Targeting.GetComponent<Identifiable>())
            {
                if (!clue)
                    clue = ContextClue.Create("Identifiable: " + SRSingleton<SceneContext>.Instance.PlayerState.Targeting.name);
            }
            else if (clue)
                Object.Destroy(clue);
            if (SRSingleton<SceneContext>.Instance && SRSingleton<SceneContext>.Instance.PlayerState && SRSingleton<SceneContext>.Instance.PlayerState.Targeting && SRSingleton<SceneContext>.Instance.PlayerState.Targeting.GetComponent<Rigidbody>())
            {
                if (!clue2)
                    clue2 = ContextClue.Create("Physics: " + SRSingleton<SceneContext>.Instance.PlayerState.Targeting.name);
            }
            else if (clue2)
                Object.Destroy(clue2);
        }
    }
}