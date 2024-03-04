using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseSection : MonoBehaviour
{

    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        createQuad(new Vector3(0,0,0),new Vector3(2,0,0),new Vector3(0,0,2),new Vector3(2,0,2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4){
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mat;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]{
            p1, p2, p3, p4
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]{
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
    }

}
