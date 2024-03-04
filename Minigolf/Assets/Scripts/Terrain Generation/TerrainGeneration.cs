using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{

    public GameObject course;
    public GameObject flag;
    public Material greenMat;
    public Material wallsMat;
    private CourseRenderer courseRenderer;

    public float courseWidth;
    public float wallWidth;
    public float wallHeight;

    [Range(0.5f, 100f)]
    [SerializeField]
    private float resetThresholdOffset = 0.5f;

    public static float GetResetThreshold(PathNode head, float resetZoneOffset)
    {
      PathNode current = head;
      float lowestY = current.Height;
      while (current.Next != null)
      {
        lowestY = Math.Min(current.Height, lowestY);
        current = current.Next;
      }

      return lowestY - resetZoneOffset;
    }
    
    public void createCourse(List<Vector3> points)
    {
        //create a course renderer
        Debug.Log(points.Count);
        PathNode head = PathGenerator.linkNodes(points);
        PathGenerator.removeClosePoints(head, courseWidth);
        //CalculateResetValue(head, resetThresholdOffset);
        PathGenerator.addRamps(head, courseWidth, wallWidth);
        PathGenerator.smoothCorners(head, courseWidth);
        PathGenerator.removeClosePoints(head, courseWidth);
        PathNode current = head;
        Debug.Log(PathGenerator.countPoints(head));
        while(current != null){
            Debug.Log(current.Location);
            current = current.Next;
        }
        courseRenderer = new CourseRenderer(head, course, PathGenerator.countPoints(head), courseWidth, wallWidth, wallHeight);
        courseRenderer.renderGreen(greenMat);
        courseRenderer.renderWalls(wallsMat);

        GameObject controller = GameObject.Find("Gameplay Controller").gameObject;
        GameController gameController = controller.GetComponent<GameController>();
        gameController.lowestTerrainPoint = GetResetThreshold(head, resetThresholdOffset);
        gameController.ballStartPos = head.Location + (Vector3.up * 0.015f);

        flag.transform.position = PathGenerator.lastPointLocation(head);
        flag.SetActive(true);
    }
}
