using System;
using UnityEngine;

namespace ContextClues
{
    public class SmoothResize : MonoBehaviour
    {
        public RectTransform Follow;
        RectTransform self;
        public RectTransform Self
        {
            get
            {
                if (!self)
                    self = GetComponent<RectTransform>();
                return self;
            }
        }
        Vector2 startTransition;
        Vector2 lastKnown;
        float time = 0;
        void Update()
        {
            if (!Follow)
                return;
            if (lastKnown != Follow.rect.size)
            {
                lastKnown = Follow.rect.size;
                time = 0;
                startTransition = Self.rect.size;
            }
            if (lastKnown != startTransition)
            {
                time += Time.deltaTime * 3;
                if (time > 1)
                    time = 1;
                var cur = Vector2.Lerp(startTransition, lastKnown, (float)Math.Sqrt(time));
                if (time == 1)
                    startTransition = lastKnown;
                Self.offsetMax = new Vector2(cur.x, cur.y / 2);
                Self.offsetMin = new Vector2(0, -cur.y / 2);
            }
        }
    }
}