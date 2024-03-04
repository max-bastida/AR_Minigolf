using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometryUtils
{
    /// <summary>
    /// Calculates the intersection of two lines defined by point and vector pairs.
    /// Uses only the x-z plane.
    /// Returns true if intersection is found, and false if there is no intersection (lines are parallel).
    /// </summary>
    /// <param name="intersection"> the calculated intersection point output </param>
    /// <param name="p"> point defining line 1 </param>
    /// <param name="q"> point defining line 2 </param>
    /// <param name="u"> vector defining line 1 </param>
    /// <param name="v"> vector defining line 2 </param>
    /// <returns></returns>
    public static bool findIntersection(out Vector3 intersection, Vector3 p, Vector3 q, Vector3 u, Vector3 v){
        intersection = new Vector3 (0.0f, 0.0f, 0.0f);
        float denom = (v.x*u.z - v.z*u.x);
        // if denom is 0, lines are parallel, no intersection
        if (Mathf.Abs(denom) < 0.001f){
            return false;
        }
        float s = ((q.z - p.z)*u.x - (q.x - p.x)*u.z) / (v.x*u.z - v.z*u.x);
        intersection.x = q.x + s * v.x;
        intersection.z = q.z + s * v.z;

        return true;
    }
}
