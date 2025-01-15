using System;
using events;
using UnityEngine;
using UnityEngine.Events;

namespace config
{
    [Serializable]
    public class EventsConfig
    {
        [SerializeField]
        public UnityEvent<CardPlayed> OnCardPlayed;

        [SerializeField]
        public UnityEvent<CardClick> OnCardHover;

        [SerializeField]
        public UnityEvent<CardRelease> OnCardUnhover;

        [SerializeField]
        public UnityEvent<CardDestroy> OnCardDestroy;
    }
}