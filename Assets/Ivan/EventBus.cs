using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEvents
{
    /// <summary>
    /// Base for events
    /// </summary>
    public abstract class Event { }
    /// <summary>
    /// Event bus template
    /// </summary>
    public class EventBus<T> where T : Event
    {
        public static event Action<T> OnEvent;

        public static void Publish(T pEvent)
        {
            OnEvent?.Invoke(pEvent);
        }
    }

    public class ChipPickedEvent : Event
    {
        public Chip chip;
        public ChipPickedEvent(Chip chip) { this.chip = chip; }
    }
}
   
