using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class PathGenerator
{
    static public PathNode linkNodes(List<Vector3> points){
        // Check that there are enough points in the list
        if (points.Count < 2){
            throw new ArgumentException(String.Format("{0} points were given but need at least 2 to render path", points.Count));
        }
        PathNode head = new PathNode(points[0]);
        PathNode previous = head;
        PathNode current;

        foreach (Vector3 point in points.Skip(1))
        {
            current = new PathNode(point, previous);
            previous = current;
        }
        return head;
    }



    static public void smoothCorners(PathNode head, float width){
        // loop through each path node and check angle
        PathNode current = head.Next;
        while (current.Next != null){
            // if angle is < 90, replace with new nodes
            if (Math.Abs(current.Angle) < 90){
                // calculate new poimt locations
                // get centre point
                Vector3 centerPoint = current.Location + current.InnerBisector * width;
                Vector3 centerDir = Quaternion.AngleAxis(-90, Vector3.up) * current.InnerBisector;
                Vector3 newLocation1; Vector3 newLocation2;
                GeometryUtils.findIntersection(out newLocation1, centerPoint, current.Location, centerDir, current.Previous.Location - current.Location);
                GeometryUtils.findIntersection(out newLocation2, centerPoint, current.Location, centerDir, current.Next.Location - current.Location);
                // create nodes
                PathNode node1 = new PathNode(newLocation1);
                node1.Height = current.Height;
                PathNode node2 = new PathNode(newLocation2);
                node2.Height = current.Height;
                PathNode previous = current.Previous;
                PathNode next = current.Next;
                // check if inner corner needs adjusting
                if (Math.Abs(current.Angle) < 60){
                    node1.innerCornerRef = current;
                    node2.innerCornerRef = current;
                }
                // link nodes
                if(previous.Location != node1.Location && node1.Location != node2.Location && node2.Location != next.Location) {
                    previous.Next = node1;
                    node1.Previous = previous;
                    node1.Next = node2;
                    node2.Previous = node1;
                    node2.Next = next;
                    next.Previous = node2;
                }
            }
            current = current.Next;
        }
    }

    static public void addRamps(PathNode head, float courseWidth, float wallWidth){
        PathNode current = head;
        while (current != null ){
            float innerAngle = Mathf.Deg2Rad * Mathf.Abs(current.Angle);
            float distance;
            if (innerAngle == 0){
                distance = courseWidth;
            } else{
                distance = (wallWidth + courseWidth) / Mathf.Tan(innerAngle / 2);
            }

            if(current.Previous != null && current.Height != current.Previous.Height && (current.Location - current.Previous.Location).magnitude > distance){
                Vector3 newLocation = ((current.Previous.Location - current.Location).normalized * distance) + current.Location;
                newLocation.y = current.Height;
                PathNode newNode = new PathNode(newLocation);
                // link nodes
                PathNode previous = current.Previous;
                if(newLocation != previous.Location && newLocation != current.Location){
                    previous.Next = newNode;
                    newNode.Previous = previous;

                    newNode.Next = current;
                    current.Previous = newNode;
                }

            }
            if(current.Next != null && current.Height != current.Next.Height && (current.Location - current.Next.Location).magnitude > distance){
                Vector3 newLocation = ((current.Next.Location - current.Location).normalized * distance) + current.Location;
                newLocation.y = current.Height;
                PathNode newNode = new PathNode(newLocation);
                // link nodes
                PathNode next = current.Next;
                if(newLocation != next.Location && newLocation != current.Location){
                    current.Next = newNode;
                    newNode.Previous = current;

                    newNode.Next = next;
                    next.Previous = newNode;
                    current = newNode;
                }
            }
            current = current.Next;

        }
    }

    static public Vector3 lastPointLocation(PathNode head){
        PathNode current = head;
        while (current.Next != null){
            current = current.Next;
        }
        Vector3 location = current.Location;
        location.y = current.Height;
        return location;
    }

    static public void removeClosePoints(PathNode head, float courseWidth){
        PathNode current = head;
        while (current.Next != null){
            if ((current.Location - current.Next.Location).magnitude <= (courseWidth / 4)){
                Debug.Log((current.Location - current.Next.Location).magnitude);
                // remove next node
                PathNode next = current.Next.Next;
                if (next == null && current.Previous != null){
                    current.Previous.Next = next;
                    next.Previous = current.Previous;
                } else if (next != null){
                    current.Next = next;
                    next.Previous = current;
                }
            }
            current = current.Next;
        }
    }

    static public int countPoints(PathNode head){
        int count = 0;
        PathNode current = head;
        while (current != null){
            count += 1;
            current = current.Next;
        }
        return count;
    }
}