using UnityEngine;

public class TurretTrespassZone : MonoBehaviour
{
    private Turret turret;  // Reference to the turret

    private void Start()
    {
        turret = GetComponentInParent<Turret>();  // Find the turret in the parent
        if (turret == null)
        {
            Debug.LogError("TurretTrespassZone: No Turret found in parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && turret != null)
        {
            turret.SetTarget(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && turret != null)
        {
            turret.ClearTarget();
        }
    }
}
