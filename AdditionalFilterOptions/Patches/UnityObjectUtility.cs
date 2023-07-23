using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalFilterOptions.Patches
{
    internal class UnityObjectUtility
    {
        public static GameObject CreateImage(string name, Color32 color, Rect rect, Transform parent)
        {
            GameObject newImageObj = new GameObject(name);
            newImageObj.transform.SetParent(parent);
            var image = newImageObj.AddComponent<Image>();
            image.color = new Color32(color.r, color.g, color.b, color.a);

            var rectTransform = newImageObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.localScale = Vector3.one;

            var position = newImageObj.transform.localPosition;
            //position.x = location.x;
            //position.y = location.y;
            position.z = 0;
            newImageObj.transform.localPosition = position;

            return newImageObj;
        }

        public delegate void ButtonPress();
        public static GameObject CreateCycleButton(string name, List<string> items, List<string> data, Rect rect, Transform parent)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent);
            var cycleButton = newObject.AddComponent<CycleButton>();
            cycleButton.InitializeItemList(items, data);


            var prevRect = rect;
            prevRect.width = rect.height;
            var buttonPrev = CreateButton("PrevButton", "<", cycleButton.PrevButtonPress, prevRect, newObject.transform);

            var labelRect = rect;
            labelRect.x = rect.x + rect.height;
            labelRect.width = rect.width - (rect.height * 2);
            var label = CreateCycleLabel(items[0], labelRect, newObject.transform);
            label.transform.SetParent(newObject.transform);

            var nextRect = rect;
            nextRect.x = rect.x + rect.width - rect.height;
            nextRect.width = rect.height;
            var buttonNext = CreateButton("NextButton", ">", cycleButton.NextButtonPress, nextRect, newObject.transform);



            var rectTransform = newObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = newObject.AddComponent<RectTransform>();
            }
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.localScale = Vector3.one;

            return newObject;
        }

        private static GameObject CreateButton(string name, string text, ButtonPress buttonPress, Rect rect, Transform parent)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);
            var button = newObject.AddComponent<Button>();
            button.onClick.AddListener(delegate { buttonPress(); });

            var image = newObject.AddComponent<Image>();
            image.color = new Color32(255, 255, 255, 255);

            var raycaster = newObject.AddComponent<GraphicRaycaster>();

            GameObject textObject = new GameObject("text");
            textObject.transform.SetParent(newObject.transform);

            var textComp = textObject.AddComponent<TextMeshProUGUI>();
            textComp.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textComp.verticalAlignment = VerticalAlignmentOptions.Middle;
            textComp.text = text;
            textComp.enableAutoSizing = true;
            textComp.fontSizeMax = 1000;
            textComp.color = new Color(0, 0, 0, 255);

            var rectTransform = newObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = newObject.AddComponent<RectTransform>();
            }
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.localScale = Vector3.one;

            return newObject;
        }

        private static GameObject CreateCycleLabel(string text, Rect rect, Transform parent)
        {
            GameObject newObject = new GameObject("Label");
            newObject.transform.SetParent(parent);
            var textComp = newObject.AddComponent<TextMeshProUGUI>();
            textComp.text = text;
            textComp.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textComp.verticalAlignment = VerticalAlignmentOptions.Middle;
            textComp.enableAutoSizing = true;
            textComp.fontSizeMax = 1000;

            var rectTransform = newObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = newObject.AddComponent<RectTransform>();
            }
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.localScale = Vector3.one;

            return newObject;
        }


        public static GameObject CreateDropdown(string name, List<string> items, Rect rect, Transform parent)
        {
            // Create the base Object data
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent);
            var dropdown = newObject.AddComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(items);

            var rectTransform = newObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.localScale = Vector3.one;

            var image = dropdown.gameObject.GetComponent<Image>();
            if (image == null)
            {
                image = dropdown.gameObject.AddComponent<Image>();
            }

            


            // Create Label Child
            var label = CreateDropDownLabel();
            label.transform.SetParent(newObject.transform);



            // Create Arrow Child
            var arrow = CreateDropDownArrow();
            arrow.transform.SetParent(newObject.transform);

            // Create Template Child


            return newObject;
        }

        private static GameObject CreateDropDownLabel()
        {
            GameObject newObject = new GameObject("Label");
            var textLabel = newObject.AddComponent<TextMeshProUGUI>();
            textLabel.text = "Label";

            var canvasRenderer = newObject.AddComponent<CanvasRenderer>();

            var rectTransform = newObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(1000, 100);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.localScale = Vector3.one;

            return newObject;
        }

        private static GameObject CreateDropDownArrow()
        {
            GameObject newObject = new GameObject("Arrow");
            var image = newObject.AddComponent<Image>();

            return newObject;
        }

        private static GameObject CreateDropDownTemplate()
        {
            GameObject newObject = new GameObject("Template");
            var textLabel = newObject.AddComponent<TextMeshProUGUI>();
            textLabel.text = "Label";

            var canvasRenderer = newObject.AddComponent<CanvasRenderer>();

            return newObject;
        }

        public static GameObject GetChildWithName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
}
