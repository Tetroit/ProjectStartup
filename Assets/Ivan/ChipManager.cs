using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject selected;

    [SerializeField]
    GameObject inventoryArea;
    public AutoLayout inventoryLayout { get; private set; }

    [SerializeField]
    RectTransform equationArea;


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
    }
    private void Start()
    {
    }
    private void Update()
    {

    }
    [ExecuteAlways]
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;


    }
   
    public void AddChip(Chip chip)
    {
        if (!chips.Contains(chip))
            chips.Add(chip);
    }
    public void RemoveChip(Chip chip)
    {
        if (chips.Contains(chip))
            chips.Remove(chip);
    }
}
