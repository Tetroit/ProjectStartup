using Equation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[RequireComponent(typeof(Grid))]
public class AutoLayout : MonoBehaviour
{
    [SerializeField]
    public bool shiftingMode = false;
    [SerializeField]
    public int columns = 6;
    [SerializeField]
    public int rows = 3;
    [SerializeField]
    public Vector2 cellSize = new Vector2(120, 120);
    [SerializeField]
    AnimationCurve shiftingAnim;
    public Vector2 spacing
    {
        get => grid.cellGap;
        set { grid.cellGap = value; }
    }
    [SerializeField]
    public Vector2 padding = new Vector2(10, 10);

    public int currentShiftID { get; private set; } = -1;
    Grid grid;
    class LayoutInfo
    {
        public bool isShifted;
        public GameObject obj;

        public LayoutInfo(GameObject go, bool isShifted = false)
        {
            this.isShifted = isShifted;
            obj = go;
        }
    }

    List<LayoutInfo> items = new List<LayoutInfo>();
    public int Count => items.Count;

    public IEnumerable<T> GetObjects<T>()
    {
        List<T> list = new List<T>();
        for (int i = 0; i < Count; i++)
        {
            T item = items[i].obj.GetComponent<T>();
            if (item != null)
                list.Add(item);
        }
        return list;
    }
    private void Start()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Transform child = grid.transform.GetChild(i);
            items.Add( new LayoutInfo(child.gameObject));
        }
    }
    private void Update()
    {
        if (shiftingMode)
        {
            UpdateShift();
        }
    }
    public void EnableShifting()
    {
        shiftingMode = true;
    }
    public void DisableShifting()
    {
        shiftingMode = false;
        ShiftItems(items.Count);
    }
    void UpdateShift()
    {
        Vector2 cursorPos = GetMousePos();

        int readShiftID = GetIDToMove(cursorPos);

        if (readShiftID != currentShiftID)
            ShiftItems(readShiftID);
    }
    private void OnValidate()
    {
        grid = GetComponent<Grid>();
    }

    [ContextMenu("Arrange")]
    public void ArrangeChildren()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Transform child = grid.transform.GetChild(i);
            child.localPosition = grid.CellToLocal(GetGridPosition(i));
        }
    }
    public Vector3Int GetGridPosition(int i)
    {
        return new Vector3Int(i % columns, -i / columns, 0);
    }
    public Vector3 GetLocalCellPosition(int i)
    {
        return grid.CellToLocal(GetGridPosition(i));
    }
    public int GetGridID(Vector3Int pos)
    {
        if (pos.x < 0 || pos.y > 0 || pos.x >= columns)
            return -1;
        return pos.x - pos.y * columns;
    }
    public int GetIDToMove(Vector3 pos)
    {
        pos.x += 0.5f * grid.cellSize.x;
        pos.y += 0.5f * grid.cellSize.y;
        Vector3Int gridPos = grid.LocalToCell(pos);
        int ID = GetGridID(gridPos);
        if (ID > rows*columns)
            return -1;
        if (ID > items.Count)
            return items.Count;
        return ID;
    }

    public void AddObject(GameObject obj, bool disableShifting = false)
    {
        int id = transform.childCount;
        AddObject(obj, id, disableShifting);
    }
    public void AddObject(GameObject obj, int id, bool disableShifting = false)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = grid.CellToLocal(GetGridPosition(id));
        if (!Has(obj))
        {
            items.Insert(id, new LayoutInfo(obj));
        }
        if (disableShifting) shiftingMode = false;
        for (int i=0; i<items.Count; i++)
            items[i].isShifted = false;
    }
    public bool Has(GameObject obj)
    {
        foreach (LayoutInfo info in items)
        {
            if (info.obj == obj) return true;
        }
        return false;
    }
    public void ShiftItems(int id)
    {
        currentShiftID = id;
        for (int i=0; i<items.Count; i++)
        {
            LayoutInfo info = items[i];
            GameObject go = info.obj;
            //we should shift forward
            if (!info.isShifted && id > -1 && i >= id)
            {
                info.isShifted = true;
                Vector3 targetPos = GetLocalCellPosition(i + 1);
                //go.transform.localPosition = targetPos;
                StartCoroutine(ShiftAnimation(go, go.transform.localPosition, targetPos, 0.2f));
            }
            //we should shift back
            else if (info.isShifted && (i < id || id < 0))
            {
                info.isShifted = false;
                Vector3 targetPos = GetLocalCellPosition(i);
                //go.transform.localPosition = GetLocalCellPosition(info.id);
                StartCoroutine(ShiftAnimation(go, go.transform.localPosition, targetPos, 0.2f));
            }
        }

    }
    public void RemoveItem(int id, bool enableShifting = false)
    {
        if (enableShifting)
        {
            shiftingMode = true;
        }
        for (int i = 0; i < items.Count; i++)
        {
            LayoutInfo info = items[i];
            if (i > id)
            {
                if (enableShifting)
                {
                    info.isShifted = true;
                }
                else
                {
                    Vector3 targetPos = GetLocalCellPosition(i - 1);
                    StartCoroutine(ShiftAnimation(info.obj, info.obj.transform.localPosition, targetPos, 0.2f));
                }
            }
        }
        items.RemoveAt(id);
    }
    public void RemoveItem(GameObject go, bool enableShifting = false)
    {
        for (int i=0; i< items.Count; i++)
        { 
            if (items[i].obj == go) 
            {
                RemoveItem(i, enableShifting);
                return; 
            } 
        }
    }

    IEnumerator ShiftAnimation(GameObject go, Vector3 startPos, Vector3 endPos, float animTime)
    {
        float elapsed = 0;
        while (elapsed < animTime)
        {
            if (go == null)
                yield break;

            elapsed += Time.deltaTime;

            float fac = (elapsed / animTime);
            go.transform.localPosition = Vector3.Lerp(startPos, endPos, shiftingAnim.Evaluate(fac)) + new Vector3(0, fac * (1 - fac), 0);

            yield return new WaitForEndOfFrame();
        }
        go.transform.localPosition = endPos;
        yield break;
    }
    //Vector2 GetMousePos()
    //{   
    //    Vector2 cursorPos;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle
    //        (grid.GetComponent<RectTransform>(), Input.mousePosition, null, out cursorPos);
    //    return cursorPos;
    //}
    Vector2 GetMousePos()
    {
        Ray mouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 normal = Vector3.Cross(transform.up, transform.right);
        float fac =
        Vector3.Dot(transform.position - mouse.origin, normal)/
        Vector3.Dot(mouse.direction, normal);

        Vector3 intersection = fac * mouse.direction + mouse.origin;

        return transform.InverseTransformPoint(intersection);
        
    }
}
