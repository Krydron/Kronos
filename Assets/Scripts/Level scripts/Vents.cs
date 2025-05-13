using Unity.VisualScripting;
using UnityEngine;

public class Vents : MonoBehaviour
{
    public Transform player, destination;
    public GameObject playerg;


    void OnTriggerEnter(Collider other)
    {
     if(other.CompareTag("Player"))
        {
            playerg.SetActive(false);
            player.position = destination.position;
            playerg.SetActive(true);
        }
    }
}
