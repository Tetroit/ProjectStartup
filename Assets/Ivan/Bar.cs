using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

namespace GameUI
{
    public class Bar : MonoBehaviour
    {
        [SerializeField]
        RectTransform box;
        [SerializeField]
        TextMeshProUGUI text;

        public float max { get; private set; }
        public float value { get; private set; }
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetValue(float value, float max)
        {
            this.value = value;
            this.max = max;
            Refresh();
        }
        public void SetValue(float value) { SetValue(value, max); }
        public void SetValue(int value, int max) { SetValue((float)value, max); }
        public void SetValue(int value) { SetValue((float)value); }

        public void SetMax(float newMax, bool changeValue = false)
        {
            max = newMax;
            if (changeValue)
                value = newMax;
            Refresh();
        }
        public void SetMax(int newMax, bool changeValue = false) { SetMax((float)newMax, changeValue); }
        public void SetMax(int newMax) { SetMax(newMax, false); }
        public void Refresh()
        {
            if (max == 0)
                throw new Exception("Division by 0");
            box.anchorMax = new Vector2(value / max, 1f);
            if (text != null)
                text.text = "" + (int)value + "/" + (int)max;
        }
    }
}
