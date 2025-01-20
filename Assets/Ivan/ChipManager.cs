using System.Collections;
using System.Collections.Generic;
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
    public AutoLayout inventoryLayout { get; private set; }
    public AutoLayout equationLayout { get; private set; }


    List<Chip> chips = new List<Chip>();

    static ChipManager _instance;
    public static ChipManager instance => _instance;

    void Awake()
    {
        if (instance == null)
        {
            _instance = this;
        }
        else if (instance != this) Destroy(gameObject);
    }

    [Space(30)]
    [Header("So this option enables dragging... Like... \n\nImagine that you are always dragging an item okay?\n As if you are about to place it into the grid")]
    [Space(20)]
    public bool dragging = false;

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
        inventoryLayout = inventoryArea.GetComponent<AutoLayout>();
        equationLayout = equationArea.GetComponent<AutoLayout>();
    }
    private void Start()
    {
    }
    private void Update()
    {
        if (selected != null)
        {
            selected.transform.localPosition = Utils.GetMousePos(transform);
            if (Input.GetMouseButtonUp(0))
                Deselect();
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
        chip.layout.RemoveItem(chip.gameObject, true);
        inventoryLayout.shiftingMode = true;
        equationLayout.shiftingMode = true;
        selected = chip;
        chip.transform.parent = transform;
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
        inventoryLayout.shiftingMode = false;
        equationLayout.shiftingMode = false;
        selected = null;
    }

    public void AddBack(Chip chip)
    {
        AddToLayout(chip, chip.layout);
    }

    public void AddToLayout(Chip chip, AutoLayout layout, int id = -1)
    {
        chip.layout = layout;
        if (id == -1)
            layout.AddObject(selected.gameObject, true);
        else
            layout.AddObject(selected.gameObject, id, true);

    }
}
