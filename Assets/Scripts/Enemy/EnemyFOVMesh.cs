using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyFOVMesh : MonoBehaviour
{
    [Header("FOV Settings (Independent)")]
    public float sightRange = 10f;     // Max distance for FOV mesh
    public float fieldOfView = 90f;    // Horizontal angle of the FOV cone
    public int rayCount = 50;
    public LayerMask obstacleMask;     // Layers considered obstacles

    [Header("Highlight Settings")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.15f);     // Base transparent color
    public Color highlightColor = new Color(1f, 1f, 0f, 0.8f);   // Highlight color at collisions

    [Header("Tilt Settings")]
    public float tiltAngle = 0f; // Manual tilt (deg, negative = down)

    [Header("Auto Tilt Settings")]
    public bool autoTilt = true;
    public float eyeHeight = 1.6f;  // Height of eyes relative to ground

    private Mesh mesh;
    private Color[] colors;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void LateUpdate()
    {
        if (autoTilt)
        {
            // Calculate tilt so cone points downward, adjusted for fieldOfView width
            // Wider FOV cones tend to look "flatter" in tilt,
            // so scale tilt by a factor based on horizontal FOV angle
            // The more narrow the FOV, the steeper the tilt (up to eyeHeight limit)
            float fovFactor = Mathf.Clamp01(90f / fieldOfView); // 1 when 90°, >1 if less, <1 if more
            float baseTilt = Mathf.Atan(eyeHeight / sightRange) * Mathf.Rad2Deg;
            tiltAngle = -baseTilt * fovFactor;
        }

        DrawFOV(sightRange, fieldOfView);
    }

    void DrawFOV(float viewRadius, float viewAngle)
    {
        float angleStep = viewAngle / rayCount;
        float angle = -viewAngle / 2f;

        List<Vector3> viewPoints = new List<Vector3>();
        List<bool> highlightPoints = new List<bool>();

        viewPoints.Add(Vector3.zero);
        highlightPoints.Add(false);

        Vector3 origin = transform.position;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = DirFromAngles(angle, tiltAngle, false);

            RaycastHit hit;
            Vector3 hitPointWorld;
            bool highlight = false;

            if (Physics.Raycast(origin, dir, out hit, viewRadius, obstacleMask))
            {
                hitPointWorld = hit.point;
                highlight = true; // Collision detected — highlight vertex
            }
            else
            {
                hitPointWorld = origin + dir * viewRadius;
            }

            viewPoints.Add(transform.InverseTransformPoint(hitPointWorld));
            highlightPoints.Add(highlight);

            angle += angleStep;
        }

        CreateMesh(viewPoints, highlightPoints);
    }

    void CreateMesh(List<Vector3> points, List<bool> highlightPoints)
    {
        mesh.Clear();

        Vector3[] vertices = points.ToArray();
        int[] triangles = new int[(points.Count - 2) * 3];
        colors = new Color[vertices.Length];

        for (int i = 0; i < points.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            if (highlightPoints[i])
            {
                // Brightness increase for collision points
                // Mix base color with highlightColor, full highlight at collision
                colors[i] = highlightColor;
            }
            else
            {
                colors[i] = normalColor;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    // Direction vector from yaw (horizontal) and pitch (vertical) angles
    Vector3 DirFromAngles(float yawAngleDeg, float pitchAngleDeg, bool global)
    {
        float yawRad = yawAngleDeg * Mathf.Deg2Rad;
        float pitchRad = pitchAngleDeg * Mathf.Deg2Rad;

        float x = Mathf.Sin(yawRad) * Mathf.Cos(pitchRad);
        float y = Mathf.Sin(pitchRad);
        float z = Mathf.Cos(yawRad) * Mathf.Cos(pitchRad);

        Vector3 dir = new Vector3(x, y, z);

        if (!global)
            dir = Quaternion.Euler(0, transform.eulerAngles.y, 0) * dir;

        return dir.normalized;
    }
}
