using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CameraFOVCone : MonoBehaviour
{
    [Range(3, 100)] public int resolution = 50;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private Material idleMaterial;
    private Material alertMaterial;

    private bool isAlert = false;

    private float downwardTiltAngle = 60f; // default tilt, set from SecurityCamera

    private SecurityCamera securityCamera;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();

        // Force reset the material to avoid editor overrides
        meshRenderer.material = null;
    }


    private void Start()
    {
        securityCamera = GetComponentInParent<SecurityCamera>();
        if (securityCamera == null)
        {
            Debug.LogError("CameraFOVCone: No parent SecurityCamera found!");
            enabled = false;
            return;
        }
        downwardTiltAngle = securityCamera.downwardTiltAngle;
        SetMaterials(idleMaterial, alertMaterial);
    }

    private void LateUpdate()
    {
        if (securityCamera == null) return;

        downwardTiltAngle = securityCamera.downwardTiltAngle;

        DrawFOVCone(
            securityCamera.fieldOfView,
            securityCamera.viewDistance,
            downwardTiltAngle
        );
    }

    public void SetMaterials(Material idleMat, Material alertMat)
    {
        idleMaterial = idleMat;
        alertMaterial = alertMat;
        if (meshRenderer != null)
            meshRenderer.material = isAlert ? alertMaterial : idleMaterial;
    }

    public void SetAlert(bool alert)
    {
        if (isAlert == alert) return;

        isAlert = alert;
        if (meshRenderer != null)
        {
            meshRenderer.material = isAlert ? alertMaterial : idleMaterial;
        }
    }

    private void DrawFOVCone(float viewAngle, float viewRadius, float tiltAngle)
    {
        mesh.Clear();

        viewAngle = Mathf.Clamp(viewAngle, 1f, 179f);
        viewRadius = Mathf.Max(viewRadius, 0.1f);

        // Calculate base radius of the cone's circular base
        float halfAngleRad = (viewAngle / 2f) * Mathf.Deg2Rad;
        float baseRadius = Mathf.Tan(halfAngleRad) * viewRadius;

        Vector3[] vertices = new Vector3[resolution + 2];
        int[] triangles = new int[resolution * 3];

        // Apex vertex at origin
        vertices[0] = Vector3.zero;

        float angleStep = 360f / resolution;

        // Rotate the entire cone downward by tiltAngle degrees around X axis
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        for (int i = 0; i <= resolution; i++)
        {
            float currentAngle = i * angleStep;
            float x = baseRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float y = baseRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

            // Vertex on base circle, initially pointing forward (z)
            Vector3 vertexPos = new Vector3(x, y, viewRadius);

            // Apply tilt so cone points downward by tiltAngle degrees
            vertices[i + 1] = tiltRotation * vertexPos;
        }

        for (int i = 0; i < resolution; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
