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

    Transform startParent = null;
    Vector3 startOffset = Vector3.zero;
    Transform endParent = null;
    Vector3 endOffset = Vector3.zero;


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

    public void Update()
    {
        bool shouldMove = false;
        if (startParent != null)
        {
            shouldMove = true;
        }
        if (endParent != null)
        {
            shouldMove = true;
        }
        if (shouldMove)
            Refresh();
    }
    public void SetTransforms(Transform startPos, Transform endPos, Vector3 startOffset, Vector3 endOffset)
    {
        startParent = startPos;
        endParent = endPos;
        start.parent = startPos;
        end.parent = endPos;

        this.startOffset = startOffset;
        this.endOffset = endOffset;
        start.localPosition = startOffset;
        end.localPosition = endOffset;

        Refresh();
    }
    public void SetTransforms(Transform startPos, Transform endPos)
    {
        SetTransforms(startPos, endPos, Vector3.zero, Vector3.zero);    
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
    public void OnDestroy()
    {
        Destroy(start.gameObject);
        Destroy(end.gameObject);
    }
}
