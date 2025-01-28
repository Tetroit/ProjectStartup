using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PinObject : MonoBehaviour
{
    [SerializeField]
    Transform start;
    [SerializeField]
    Transform end;

    [SerializeField]
    LineRenderer lineRenderer;

    [ExecuteInEditMode]

    private void OnValidate()
    {
        if (start == null)
            start = transform;
        lineRenderer = GetComponent<LineRenderer>();
        if (end == null)
        {
            Debug.LogWarning("end is not set");
            return;
        }

    }

    public void SetPositions(Vector3 startPos, Vector3 endPos)
    {
        start.position = startPos;
        end.position = endPos;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    [ContextMenu(itemName:"Refresh")]
    public void Refresh()
    {
        if (start == null || end == null || lineRenderer == null)
            return;
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);
    }
}
