using Equation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ChipManager : MonoBehaviour
{
    [HideInInspector]
    public Chip selected;

    [SerializeField]
    GameObject inventoryArea;
    [SerializeField]
    GameObject equationArea;

    [SerializeField]
    TextMeshProUGUI equationDisplay;
    public AutoLayout inventoryLayout { get; private set; }
    public AutoLayout equationLayout { get; private set; }


    List<Chip> chips = new List<Chip>();

    static ChipManager _instance;
    public static ChipManager instance => _instance;

    Formula formula = new Formula();

    void Awake()
    {
        if (instance == null)
        {
            _instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    public IEnumerable GetInventoryChips()
    {
        return null;
    }
    public IEnumerable GetEquationChips()
    {
        return null;
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        //inventoryLayout = inventoryArea.GetComponent<AutoLayout>();
        //equationLayout = equationArea.GetComponent<AutoLayout>();
    }
    private void Start()
    {
        foreach (Transform tr in equationArea.transform) 
        {
            Chip chip = tr.GetComponent<Chip>();
            if (chip && chip.element)
                formula.AddElement(chip.element);
        }
        UpdateEquation();
    }
    private void Update()
    {
        if (selected != null)
        {
            selected.transform.localPosition = Utils.GetMousePos(transform);
            if (Input.GetMouseButtonUp(0))
            {
                if (selected.selfDestructable)
                    DeselectSelfDestructive();
                else
                    Deselect();
            }
        }
    }
    [ExecuteAlways]
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;


    }
   
    public void AddChip(Chip chip)
    {
        if (!chips.Contains(chip))
        {
            chips.Add(chip);
            if (chip.transform.parent == inventoryArea.transform)
                chip.layout = inventoryLayout;
            if (chip.transform.parent == equationArea.transform)
                chip.layout = equationLayout;
        }
    }
    public void RemoveChip(Chip chip)
    {
        if (chips.Contains(chip))
            chips.Remove(chip);
    }
    public void Select(Chip chip)
    {
        if (chip.layout)
            chip.layout.RemoveItem(chip.gameObject, true);

        if (!chip.selfDestructable)
            inventoryLayout.EnableShifting();
        equationLayout.EnableShifting();

        selected = chip;
        chip.transform.parent = transform;
    }
    public void DeselectSelfDestructive()
    {
        if (equationLayout.currentShiftID != -1)
        {
            AddToLayout(selected, equationLayout, equationLayout.currentShiftID);
        }
        else
        {
            if (selected.layout == equationLayout)
            {
                equationLayout.RemoveItem(selected.gameObject, true);
                formula.RemoveElement(selected.element);
                UpdateEquation();
            }
            Destroy(selected.gameObject);
        }

        equationLayout.DisableShifting();
        selected = null;
    }
    public void Deselect()
    {

        if (equationLayout.currentShiftID != -1)
        {
            AddToLayout(selected, equationLayout, equationLayout.currentShiftID);
        }
        else if (inventoryLayout.currentShiftID != -1)
        {
            AddToLayout(selected, inventoryLayout, inventoryLayout.currentShiftID);
        }
        else
        {
            AddBack(selected);
        }

        inventoryLayout.DisableShifting();
        equationLayout.DisableShifting();
        selected = null;
    }

    public void AddBack(Chip chip)
    {
        AddToLayout(chip, chip.layout);
    }

    public void AddToLayout(Chip chip, AutoLayout layout, int id = -1)
    {
        if (chip.layout == equationLayout)
        {
            formula.RemoveElement(chip.element);
            UpdateEquation();
        }
        if (layout == equationLayout)
        {   if (id == -1)
                formula.AddElement(chip.element);
            else
                formula.AddElement(chip.element, id);
            UpdateEquation();
        }

        chip.layout = layout;

        if (id == -1)
            layout.AddObject(selected.gameObject, true);
        else
            layout.AddObject(selected.gameObject, id, true);

    }
    void UpdateEquation()
    {
        bool valid = formula.Validate();
        if (valid)
            equationDisplay.color = Color.green;
        else
            equationDisplay.color = Color.red;

        equationDisplay.text = formula.ToString() + (valid ? (" = " + formula.Calculate()) : "");
    }
}
