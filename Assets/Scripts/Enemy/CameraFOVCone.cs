using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CameraFOVCone : MonoBehaviour
{
    [Range(3, 100)] public int resolution = 50;
    public LayerMask obstacleMask;

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private Material idleMaterial;
    private Material alertMaterial;
    private bool isAlert = false;

    private float downwardTiltAngle = 60f;
    private SecurityCamera securityCamera;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = null; // Reset to prevent override
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

        // Sync position and rotation with camera
        transform.position = securityCamera.transform.position;
        transform.rotation = securityCamera.transform.rotation;

        downwardTiltAngle = securityCamera.downwardTiltAngle;

        DrawFOVCone(
            securityCamera.fieldOfView,
            securityCamera.verticalFieldOfView,
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
            meshRenderer.material = isAlert ? alertMaterial : idleMaterial;
    }

    private void DrawFOVCone(float horizontalViewAngle, float verticalViewAngle, float viewRadius, float tiltAngle)
    {
        mesh.Clear();

        int segments = resolution;
        float halfHorizontalRad = (horizontalViewAngle / 2f) * Mathf.Deg2Rad;
        float halfVerticalRad = (verticalViewAngle / 2f) * Mathf.Deg2Rad;

        // Vertices count:
        // 1 apex + (segments+1) horizontal ring points + (segments+1) vertical ring points (for simplicity, we create a cone with horizontal segments)
        // For a cone, we’ll create a circular base but to represent vertical FOV properly, we use elliptical base scaling

        // Total vertices: apex + base ring + base center
        Vector3[] vertices = new Vector3[segments + 3];
        int[] triangles = new int[segments * 6]; // sides + base

        // Apex vertex
        vertices[0] = Vector3.zero; // cone apex at origin

        // Base center vertex (placed at local forward * viewRadius with tilt)
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        Vector3 baseCenterLocal = tiltRotation * (Vector3.forward * viewRadius);
        vertices[segments + 2] = baseCenterLocal;

        // Calculate base ring vertices (with tilt and obstacle raycast)
        float baseRadiusX = Mathf.Tan(horizontalViewAngle * 0.5f * Mathf.Deg2Rad) * viewRadius;
        float baseRadiusY = Mathf.Tan(verticalViewAngle * 0.5f * Mathf.Deg2Rad) * viewRadius;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            float x = baseRadiusX * Mathf.Cos(angle);
            float y = baseRadiusY * Mathf.Sin(angle);
            Vector3 localPos = new Vector3(x, y, viewRadius);

            localPos = tiltRotation * localPos;

            Vector3 worldDir = transform.rotation * localPos.normalized;
            Vector3 origin = transform.position;
            Vector3 endPoint = origin + worldDir * viewRadius;

            if (Physics.Raycast(origin, worldDir, out RaycastHit hit, viewRadius, obstacleMask))
            {
                endPoint = hit.point;
            }

            vertices[i + 1] = transform.InverseTransformPoint(endPoint);
        }

        // --- Build triangles ---

        // Sides (apex to base ring)
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;           // apex
            triangles[i * 3 + 1] = i + 1;   // current base vertex
            triangles[i * 3 + 2] = i + 2;   // next base vertex
        }

        // Base (base center to base ring), winding order reversed to face outward
        int baseIndexOffset = segments * 3;
        int baseCenterIndex = segments + 2;

        for (int i = 0; i < segments; i++)
        {
            triangles[baseIndexOffset + i * 3] = baseCenterIndex;          // base center vertex
            triangles[baseIndexOffset + i * 3 + 1] = i + 2;                // next base vertex
            triangles[baseIndexOffset + i * 3 + 2] = i + 1;                // current base vertex
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }
}
