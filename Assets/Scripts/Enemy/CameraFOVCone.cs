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

    private void DrawFOVCone(float viewAngle, float viewRadius, float tiltAngle)
    {
        mesh.Clear();

        float halfAngleRad = (viewAngle / 2f) * Mathf.Deg2Rad;
        float baseRadius = Mathf.Tan(halfAngleRad) * viewRadius;

        Vector3[] vertices = new Vector3[resolution + 2];
        int[] triangles = new int[resolution * 3];

        vertices[0] = Vector3.zero; // apex of the cone

        float angleStep = 360f / resolution;
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
        Vector3 origin = transform.position;

        for (int i = 0; i <= resolution; i++)
        {
            float currentAngle = i * angleStep;
            float x = baseRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float y = baseRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

            Vector3 localDirection = new Vector3(x, y, viewRadius).normalized;
            Vector3 worldDirection = tiltRotation * localDirection;

            Vector3 endPoint = origin + transform.rotation * worldDirection * viewRadius;

            if (Physics.Raycast(origin, transform.rotation * worldDirection, out RaycastHit hit, viewRadius, obstacleMask))
            {
                endPoint = hit.point;
            }

            // Convert world point to local space relative to the FOV GameObject
            vertices[i + 1] = transform.InverseTransformPoint(endPoint);

            // Optional debug
            Debug.DrawLine(origin, endPoint, isAlert ? Color.red : Color.green, 0.05f);
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
