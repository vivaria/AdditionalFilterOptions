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
    public class UICreation
    {
        static public GameObject CreateButton(string name, Rect rect, GameObject parent)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);

            var image = AddImageComponent(newObject, new Color32(255, 255, 255, 255));

            var button = newObject.AddComponent<Button>();

            var rectTransform = SetRect(rect, newObject);

            var text = AddText("Text", "Button", TextAlignmentOptions.Center, newObject);

            return newObject;
        }

        static public GameObject CreateInputField(string name, Rect rect, GameObject parent)
        {
            GameObject newObject = new GameObject(name);
            newObject.transform.SetParent(parent.transform);

            var rectTransform = SetRect(rect, newObject);

            var image = AddImageComponent(newObject, new Color32(255, 255, 255, 255));
            var inputField = newObject.AddComponent<TMP_InputField>();

            var textArea = new GameObject("Text Area");
            textArea.transform.SetParent(newObject.transform);
            SetRect(new Rect(0, 0, rect.width, rect.height), textArea);
            var rectMask = textArea.AddComponent<RectMask2D>();

            var text = AddText("Text", "", TextAlignmentOptions.Left, rectMask.gameObject);
            var placeholder = AddText("Placeholder", "Enter text...", TextAlignmentOptions.Left, rectMask.gameObject);
            var placeHolderText = placeholder.GetComponent<TextMeshProUGUI>();
            placeHolderText.color = new Color(placeHolderText.color.r, placeHolderText.color.g, placeHolderText.color.b, 0.5f);

            inputField.textViewport = textArea.GetComponent<RectTransform>();
            inputField.textComponent = text.GetComponent<TextMeshProUGUI>();
            inputField.placeholder = placeholder.GetComponent<TextMeshProUGUI>();

            return newObject;
        }

        static public GameObject CreateSlider(string name, int min, int max, Rect rect, GameObject parent)
        {
            var newObject = CreateSlider(name, rect, parent);
            var slider = newObject.GetComponent<Slider>();

            slider.wholeNumbers = true;

            slider.minValue = min;
            slider.maxValue = max;

            return newObject;
        }

        static public GameObject CreateSlider(string name, float min, float max, Rect rect, GameObject parent)
        {
            var newObject = CreateSlider(name, rect, parent);
            var slider = newObject.GetComponent<Slider>();

            slider.wholeNumbers = false;

            slider.minValue = min;
            slider.maxValue = max;

            return newObject;
        }

        static private GameObject CreateSlider(string name, Rect rect, GameObject parent)
        {
            // Create the object
            GameObject newObject = CreateGameObject(name, rect, parent);

            // Add a Slider component to it
            var slider = newObject.AddComponent<Slider>();

            // Create background child
            GameObject backgroundObject = CreateGameObject("Background", new Rect(0, 0, 0, 0), newObject);
            var backgroundRect = backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);

            // Add Image component to background child
            var bgImage = AddImageComponent(backgroundObject, new Color32(214, 214, 214, 255));

            // Create Fill Area object
            GameObject fillAreaObject = CreateGameObject("Fill Area", new Rect(-5f, 0, -20, 0), newObject);
            var fillAreaRect = fillAreaObject.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.pivot = new Vector2(0.5f, 0.5f);

            // Create Fill object
            GameObject fillObject = CreateGameObject("Fill", new Rect(0, 0, 10, 0), fillAreaObject);
            var fillImage = AddImageComponent(fillObject, new Color32(255, 255, 255, 255));

            // Create Handle Slide Area object
            GameObject handleSlideAreaObject = CreateGameObject("Handle Slide Area", new Rect(0, 0, -20, 0), newObject);
            var handleSlideAreaRect = handleSlideAreaObject.GetComponent<RectTransform>();
            handleSlideAreaRect.anchorMin = new Vector2(0, 0);
            handleSlideAreaRect.anchorMax = new Vector2(1, 1);
            handleSlideAreaRect.pivot = new Vector2(0.5f, 0.5f);

            // Create Handle object
            GameObject handleObject = CreateGameObject("Handle", new Rect(0, 0, 20, 0), handleSlideAreaObject);
            var handleImage = AddImageComponent(handleObject, new Color32(255, 255, 255, 255));

            // Assign Slider variables
            slider.targetGraphic = handleImage;
            slider.fillRect = fillObject.GetComponent<RectTransform>();
            slider.handleRect = handleObject.GetComponent<RectTransform>();

            return newObject;
        }

        static public GameObject CreateDropdown(string name, List<string> options, Rect rect, GameObject parent)
        {
            GameObject dropdownObject = CreateGameObject(name, rect, parent);
            var dropdownImage = AddImageComponent(dropdownObject, new Color32(255, 255, 255, 255));
            var dropdownDropdown = dropdownObject.AddComponent<TMP_Dropdown>();

            // Create Label
            GameObject labelObject = CreateGameObject("Label", new Rect(-7.5f, -0.5f, -35, -13), dropdownObject);
            AdjustAnchors(labelObject, new Vector2(0, 0), new Vector2(1, 1));
            var labelText = AddTextComponent(labelObject, "Option A", TextAlignmentOptions.Left);
            labelText.enableAutoSizing = false;

            // Create Arrow
            GameObject arrowObject = CreateGameObject("Arrow", new Rect(-15, 0, 20, 20), dropdownObject);
            AdjustAnchors(arrowObject, new Vector2(1, 0.5f), new Vector2(1, 0.5f));
            var arrowImage = AddImageComponent(arrowObject, new Color32(20, 20, 20, 40));

            // Create Template
            GameObject templateObject = CreateGameObject("Template", new Rect(0, 2, 0, 150), dropdownObject);
            AdjustAnchors(templateObject, new Vector2(0, 0), new Vector2(1, 0));
            AdjustPivot(templateObject, new Vector2(0.5f, 1));
            templateObject.SetActive(false);
            var templateImage = AddImageComponent(templateObject, new Color32(255, 255, 255, 255));
            var templateScrollRect = templateObject.AddComponent<ScrollRect>();
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            // Create Viewport
            GameObject viewportObject = CreateGameObject("Viewport", new Rect(0, 0, -18, 0), templateObject);
            AdjustAnchors(viewportObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(viewportObject, new Vector2(0, 1));
            var viewportMask = viewportObject.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            var viewportImage = AddImageComponent(viewportObject, new Color32(255, 255, 255, 255));

            // Create Content
            GameObject contentObject = CreateGameObject("Content", new Rect(0, 0, 0, 28), viewportObject);
            AdjustAnchors(contentObject, new Vector2(0, 1), new Vector2(1, 1));
            AdjustPivot(contentObject, new Vector2(0.5f, 1));

            // Create Item
            GameObject itemObject = CreateGameObject("Item", new Rect(0, 0, 0, 20), contentObject);
            AdjustAnchors(itemObject, new Vector2(0, 0.5f), new Vector2(1, 0.5f));
            AdjustPivot(itemObject, new Vector2(0.5f, 0.5f));
            var itemToggle = itemObject.AddComponent<Toggle>();
            itemToggle.isOn = true;

            // Create Item Background
            GameObject itemBackgroundObject = CreateGameObject("Item Background", new Rect(0, 0, 0, 0), itemObject);
            AdjustAnchors(itemBackgroundObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(itemBackgroundObject, new Vector2(0.5f, 0.5f));
            var itemBackgroundImage = AddImageComponent(itemBackgroundObject, new Color32(255, 255, 255, 255));

            // Create Item Checkmark
            GameObject itemCheckmarkObject = CreateGameObject("Item Checkmark", new Rect(10, 0, 20, 20), itemObject);
            AdjustAnchors(itemCheckmarkObject, new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            AdjustPivot(itemCheckmarkObject, new Vector2(0.5f, 0.5f));
            var itemCheckmarkImage = AddImageComponent(itemCheckmarkObject, new Color32(20, 20, 20, 255));


            // Create Item Label
            GameObject itemLabelObject = CreateGameObject("Item Label", new Rect(5, -0.5f, -30, -3), itemObject);
            AdjustAnchors(itemLabelObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(itemLabelObject, new Vector2(0.5f, 0.5f));
            var itemLabelText = AddTextComponent(itemLabelObject, "Option A", TextAlignmentOptions.Left);
            itemLabelText.enableAutoSizing = true;


            // Create Scrollbar
            GameObject scrollbarObject = CreateGameObject("Scrollbar", new Rect(0, 0, 20, 0), templateObject);
            AdjustAnchors(scrollbarObject, new Vector2(1, 0), new Vector2(1, 1));
            AdjustPivot(scrollbarObject, new Vector2(1, 1));
            var scrollbarImage = AddImageComponent(scrollbarObject, new Color32(255, 255, 255, 255));
            var scrollbarScrollbar = scrollbarObject.AddComponent<Scrollbar>();
            scrollbarScrollbar.direction = Scrollbar.Direction.BottomToTop;

            // Create Sliding Area
            GameObject slidingAreaObject = CreateGameObject("Sliding Area", new Rect(0, 0, -20, -20), scrollbarObject);
            AdjustAnchors(slidingAreaObject, new Vector2(0, 0), new Vector2(1, 1));
            AdjustPivot(slidingAreaObject, new Vector2(0.5f, 0.5f));


            // Create Handle
            GameObject handleObject = CreateGameObject("Handle", new Rect(0, 0, 20, 20), slidingAreaObject);
            AdjustAnchors(handleObject, new Vector2(0, 0), new Vector2(1, 0.2f));
            AdjustPivot(handleObject, new Vector2(0.5f, 0.5f));
            var handleImage = AddImageComponent(handleObject, new Color32(255, 255, 255, 255));


            // Connect it all together
            dropdownDropdown.template = templateObject.GetComponent<RectTransform>();
            dropdownDropdown.captionText = labelText;
            dropdownDropdown.itemText = itemLabelText;

            templateScrollRect.content = contentObject.GetComponent<RectTransform>();
            templateScrollRect.viewport = viewportObject.GetComponent<RectTransform>();
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;

            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;

            scrollbarScrollbar.handleRect = handleObject.GetComponent<RectTransform>();


            // Fill Content
            dropdownDropdown.AddOptions(options);

            return dropdownObject;
        }

        static private void AdjustAnchors(GameObject gameObject, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        static private void AdjustPivot(GameObject gameObject, Vector2 pivot)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.pivot = pivot;
        }

        static public Image AddImageComponent(GameObject parent, Color32 color)
        {
            var image = parent.GetComponent<Image>();
            if (image != null)
            {
                return image;
            }
            image = parent.AddComponent<Image>();

            image.color = color;

            return image;
        }

        static public RectTransform SetRect(Rect rect, GameObject parent)
        {
            var rectTransform = parent.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = parent.AddComponent<RectTransform>();
            }

            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);

            return rectTransform;
        }

        static public TextMeshProUGUI AddTextComponent(GameObject gameObject, string text, TextAlignmentOptions align)
        {
            var textObject = gameObject.GetComponent<TextMeshProUGUI>();
            if (textObject == null)
            {
                textObject = gameObject.AddComponent<TextMeshProUGUI>();
            }
            textObject.text = text;
            textObject.alignment = align;
            textObject.enableAutoSizing = true;
            textObject.color = new Color32(0, 0, 0, 255);

            return textObject;
        }

        static public GameObject AddText(string name, string text, TextAlignmentOptions align, GameObject parent)
        {
            //GameObject newObject = new GameObject(name);
            //newObject.transform.SetParent(parent.transform);
            //SetRect(GetGameObjectRect(parent), newObject);

            var parentRect = GetGameObjectRect(parent);
            var newRect = new Rect(0, 0, parentRect.width, parentRect.height);
            GameObject newObject = CreateGameObject(name, newRect, parent);

            var textObject = newObject.GetComponent<TextMeshProUGUI>();
            if (textObject == null)
            {
                textObject = newObject.AddComponent<TextMeshProUGUI>();
            }
            textObject.text = text;
            textObject.alignment = align;
            textObject.enableAutoSizing = true;
            textObject.color = new Color32(0, 0, 0, 255);

            // You need to set the rectangle after the text is created.
            // I hate that
            SetRect(newRect, newObject);

            return newObject;
        }

        static public Rect GetGameObjectRect(GameObject parent)
        {
            var rectTransform = parent.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return new Rect(rectTransform.anchoredPosition, rectTransform.sizeDelta);
            }
            else
            {
                // Position will be available, but Size will not
                return new Rect(parent.transform.position.x, parent.transform.position.y, 100, 100);
            }
        }

        static public GameObject CreateGameObject(string name, Rect rect, GameObject parent)
        {
            return CreateGameObject(name, rect, parent.transform);
        }

        static public GameObject CreateGameObject(string name, Rect rect, Transform parent)
        {
            var newObject = new GameObject(name);
            newObject.transform.SetParent(parent);
            SetRect(rect, newObject);
            return newObject;
        }
    }
}
