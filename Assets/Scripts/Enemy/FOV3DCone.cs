using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FOV3DCone : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public float height = 2f;
    public int segments = 30;

    private Mesh mesh;

    void Start()
    {
        GenerateConeMesh();
    }

    void GenerateConeMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Tip of the cone (apex)
        Vector3 apex = Vector3.zero;
        vertices.Add(apex);

        float angleStep = viewAngle / segments;

        // Base circle of the cone (in an arc, not full circle)
        for (int i = 0; i <= segments; i++)
        {
            float angle = -viewAngle / 2 + angleStep * i;
            float rad = Mathf.Deg2Rad * angle;

            float x = Mathf.Sin(rad) * viewRadius;
            float z = Mathf.Cos(rad) * viewRadius;

            vertices.Add(new Vector3(x, -height, z)); // push down to form sloped sides
        }

        // Triangles (sides)
        for (int i = 1; i <= segments; i++)
        {
            triangles.Add(0);       // apex
            triangles.Add(i);       // current
            triangles.Add(i + 1);   // next
        }

        // Optionally add base cap
        int baseStartIndex = vertices.Count;
        vertices.Add(Vector3.down * height); // center of base

        for (int i = 1; i <= segments; i++)
        {
            vertices.Add(vertices[i + 0]); // duplicate rim vertices for base
        }

        for (int i = 0; i < segments; i++)
        {
            int center = baseStartIndex;
            int current = baseStartIndex + 1 + i;
            int next = baseStartIndex + 1 + (i + 1) % segments;

            triangles.Add(center);
            triangles.Add(next);
            triangles.Add(current);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
