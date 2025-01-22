using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Equation;
using UnityEngine.UI;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI;


public class Chip : MonoBehaviour
{
    //bool hovered = false;
    [SerializeField]
    public bool selfDestructable = false;
    public bool duplicating = false;

    public AutoLayout layout;

    ChipManager chipManager;
    Collider hitBox;

    [SerializeField]
    string elementName = "Number";

    [SerializeField]
    object aboba;

    EquationElement equationElement = null;
    public EquationElement element => equationElement;

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
    }   

    void OnClick()
    {
        if (chipManager.selected == null)
        {
            if (duplicating)
            {
                GameObject instance = Instantiate(gameObject);
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

    private void OnMouseDown()
    {
        RaycastHit hit;
        if (hitBox.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
        {
            OnClick();
        }
    }
}
