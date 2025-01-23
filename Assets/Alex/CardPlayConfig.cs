using System;
using System.Collections.Generic;
using UnityEngine;

namespace config
{
    [Serializable]
    public class CardPlayConfig
    {
        public List<GameObject> playableSlots = new List<GameObject>();

        [SerializeField]
        public bool destroyOnPlay;

    }
}