using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class Utils
{
    /// <summary>
    /// Gives mouse cursor position in local space of <paramref name="obj"/>
    /// </summary>
    /// <param name="obj">
    /// transform in which coordinate system the result will be given
    /// </param>
    /// <param name="cam"></param>
    /// <returns></returns>
    static public Vector2 GetMousePos(Transform obj, Camera cam = null)
    {
        if (cam == null ) cam = Camera.main;
        Ray mouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 normal = Vector3.Cross(obj.up, obj.right);
        float fac =
        Vector3.Dot(obj.position - mouse.origin, normal) /
        Vector3.Dot(mouse.direction, normal);

        Vector3 intersection = fac * mouse.direction + mouse.origin;

        return obj.InverseTransformPoint(intersection);

    }
    /// <summary>
    /// Gives mouse cursor position in local space of <paramref name="obj"/>
    /// </summary>
    /// <param name="obj">
    /// transform in which coordinate system the result will be given
    /// </param>
    /// <param name="up">
    /// up direction of projection plane in object's local space
    /// </param>
    /// <param name="right">
    /// right direction of projection plane in object's local space
    /// </param>
    /// <param name="cam"></param>
    /// <returns></returns>
    static public Vector2 GetMousePos(Transform obj, Vector3 up, Vector3 right, Camera cam = null)
    {
        if (cam == null) cam = Camera.main;
        Ray mouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 normal = Vector3.Normalize(Vector3.Cross(obj.up, obj.right));
        float fac =
        Vector3.Dot(obj.position - mouse.origin, normal) /
        Vector3.Dot(mouse.direction, normal);

        Vector3 intersection = fac * mouse.direction + mouse.origin;

        return obj.InverseTransformPoint(intersection);

    }
}
