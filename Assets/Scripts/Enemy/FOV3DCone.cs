using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FOV3DCone : MonoBehaviour
{
    [Header("FOV Settings")]
    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public float height = 2f;
    public int segments = 30;

    [Header("Obstacle Detection")]
    public LayerMask obstacleMask;

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate()
    {
        GenerateConeMesh();
    }

    void GenerateConeMesh()
    {
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 apex = Vector3.zero;
        vertices.Add(apex); // index 0

        float angleStep = viewAngle / segments;

        // Generate cone arc via raycasts
        for (int i = 0; i <= segments; i++)
        {
            float angle = -viewAngle / 2 + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 origin = transform.position;

            Vector3 endPoint = origin + dir * viewRadius;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, viewRadius, obstacleMask))
            {
                endPoint = hit.point;
            }

            Vector3 localPoint = transform.InverseTransformPoint(endPoint);
            localPoint.y = -height; // push down to make sloped sides
            vertices.Add(localPoint);
        }

        // Side triangles (from apex to each arc segment)
        for (int i = 1; i <= segments; i++)
        {
            triangles.Add(0);       // apex
            triangles.Add(i);       // current
            triangles.Add(i + 1);   // next
        }

        // Optional base cap
        int baseCenterIndex = vertices.Count;
        vertices.Add(Vector3.down * height); // center of base

        for (int i = 1; i <= segments; i++)
        {
            vertices.Add(vertices[i]); // reuse rim vertices
        }

        for (int i = 0; i < segments; i++)
        {
            int center = baseCenterIndex;
            int current = baseCenterIndex + 1 + i;
            int next = baseCenterIndex + 1 + (i + 1) % segments;

            triangles.Add(center);
            triangles.Add(next);
            triangles.Add(current);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
