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

    [Header("Air Culling Settings")]
    public bool enableAirCulling = true;
    public float groundCheckAbove = 0.2f;   // how far above to start the downward check
    public float groundCheckDistance = 2f;  // how far down to look for ground
    public float edgePullback = 0.5f;       // pullback distance if over air

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

        Vector3 origin = transform.position + Vector3.up * eyeHeight;

        List<Vector3> viewPoints = new List<Vector3>();
        List<int> validIndices = new List<int>(); // track valid vertices for mesh

        for (int i = 0; i <= rayCount; i++)
        {
            float yawAngle = startAngle + angleStep * i;
            Vector3 dir = DirFromAngles(yawAngle, tiltAngle);

            RaycastHit hit;
            Vector3 point;

            if (Physics.Raycast(origin, dir, out hit, sightRange, obstacleMask))
            {
                point = hit.point;
            }
            else
            {
                point = origin + dir * sightRange;
            }

            bool isValid = true;

            if (enableAirCulling)
            {
                RaycastHit groundHit;
                if (Physics.Raycast(point + Vector3.up * groundCheckAbove, Vector3.down,
                    out groundHit, groundCheckDistance, obstacleMask))
                {
                    point = groundHit.point;
                }
                else
                {
                    // No ground below --- mark invalid to create gap
                    isValid = false;
                }
            }

            if (isValid)
            {
                viewPoints.Add(transform.InverseTransformPoint(point));
                validIndices.Add(viewPoints.Count - 1);
            }
            else
            {
                // Add a placeholder point to keep indexing consistent
                viewPoints.Add(Vector3.zero);
            }
        }

        CreateMeshWithGaps(viewPoints, validIndices);
    }

    void CreateMeshWithGaps(List<Vector3> points, List<int> validIndices)
    {
        mesh.Clear();

        // Add origin vertex at index 0
        List<Vector3> vertices = new List<Vector3> { Vector3.zero };

        // We'll build triangles between origin and consecutive valid points,
        // skipping any invalid points to create gaps.
        List<int> triangles = new List<int>();

        // Map original viewPoints indices to new vertices indices (skip invalids)
        Dictionary<int, int> indexMap = new Dictionary<int, int>();

        // Start from 1 because 0 is origin
        int newIndex = 1;
        foreach (int i in validIndices)
        {
            vertices.Add(points[i]);
            indexMap[i] = newIndex;
            newIndex++;
        }

        // Create triangles between origin and pairs of consecutive valid points
        for (int i = 0; i < validIndices.Count - 1; i++)
        {
            int current = validIndices[i];
            int next = validIndices[i + 1];

            // Only connect if these indices are consecutive in the original list
            if (next == current + 1)
            {
                triangles.Add(0); // origin vertex
                triangles.Add(indexMap[current]);
                triangles.Add(indexMap[next]);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    Vector3 DirFromAngles(float yawDegrees, float pitchDegrees)
    {
        Quaternion rotation = Quaternion.Euler(pitchDegrees, yawDegrees + transform.eulerAngles.y, 0);
        return rotation * Vector3.forward;
    }
}
