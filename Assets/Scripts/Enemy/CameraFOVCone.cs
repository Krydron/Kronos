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

    public float downwardTiltAngle = 60f;
    private SecurityCamera securityCamera;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "FOV Mesh";
        GetComponent<MeshFilter>().mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
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

        SetMaterials(securityCamera.idleFOVMaterial, securityCamera.alertFOVMaterial);
    }

    private void LateUpdate()
    {
        if (securityCamera == null) return;

        DrawFanFOV(
            securityCamera.fieldOfView,
            securityCamera.viewDistance,
            securityCamera.downwardTiltAngle
        );
    }

    public void SetMaterials(Material idleMat, Material alertMat)
    {
        idleMaterial = idleMat;
        alertMaterial = alertMat;
        meshRenderer.material = isAlert ? alertMaterial : idleMaterial;
    }

    public void SetAlert(bool alert)
    {
        if (isAlert == alert) return;
        isAlert = alert;
        meshRenderer.material = isAlert ? alertMaterial : idleMaterial;
    }

    private void DrawFanFOV(float horizontalViewAngle, float viewRadius, float tiltAngle)
    {
        mesh.Clear();

        int segments = resolution;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Apex at origin
        vertices[0] = Vector3.zero;

        // Rotation for downward tilt
        Quaternion tiltRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        // Create arc vertices
        for (int i = 0; i <= segments; i++)
        {
            float angle = -horizontalViewAngle / 2f + (horizontalViewAngle * i / segments);
            Quaternion rot = Quaternion.Euler(0f, angle, 0f);
            Vector3 dir = tiltRotation * (rot * Vector3.forward);

            Vector3 worldDir = transform.rotation * dir;
            Vector3 endPoint = transform.position + worldDir * viewRadius;

            // Optional: clip mesh visually with obstacles (comment out to ignore)
            if (Physics.Raycast(transform.position, worldDir, out RaycastHit hit, viewRadius, obstacleMask))
            {
                endPoint = hit.point;
            }

            vertices[i + 1] = transform.InverseTransformPoint(endPoint);
        }

        // Triangles
        for (int i = 0; i < segments; i++)
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
