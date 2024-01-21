using InControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ContextClues
{
    public class ContextClue : UnityEngine.EventSystems.UIBehaviour, ILayoutElement, ILayoutSelfController, ILayoutIgnorer
    {
        public static GameObject Create(Func<string> message)
        {
            if (!Main.container)
                return null;
            var n = Instantiate(Main.itemPrefab, Main.container);
            n.message = message;
            n.Refresh();
            return n.gameObject;
        }
        public static GameObject Create(string message)
        {
            if (!Main.container)
                return null;
            var n = Instantiate(Main.itemPrefab, Main.container);
            n.text.text = message;
            return n.gameObject;
        }
        public TMP_Text text;
        public Func<string> message;
        public static HashSet<ContextClue> created = new HashSet<ContextClue>();
        protected override void Awake()
        {
            base.Awake();
            created.Add(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            created.Remove(this);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        public void Refresh()
        {
            if (message != null)
                text.text = message();
        }
        public void CalculateLayoutInputHorizontal() { }
        public void CalculateLayoutInputVertical() { }
        public float minWidth => width;
        public float preferredWidth => width;
        public float flexibleWidth => 0;
        public float minHeight => 0;
        public float preferredHeight => _h;
        public float flexibleHeight => 10000;
        public int layoutPriority { get; set; } = 1;
        float _w;
        float _h;
        float height;
        public float width;
        void Update()
        {
            var dirty = _w != width || _h != height;
            if (dirty && isActiveAndEnabled)
            {
                SetLayoutHorizontal();
                SetLayoutVertical();
                _w = width;
                _h = height;
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
            }
        }
        public void SetLayoutHorizontal()=>(transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        public void SetLayoutVertical()=>(transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height = text.GetPreferredValues(text.text, width, 0).y);
        public bool ignoreLayout { get; set; } = false;

        static MethodInfo _GetKeyStringForGamepad = typeof(InputDirector).GetMethod("GetKeyStringForGamepad", ~BindingFlags.Default);
        static MethodInfo _GetKeyStringForMouseKeyboard = typeof(InputDirector).GetMethod("GetKeyStringForMouseKeyboard", ~BindingFlags.Default);
        public static string GetKeyString(PlayerAction action)
        {
            if (GameContext.Instance && GameContext.Instance.InputDirector)
            {
                if (InputDirector.UsingGamepad())
                    return (string)_GetKeyStringForGamepad.Invoke(GameContext.Instance.InputDirector, new object[] { action, action.Name });
                else
                    return (string)_GetKeyStringForMouseKeyboard.Invoke(GameContext.Instance.InputDirector, new object[] { action });
            }
            return null;
        }
    }
}