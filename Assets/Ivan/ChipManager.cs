using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class ChipManager : MonoBehaviour
{
    [SerializeField]
    RectTransform inventoryArea;
    [SerializeField]
    public int inventoryRows = 6;
    [SerializeField]
    public Vector2 inventoryCellSize = new Vector2(120, 120);

    public Vector2 spacing {
        get => inventoryGrid.cellGap;
        set { inventoryGrid.cellGap = value; }
    }
    [SerializeField]
    public Vector2 padding = new Vector2 (10, 10);


    Grid inventoryGrid;

    [SerializeField]
    RectTransform equationArea;

    List<Chip> chipListInventory = new List<Chip>();
    List<Chip> chipListEquation = new List<Chip>();
    public IEnumerable GetInventoryChips()
    {
        return chipListInventory;
    }
    public IEnumerable GetEquationChips()
    {
        return chipListInventory;
    }

    public Vector2 GetNextFreeGridPosition()
    {
        GridLayoutGroup gridLayout = inventoryArea.GetComponent<GridLayoutGroup>();
        Vector2 lastPos = gridLayout.transform.GetChild(inventoryArea.childCount - 1).localPosition;
        Vector2 nextPos = lastPos + new Vector2(gridLayout.cellSize.x + gridLayout.spacing.x, 0);
        return nextPos;
    }
    [ExecuteInEditMode]
    private void OnValidate()
    {
        inventoryGrid = GetComponentInChildren<Grid>();
        inventoryGrid.transform.localPosition = new Vector2 (0, inventoryArea.rect.height) + (padding + inventoryCellSize/2) * new Vector2(1, -1);
        ArrangeInventoryChildren();
    }
    private void Start()
    {
        chipListInventory = inventoryArea.GetComponentsInChildren<Chip>().ToList();
    }
    private void Update()
    {

    }
    [ExecuteAlways]
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;

        Vector2 cursorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (inventoryGrid.GetComponent<RectTransform>(), Input.mousePosition, null, out cursorPos);

        //Debug.Log(GetIDToMove(cursorPos));
        Gizmos.DrawSphere(cursorPos, 1);
        Vector3 pos = inventoryGrid.CellToLocal(inventoryGrid.LocalToCell(cursorPos));
        Gizmos.DrawWireCube(inventoryGrid.transform.TransformPoint(pos), Vector3.Scale(inventoryGrid.transform.lossyScale, inventoryGrid.cellSize));

    }
    public Vector3Int GetGridPosition(int i)
    {
        return new Vector3Int(i % inventoryRows, - i / inventoryRows, 0);
    }
    public int GetGridID(Vector3Int pos)
    {
        if (pos.x < 0 || pos.y > 0 || pos.x >= 6)
            return -1;
        return pos.x - pos.y * inventoryRows;
    }
    public int GetIDToMove(Vector3 pos)
    {
        pos.y += 0.5f * inventoryGrid.cellSize.y;
        Vector3Int gridPos = inventoryGrid.LocalToCell(pos);
        gridPos.x++;
        int ID = GetGridID(gridPos);
        return ID;
    }
    [ContextMenu("arrange")]
    public void ArrangeInventoryChildren()
    {
        for (int i = 0; i < inventoryGrid.transform.childCount; i++)
        {
            Transform child = inventoryGrid.transform.GetChild(i);
            child.localPosition = inventoryGrid.CellToLocal(GetGridPosition(i));
        }
    }

    public void AddToInventory(GameObject obj)
    {
        int id = inventoryArea.transform.childCount;
        obj.transform.parent = inventoryArea.transform;
        obj.transform.localPosition = inventoryGrid.CellToLocal(GetGridPosition(id));
    }
}
