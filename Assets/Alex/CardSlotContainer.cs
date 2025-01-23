using System.Collections.Generic;
using UnityEngine;

public class CardSlotContainer : MonoBehaviour
{
    private List<CardSlot> slots = new List<CardSlot>();

    private void Start()
    {
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        slots.Clear();
        foreach(Transform child in transform)
        {
            CardSlot slot = child.GetComponent<CardSlot>();
            if(slot == null)
            {
                slot = child.gameObject.AddComponent<CardSlot>();
            }
            slots.Add(slot);
        }
    }
}
