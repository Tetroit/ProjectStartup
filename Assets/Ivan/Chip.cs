using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Equation;
using UnityEngine.UI;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;
using Unity.Burst.CompilerServices;


public class Chip : MonoBehaviour
{
    //bool hovered = false;
    [SerializeField]
    public bool selfDestructable = false;
    public bool duplicating = false;
    public bool isInteractable = true;

    public AutoLayout layout;

    ChipManager chipManager;
    Collider hitBox;

    public string elementName = "Number";

    [SerializeField]
    object aboba;

    EquationElement equationElement = null;
    public EquationElement element
    {
        get { return equationElement; }
        set { equationElement = value; 
            elementName = value.GetType().Name; }
    }

    private void OnEnable()
    {
        int num;
        if (int.TryParse(elementName, out num))
            equationElement = EquationElementFactory.Get("Number", num);
        else
            equationElement = EquationElementFactory.Get(elementName);
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
        if (Input.GetMouseButtonDown(0))
        {
            if (hitBox.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100.0f))
            {
                OnClick();
            }
        }
    }   

    void OnClick()
    {
        if (chipManager.selected == null && isInteractable)
        {
            if (duplicating)
            {
                GameObject instance = Instantiate(gameObject, chipManager.transform);
                instance.transform.localRotation = transform.localRotation;
                Chip chip = instance.GetComponent<Chip>();
                chip.duplicating = false;
                chipManager.Select(chip);
            }
            else
            {
                chipManager.Select(this);
            }
        }

    }
}
