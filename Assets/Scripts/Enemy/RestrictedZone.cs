using UnityEngine;

public class RestrictedZone : MonoBehaviour
{
    public static RestrictedZone[] AllZones => FindObjectsOfType<RestrictedZone>();

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player entered restricted zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Player left restricted zone.");
        }
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}
