using System;
using events;
using UnityEngine;
using UnityEngine.Events;

namespace config
{
    public class EventsConfig : MonoBehaviour
    {
        public event Action<CardPlayed> OnCardPlayed;

        public event Action<CardClick> OnCardClick;

        public event Action<CardRelease> OnCardRelease;

        public event Action<CardDestroy> OnCardDestroy;

        public void RaiseOnCardPlayed(CardPlayed cardPlayed)
        {
            OnCardPlayed?.Invoke(cardPlayed);
        }

        public void RaiseOnCardClick(CardClick cardClick)
        {
            OnCardClick?.Invoke(cardClick);
        }

        public void RaiseCardRelease(CardRelease cardRelease)
        {
            OnCardRelease?.Invoke(cardRelease);
        }

        public void RaiseCardDestroy(CardDestroy cardDestroy)
        {
            OnCardDestroy?.Invoke(cardDestroy);
        }
    }
}
