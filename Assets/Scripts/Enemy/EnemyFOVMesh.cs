using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyFOVMesh : MonoBehaviour
{
    [Header("FOV Settings")]
    public float sightRange = 10f;
    [Range(10, 360)] public float fieldOfView = 90f;
    public int rayCount = 50;
    public LayerMask obstacleMask;

    [Header("Scan Settings")]
    public float scanSpeed = 30f; // degrees per second
    public Color baseColor = new Color(1f, 1f, 0f, 0.15f);
    public Color scanColor = new Color(1f, 0f, 0f, 0.6f);
    public float scanWidth = 2f; // degrees

    [Header("Eye Settings")]
    public float eyeHeight = 1.6f;
    [Range(-90f, 90f)]
    public float tiltAngle = -30f;  // Negative = looking down

    private Mesh mesh;
    private Material fovMaterial;

    private float currentAngle;
    private float scanDirection = 1f; // 1 = right, -1 = left

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        fovMaterial = new Material(Shader.Find("Custom/RadarFOV"));
        GetComponent<MeshRenderer>().material = fovMaterial;

        fovMaterial.SetFloat("_FOVAngle", fieldOfView);
        fovMaterial.SetFloat("_Range", sightRange);
        fovMaterial.SetFloat("_ScanWidth", scanWidth);
        fovMaterial.SetColor("_BaseColor", baseColor);
        fovMaterial.SetColor("_ScanColor", scanColor);

        // Start scanning from left side
        currentAngle = -fieldOfView * 0.5f;
    }

    void LateUpdate()
    {
        DrawFOVMesh();

        // Ping-pong scan logic
        currentAngle += scanDirection * scanSpeed * Time.deltaTime;

        if (currentAngle > fieldOfView * 0.5f)
        {
            currentAngle = fieldOfView * 0.5f;
            scanDirection = -1f;
        }
        else if (currentAngle < -fieldOfView * 0.5f)
        {
            currentAngle = -fieldOfView * 0.5f;
            scanDirection = 1f;
        }

        // Calculate tilted forward vector
        Vector3 tiltedForward = Quaternion.Euler(tiltAngle, 0f, 0f) * transform.forward;

        // Pass dynamic properties to shader
        fovMaterial.SetVector("_Origin", transform.position + Vector3.up * eyeHeight);
        fovMaterial.SetVector("_ForwardDir", tiltedForward);
        fovMaterial.SetFloat("_CurrentAngle", currentAngle);
    }

    void DrawFOVMesh()
    {
        float angleStep = fieldOfView / rayCount;
        float startAngle = -fieldOfView / 2f;

        List<Vector3> viewPoints = new List<Vector3> { Vector3.zero };
        Vector3 origin = transform.position + Vector3.up * eyeHeight;

        for (int i = 0; i <= rayCount; i++)
        {
            float yawAngle = startAngle + angleStep * i;
            Vector3 dir = DirFromAngles(yawAngle, tiltAngle);

            RaycastHit hit;
            Vector3 point;
            if (Physics.Raycast(origin, dir, out hit, sightRange, obstacleMask))
                point = transform.InverseTransformPoint(hit.point);
            else
                point = transform.InverseTransformPoint(origin + dir * sightRange);

            viewPoints.Add(point);
        }

        CreateMesh(viewPoints);
    }

    void CreateMesh(List<Vector3> points)
    {
        mesh.Clear();
        mesh.vertices = points.ToArray();

        int[] triangles = new int[(points.Count - 2) * 3];
        for (int i = 0; i < points.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    Vector3 DirFromAngles(float yawDegrees, float pitchDegrees)
    {
        Quaternion rotation = Quaternion.Euler(pitchDegrees, yawDegrees + transform.eulerAngles.y, 0);
        return rotation * Vector3.forward;
    }
}
