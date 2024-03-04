using System.Collections.Generic;
using UnityEngine;


public class CourseRenderer
{
  private float courseWidth;
  private float wallWidth;
  private float wallHeight;
  private GameObject courseObject;
  private PathNode head;
  private int numPoints;

  public CourseRenderer(PathNode head, GameObject courseObject, int numPoints, float courseWidth,
    float wallWidth, float wallHeight)
  {
    this.courseObject = courseObject;
    this.head = head;
    this.numPoints = numPoints;
    this.courseWidth = courseWidth;
    this.wallWidth = wallWidth;
    this.wallHeight = wallHeight;
  }

  public void printValues()
  {
    PathNode current = head;
    while (current != null)
    {
      Debug.Log(string.Format("Location: {0}. Vector: {1}. Angle: {2}", current.Location,
        current.InnerBisector, current.Angle));
      current = current.Next;
    }
  }

  public void renderGreen(Material greenMat)
  {
    Mesh greenMesh = generateGreenMesh();
    createMeshObject(greenMesh, "Green", greenMat);
  }

  public void renderWalls(Material wallMat)
  {
    Mesh wallMesh = generateWallMesh();
    PhysicMaterial wallPhysics = new PhysicMaterial();
    wallPhysics.bounciness = 0.7f;
    wallPhysics.dynamicFriction = 0.3f;
    createMeshObject(wallMesh, "Wall", wallMat, wallPhysics);
  }

  // private void generateHoleVerticesAndNormals(PathNode pathNode, List<Vector3> verticesList,
  //   List<Vector3> normalsList)
  // {
  //   var subdivisions = 1;
  //   var diameter = 0.1f;
  //   var bottomLeft = verticesList[^2];
  //   var bottomRight = verticesList[^1];
  //   var topLeft = pathNode.getVertex(courseWidth);
  //   var topRight = pathNode.getVertex(courseWidth, true);
  //
  //   // Add new vertices for the circle (midpoints)
  //   var midpointLeft = (bottomLeft + topLeft) / 2;
  //   var midpointTop = (topLeft + topRight) / 2;
  //   var midpointRight = (bottomRight + topRight) / 2;
  //   var midpointBottom = (bottomLeft + bottomRight) / 2;
  //   var center = (bottomLeft + bottomRight + topLeft + topRight) / 4;
  //
  //   List<Vector3> points = new List<Vector3>()
  //     { midpointRight, topRight, midpointTop, topLeft, midpointLeft, bottomLeft };
  //
  //   for (int i = 0; i < points.Count - 1; i++)
  //   {
  //     verticesList.Add(points[i]);
  //     verticesList.Add(center - new Vector3(points[i].x * diameter, 0, points[i].y * diameter));
  //     verticesList.Add(center -
  //                      new Vector3(points[i + 1].x * diameter, 0, points[i + 1].y * diameter));
  //     verticesList.Add(points[i]);
  //     normalsList.Add(Vector3.up);
  //     normalsList.Add(Vector3.up);
  //     normalsList.Add(Vector3.up);
  //     normalsList.Add(Vector3.up);
  //       
  //   }
  // }

  private Mesh generateGreenMesh()
  {
    Mesh mesh = new Mesh();
    List<Vector3> verticesList = new List<Vector3>();
    List<int> trianglesList = new List<int>();
    List<Vector3> normalsList = new List<Vector3>();

    // loop through path nodes to add vertices and normals
    PathNode current = head;
    while (current != null)
    {
      verticesList.Add(current.getVertex(courseWidth));
      normalsList.Add(current.Normal());
      verticesList.Add(current.getVertex(courseWidth, true));
      normalsList.Add(current.Normal());
      if (current.Next.Next == null)
      {
        current = current.Next;
        // generateHoleVerticesAndNormals(current, verticesList, normalsList);
        verticesList.Add(current.getVertex(courseWidth));
        normalsList.Add(current.Normal());
        verticesList.Add(current.getVertex(courseWidth, true));
        normalsList.Add(current.Normal());
        break;
      }

      current = current.Next;
    }

    // generate triangles
    for (int i = 0; i < verticesList.Count - 2; i += 2)
    {
      // Triangle 1
      trianglesList.Add(i);
      trianglesList.Add(i + 2);
      trianglesList.Add(i + 1);

      // Triangle 2
      trianglesList.Add(i + 1);
      trianglesList.Add(i + 2);
      trianglesList.Add(i + 3);
    }

    mesh.vertices = verticesList.ToArray();
    mesh.triangles = trianglesList.ToArray();
    mesh.normals = normalsList.ToArray();

    return mesh;
  }

  private Mesh generateWallMesh()
  {
    Mesh mesh = new Mesh();

    // setup empty arrays to define mesh
    int num_base_vertices = numPoints * 12;
    // TODO add more vertices for more realistic shading (no shared vertices between faces)
    Vector3[] vertices = new Vector3[num_base_vertices + 16];
    int[] triangles = new int[numPoints * 36];
    Vector3[] normals = new Vector3[vertices.Length];
    int index = 0;
    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;

    // for each wall generate mesh info
    foreach (bool wall in new[] { false, true })
    {
      // loop through path nodes to add vertices and normals
      PathNode current = head;
      while (current != null)
      {
        // generate vectors
        p1 = current.getVertex(courseWidth, wall);
        p2 = p1 + Vector3.up * wallHeight;
        p3 = current.getVertex(courseWidth + wallWidth, wall);
        p4 = p3 + Vector3.up * wallHeight;

        // add vectors
        // we want to shade with face normals instead of vertex normals so have to duplicate some vectors
        // facing inwards
        vertices[index] = p1;
        vertices[index + 1] = p2;
        normals[index] = normals[index + 1] = current.Normal(false, wall, true);
        //facing up
        vertices[index + 2] = p2;
        vertices[index + 3] = p4;
        normals[index + 2] = normals[index + 3] = current.Normal();
        // facing outwards
        vertices[index + 4] = p4;
        vertices[index + 5] = p3;
        normals[index + 4] = normals[index + 5] = current.Normal(false, wall, false);
        index += 6;
        current = current.Next;
      }
    }

    int startWall2 = num_base_vertices / 2;
    // add extra vertices for generating ends of course
    foreach (int x in new[] { 0, startWall2, startWall2 - 6, (startWall2 * 2) - 6 })
    {
      // wall inner
      vertices[index] = vertices[x];
      vertices[index + 1] = vertices[x + 1];
      normals[index] = normals[index + 1] = head.Normal(inner: true, end: true);
      index += 2;
      // wall outer
      vertices[index] = vertices[x + 4];
      vertices[index + 1] = vertices[x + 5];
      normals[index] = normals[index + 1] = head.Normal(inner: false, end: true);
      index += 2;
    }

    // generate triangles for one side
    int i = 0;
    int j = 6;
    index = 0;
    while (j < (num_base_vertices / 2) - 5)
    {
      for (int x = 0; x < 3; x++)
      {
        // triangle 1
        triangles[index] = i;
        triangles[index + 1] = i + 1;
        triangles[index + 2] = j + 1;
        index += 3;
        // triangle 2
        triangles[index] = i;
        triangles[index + 1] = j + 1;
        triangles[index + 2] = j;
        i += 2;
        j += 2;
        index += 3;
      }
    }

    // generate triangles for the other side
    i += 6;
    j += 6;
    while (j < num_base_vertices - 1 && index < triangles.Length)
    {
      for (int x = 0; x < 3; x++)
      {
        // triangle 1
        triangles[index] = i;
        triangles[index + 1] = j + 1;
        triangles[index + 2] = i + 1;
        index += 3;
        // triangle 2
        triangles[index] = i;
        triangles[index + 1] = j;
        triangles[index + 2] = j + 1;
        i += 2;
        j += 2;
        index += 3;
      }
    }

    // generate mesh for start and end of course
    // start
    int[][] start_endIndices = new int[6][]
    {
      new int[4] { 3, 2, num_base_vertices / 2 + 2, num_base_vertices / 2 + 3 }, // start up
      new int[4]
      {
        num_base_vertices + 1, num_base_vertices, num_base_vertices + 4, num_base_vertices + 5
      }, // start inner
      new int[4]
      {
        num_base_vertices + 3, num_base_vertices + 2, num_base_vertices + 6, num_base_vertices + 7
      }, // start outer
      new int[4]
      {
        num_base_vertices - 3, num_base_vertices - 4, num_base_vertices / 2 - 4,
        num_base_vertices / 2 - 3
      }, // end up
      new int[4]
      {
        num_base_vertices + 13, num_base_vertices + 12, num_base_vertices + 8, num_base_vertices + 9
      }, // end inner
      new int[4]
      {
        num_base_vertices + 15, num_base_vertices + 14, num_base_vertices + 10,
        num_base_vertices + 11
      }, // end outer
    };
    foreach (int[] x in start_endIndices)
    {
      // triangle 1
      triangles[index] = x[0];
      triangles[index + 1] = x[1];
      triangles[index + 2] = x[2];
      index += 3;
      // triangle 2
      triangles[index] = x[0];
      triangles[index + 1] = x[2];
      triangles[index + 2] = x[3];
      index += 3;
    }

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.normals = normals;

    return mesh;
  }

  private void createMeshObject(Mesh mesh, string name, Material mat, PhysicMaterial physMat = null)
  {
    GameObject meshContainer = new GameObject();
    meshContainer.name = name;
    meshContainer.transform.parent = courseObject.transform;

    MeshRenderer meshRenderer = meshContainer.AddComponent<MeshRenderer>();
    meshRenderer.material = mat;

    MeshFilter meshFilter = meshContainer.AddComponent<MeshFilter>();
    meshFilter.mesh = mesh;

    MeshCollider meshCollider = meshContainer.AddComponent<MeshCollider>();
    meshCollider.sharedMesh = mesh;
    physMat ??= new PhysicMaterial();
    physMat.dynamicFriction = 1;
    meshCollider.material = physMat;
  }
}