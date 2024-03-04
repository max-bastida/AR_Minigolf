using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathNode
{
    // Point location of the node
    private Vector3 _location;
    public Vector3 Location {
        set {
            _location = value;
            Height = _location.y;
            _location.y = 0;
        }
        get => _location;
    }
    public Vector3 FullLocation { get {
        Vector3 result = Location;
        result.y = Height;
        return result;
    }}
    // Previous node in path
    private PathNode _prev;
    public PathNode Previous {
        get => _prev;
        set
        {
            _prev = value;
            _innerBisector = bisectionVector();
            _angle = findAngle();
        }
    }
    // Next node in path
    private PathNode _next;
    public PathNode Next {
        get => _next;
        set
        {
            _next = value;
            _innerBisector = bisectionVector();
            _angle = findAngle();
        }
    }
    // Vector bisecting angle between nodes, point towards the smallest angle
    private Vector3 _innerBisector;
    public Vector3 InnerBisector {
        get
        {
            if (_innerBisector == Vector3.zero){
                _innerBisector = bisectionVector();
            }
            return _innerBisector;
        }
        }
    public Vector3 LeftVector {
        get
        {
            if (this.Angle < 0){
                return this.InnerBisector * -1;
            }
            return this.InnerBisector;
        }
    }

    // Angle between nodes
    private float _angle;
    public float Angle {
        get
        {
            if (_angle == 0){
                _angle = findAngle();
            }
            return _angle;
        }
        }
    
    public PathNode innerCornerRef {get;set;}
    public float Height {get;set;}
    
    public Vector3 Normal(bool up=true, bool right=false, bool inner=false, bool end=false){
        Vector3 result = LeftVector;
        if (up){
            Vector3 v1; Vector3 v2;
            if (Previous == null) { v1 = Vector3.forward;} else { v1 = Previous.FullLocation - FullLocation;}
            if (Next == null) { v2 = Vector3.forward;} else { v2 = Next.FullLocation - FullLocation;}
            if (v1 == v2){
                return Vector3.up;
            }
            result = Vector3.Cross(v1, v2).normalized;
            if (result.y < 0){ result *= -1;}
            return result;
        }
        if (right){
            result *= -1;
        }
        if (end){
            if (_next == null){
                result = (Location - Previous.Location).normalized;
            } else {
                result = (Location - Next.Location).normalized;
            }
        }
        if (inner){
            result *= -1;
        }
        return result;
    }

    public PathNode(Vector3 point, PathNode prevNode){
        this.Location = point;
        this.Previous = prevNode;
        this.Previous.Next = this;
    }
    public PathNode(Vector3 point){
        this.Location = point;
    }

    //180 - ||180-theta|| (degrees)

    /// <summary>
    /// Calculates the vector at the point towards the acute angle (if there is one)
    /// formed by this node and the previous and next nodes.
    /// Uses a vector calcuation function depending on whether previous or next nodes exist.
    /// </summary>
    /// <returns> Vector associated with this node </returns>
    /// <exception cref="InvalidOperationException"> If there are no previous or next nodes </exception>
    Vector3 bisectionVector(){
        if (Previous == null && Next == null){
            throw new InvalidOperationException("Previous and Next nodes not initialized");
        } else if (Next == null){
            return calculateBisectionVector(Previous.Location, Location);
        } else if (Previous == null) {
            return calculateBisectionVector(Location, Next.Location);
        } else {
            return calculateBisectionVector(Previous.Location, Location, Next.Location);
        }
    }

    /// <summary>
    /// Calculate the vector bisecting the acute angle formed by 3 points.
    /// </summary>
    /// <param name="p1"> point 1 </param>
    /// <param name="p2"> point 2 </param>
    /// <param name="p3"> point 3 </param>
    /// <returns> a vector bisecting the acute angle formed by the given points. </returns>
    /// <exception cref="InvalidOperationException"> When two of the points given are the same point </exception>
    static Vector3 calculateBisectionVector(Vector3 p1, Vector3 p2, Vector3 p3){
        // p1 = p2
        Vector3 u1 = (p2 - p1).normalized;
        Vector3 u2 = (p3 - p2).normalized;
        if (u1 == u2) {
            if(u1 == Vector3.zero){
                //string.Format("Location: {0}. Vector: {1}. Angle: {2}", current.Location, current.InnerBisector, current.Angle)
                throw new InvalidOperationException(string.Format("Points on top of each other: {0}, {1}", p2.x, p2.z));
            } else {
                u1 = Quaternion.AngleAxis(-90, Vector3.up) * u1;
            }
            return u1;
        }
        return (u2 - u1).normalized;
    }

    /// <summary>
    /// Calculate the vector normal to the vector between 2 points
    /// </summary>
    /// <param name="p1"> point 1 </param>
    /// <param name="p2"> point 2 </param>
    /// <returns> a vector normal to the vector between the given points. </returns>
    /// <exception cref="InvalidOperationException"> When the points given are the same point </exception>
    static Vector3 calculateBisectionVector(Vector3 p1, Vector3 p2){
        Vector3 u1 = (p2 - p1).normalized;
        if(u1 == Vector3.zero){
                throw new InvalidOperationException(string.Format("Points on top of each other: {0}, {1}", p2.x, p2.z));
        }
        u1 = Quaternion.AngleAxis(-90, Vector3.up) * u1;
        return u1;
    }

    float findAngle(){
        if (Previous == null || Next == null){
            return 0;
        } else {
            return cornerAngle(Previous.Location, Location, Next.Location);
        }
    }

    static float cornerAngle(Vector3 p1, Vector3 p2, Vector3 p3){
        Vector3 u1 = (p1 - p2).normalized;
        Vector3 u2 = (p3 - p2).normalized;
        return Vector3.SignedAngle(u1, u2, Vector3.up);
    }

    public Vector3 getVertex(float courseWidth, bool rightVertex=false){
        Vector3 intersection = Vector3.zero;
        if (innerCornerRef != null){
            if ((LeftVector == InnerBisector) != rightVertex){
                return innerCornerRef.getVertex(courseWidth, rightVertex);
            }
        }
        Vector3 path;
        if (Previous != null){
            path = (Location - Previous.Location).normalized;
        } else {
            path = (Next.Location - Location).normalized;
        }
        Vector3 width = Quaternion.AngleAxis(-90, Vector3.up) * path * courseWidth;

        Vector3 dir = LeftVector;
        if (rightVertex) {
            dir = dir * -1;
            width = width * - 1;
        }
        GeometryUtils.findIntersection(out intersection, Location, Location + width, dir, path);

        if(Previous == null){
            intersection -= path * courseWidth;
        } else if (_next == null) {
            intersection += path * courseWidth;
        }
        intersection.y += Height;
        return intersection;
    }

}