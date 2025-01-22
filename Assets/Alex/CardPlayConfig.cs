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
        public bool destroyOnPlay;

    }
}