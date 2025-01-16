using System;
using System.Collections.Generic;
using UnityEngine;

namespace config
{
    [Serializable]
    public class CardPlayConfig
    {
        public List<RectTransform> playableSlots = new List<RectTransform>();

        [SerializeField]
        public RectTransform slot_1;

        [SerializeField]
        public RectTransform slot_2;

        [SerializeField]
        public RectTransform slot_3;

        [SerializeField]
        public RectTransform slot_4;

        [SerializeField]
        public RectTransform slot_5;

        [SerializeField]
        public bool destroyOnPlay;

        [SerializeField]
        public RectTransform previewArea;
    }
}