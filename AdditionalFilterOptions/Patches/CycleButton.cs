using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AdditionalFilterOptions.Patches
{
    internal class CycleButton : MonoBehaviour
    {
        // items is what will be visible to the user
        List<string> items = new List<string>();

        // data will be any underlying data that may be needed
        List<string> data = new List<string>();


        int currentIndex;

        bool hasChanged = false;

        void Start()
        {
            currentIndex = 0;
        }

        public void InitializeItemList(List<string> newItems, List<string> newData)
        {
            // This function probably doesn't need to be this complicated
            // But I like things being very explicit
            if (items != null)
            {
                items.Clear();
            }
            items = new List<string>();
            for (int i = 0; i < newItems.Count; i++)
            {
                items.Add(newItems[i]);
            }

            if (data != null)
            {
                data.Clear();
            }
            data = new List<string>();
            for (int i = 0; i < newData.Count; i++)
            {
                data.Add(newData[i]);
            }
        }

        public bool HasChanged()
        {
            var prevHasChanged = hasChanged;
            hasChanged = false;
            return prevHasChanged;
        }

        public string GetCurrentData()
        {
            return data[currentIndex];
        }

        public void PrevButtonPress()
        {
            currentIndex = (currentIndex - 1 + items.Count) % items.Count;
            UpdateLabelText();
            hasChanged = true;
        }

        public void NextButtonPress()
        {
            currentIndex = (currentIndex + 1) % items.Count;
            UpdateLabelText();
            hasChanged = true;
        }

        private void UpdateLabelText()
        {

            GameObject labelObject = UnityObjectUtility.GetChildWithName(gameObject, "Label");
            var textComp = labelObject.GetComponent<TextMeshProUGUI>();
            textComp.text = items[currentIndex];
        }
    }
}
