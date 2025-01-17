using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Equation;
using UnityEngine.UI;
using Unity.VisualScripting;


public class Chip : MonoBehaviour
{
    bool hovered = false;

    ChipManager chipManager;
    Collider hitBox;
    public EquationElement element { get; protected set; }
    private void OnEnable()
    {
        chipManager = ChipManager.instance;
        chipManager.AddChip(this);
    }
    private void OnDisable()
    {
        chipManager.RemoveChip(this);
    }
    void Start()
    {
        hitBox = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
    }   

    void OnClick()
    {
        chipManager.inventoryLayout.RemoveItem(gameObject);
        chipManager.selected = gameObject;

    }

    private void OnMouseDown()
    {
        RaycastHit hit;
        if (hitBox.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
        {
            OnClick();
        }
    }
}
