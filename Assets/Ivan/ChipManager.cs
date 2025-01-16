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

    int currentShiftID = -1;
    Grid inventoryGrid;

    [SerializeField]
    RectTransform equationArea;

    class LayoutInfo
    {
        public int id;
        public bool isShifted;

        public LayoutInfo(int id, bool isShifted = false) 
        {
            this.id = id;
            this.isShifted = isShifted;
        }
    }

    Dictionary<GameObject, LayoutInfo> inventoryItems = new Dictionary<GameObject, LayoutInfo>();



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
        for (int i = 0; i < inventoryGrid.transform.childCount; i++)
        {
            Transform child = inventoryGrid.transform.GetChild(i);

            if (!inventoryItems.ContainsKey(child.gameObject))
            {
                inventoryItems.Add(child.gameObject, new LayoutInfo(i));
            }
        }
    }
    private void Update()
    {
        if (dragging)
        {
            Vector2 cursorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (inventoryGrid.GetComponent<RectTransform>(), Input.mousePosition, null, out cursorPos);

            int readShiftID = GetIDToMove(cursorPos);
            Debug.Log(readShiftID);

            if (readShiftID != currentShiftID)
                ShiftInventoryItems(readShiftID);
        }

    }
    [ExecuteAlways]
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;


    }
    public Vector3Int GetGridPosition(int i)
    {
        return new Vector3Int(i % inventoryRows, - i / inventoryRows, 0);
    }
    public Vector3 GetLocalCellPosition(int i)
    {
        return inventoryGrid.CellToLocal(GetGridPosition(i));
    }
    public int GetGridID(Vector3Int pos)
    {
        if (pos.x < 0 || pos.y > 0 || pos.x >= 6)
            return -1;
        return pos.x - pos.y * inventoryRows;
    }
    public int GetIDToMove(Vector3 pos)
    {
        pos.x += 0.5f * inventoryGrid.cellSize.x;
        pos.y += 0.5f * inventoryGrid.cellSize.y;
        Vector3Int gridPos = inventoryGrid.LocalToCell(pos);
        int ID = GetGridID(gridPos);
        return ID;
    }
    [ContextMenu("Arrange")]
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
        AddToInventory(obj, id);
    }
    public void AddToInventory(GameObject obj, int id)
    {
        obj.transform.parent = inventoryArea.transform;
        obj.transform.localPosition = inventoryGrid.CellToLocal(GetGridPosition(id));
        if (!inventoryItems.ContainsKey(obj))
        {
            inventoryItems.Add(obj, new LayoutInfo(id));
        }
    }
    public void ShiftInventoryItems(int id)
    {
        currentShiftID = id;
        foreach (GameObject go in inventoryItems.Keys)
        {
            LayoutInfo info = inventoryItems[go];
            //we should shift forward
            if (!info.isShifted && id > -1 && info.id >= id)
            {
                info.isShifted = true;
                Vector3 targetPos = GetLocalCellPosition(info.id + 1);
                //go.transform.localPosition = targetPos;
                StartCoroutine(ShiftAnimation(go, go.transform.localPosition, targetPos, 0.2f));
            }
            //we should shift back
            else if (info.isShifted && (info.id < id || id < 0))
            {
                info.isShifted = false;
                Vector3 targetPos = GetLocalCellPosition(info.id);
                //go.transform.localPosition = GetLocalCellPosition(info.id);
                StartCoroutine(ShiftAnimation(go, go.transform.localPosition, targetPos, 0.2f));
            }
        }

    }

    IEnumerator ShiftAnimation(GameObject go, Vector3 startPos, Vector3 endPos, float animTime)
    {
        float elapsed = 0;
        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            
            go.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed/animTime);

            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}
