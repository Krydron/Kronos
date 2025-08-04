using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyFOVMesh : MonoBehaviour
{
    [Header("FOV Settings")]
    public float sightRange = 10f;
    public float fieldOfView = 90f;
    public int rayCount = 50;
    public LayerMask obstacleMask;

    [Header("Ground Culling Settings")]
    public LayerMask groundMask;
    public float groundCheckDistance = 2f;

    [Header("Colors")]
    public Color fovColor = new Color(1f, 1f, 0f, 0.15f);
    public Color scanLineColor = new Color(1f, 1f, 0.5f, 0.8f);

    [Header("Tilt Settings")]
    public bool autoTilt = true;
    public float eyeHeight = 1.6f;
    private float tiltAngle = 0f;

    [Header("Scan Line Settings")]
    public float scanSpeed = 1f;

    private Mesh mesh;
    private Material fovMaterial;
    private Material lineMaterial;

    private float scanPosition = 0f;
    private int scanDirection = 1;

    // Store visible ray endpoints for the scan line
    private List<Vector3> visibleRayEnds = new List<Vector3>();

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        fovMaterial = new Material(Shader.Find("Custom/FOVTransparent"));
        fovMaterial.color = fovColor;
        GetComponent<MeshRenderer>().material = fovMaterial;

        lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = scanLineColor;
        lineMaterial.renderQueue = 3100;
    }

    void LateUpdate()
    {
        if (autoTilt)
        {
            float fovFactor = Mathf.Clamp01(90f / fieldOfView);
            float baseTilt = Mathf.Atan(eyeHeight / sightRange) * Mathf.Rad2Deg;
            tiltAngle = -baseTilt * fovFactor;
        }

        DrawFOV(sightRange, fieldOfView);

        scanPosition += scanDirection * scanSpeed * Time.deltaTime;
        if (scanPosition >= 1f) { scanPosition = 1f; scanDirection = -1; }
        if (scanPosition <= 0f) { scanPosition = 0f; scanDirection = 1; }

        if (fovMaterial != null)
            fovMaterial.SetFloat("_ScanPos", scanPosition);
    }

    void OnRenderObject()
    {
        if (!lineMaterial || visibleRayEnds.Count == 0) return;
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Color(scanLineColor);

        // Determine which ray segment corresponds to the scan position
        int scanIndex = Mathf.RoundToInt(scanPosition * (rayCount - 1));

        if (scanIndex >= 0 && scanIndex < visibleRayEnds.Count)
        {
            Vector3 start = Vector3.zero; // center
            Vector3 end = visibleRayEnds[scanIndex];
            if (end != Vector3.zero) // only draw if this segment is visible
            {
                GL.Vertex(start);
                GL.Vertex(end);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    void DrawFOV(float viewRadius, float viewAngle)
    {
        float angleStep = viewAngle / rayCount;
        float startAngle = -viewAngle / 2f;
        float angle = startAngle;

        List<Vector3> viewPoints = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        visibleRayEnds.Clear();

        viewPoints.Add(Vector3.zero);
        uvs.Add(new Vector2(0.5f, 0));

        Vector3 origin = transform.position;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = DirFromAngles(angle, tiltAngle, false);
            RaycastHit hit;
            Vector3 hitPointWorld;

            if (Physics.Raycast(origin, dir, out hit, viewRadius, obstacleMask))
                hitPointWorld = hit.point;
            else
                hitPointWorld = origin + dir * viewRadius;

            bool hasGround = Physics.Raycast(
                hitPointWorld + Vector3.up * 0.5f,
                Vector3.down,
                groundCheckDistance + 0.5f,
                groundMask
            );

            if (!hasGround)
            {
                hitPointWorld = origin;
                visibleRayEnds.Add(Vector3.zero); // mark invisible
            }
            else
            {
                visibleRayEnds.Add(transform.InverseTransformPoint(hitPointWorld));
            }

            Vector3 localPoint = transform.InverseTransformPoint(hitPointWorld);
            viewPoints.Add(localPoint);

            float normalizedAngle = (angle - startAngle) / viewAngle;
            uvs.Add(new Vector2(normalizedAngle, 0));

            angle += angleStep;
        }

        CreateMesh(viewPoints, uvs);
    }

    void CreateMesh(List<Vector3> points, List<Vector2> uvs)
    {
        mesh.Clear();
        mesh.vertices = points.ToArray();
        mesh.uv = uvs.ToArray();

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
