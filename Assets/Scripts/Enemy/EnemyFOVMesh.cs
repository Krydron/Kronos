using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyFOVMesh : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public int rayCount = 50;
    public LayerMask obstacleMask;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate()
    {
        DrawFOV();
    }

    void DrawFOV()
    {
        float angleStep = viewAngle / rayCount;
        float angle = -viewAngle / 2f;

        List<Vector3> viewPoints = new List<Vector3>();
        viewPoints.Add(Vector3.zero); // origin (local)

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = DirFromAngle(angle, true);
            Vector3 worldOrigin = transform.position;
            RaycastHit hit;

            Vector3 hitPoint;
            if (Physics.Raycast(worldOrigin, dir, out hit, viewRadius, obstacleMask))
            {
                hitPoint = transform.InverseTransformPoint(hit.point); // convert to local space
            }
            else
            {
                hitPoint = transform.InverseTransformPoint(worldOrigin + dir * viewRadius);
            }

            viewPoints.Add(hitPoint);
            angle += angleStep;
        }

        CreateMesh(viewPoints);
    }

    void CreateMesh(List<Vector3> points)
    {
        mesh.Clear();

        vertices = points.ToArray();
        triangles = new int[(points.Count - 2) * 3];

        for (int i = 0; i < points.Count - 2; i++)
        {
            triangles[i * 3] = 0;         // origin
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Angle in degrees
    Vector3 DirFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
