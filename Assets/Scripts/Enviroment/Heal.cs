using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] float healAmmout;

    void HealPlayer(GameObject player)
    {
        if (player.tag == "Player")
        {
            player.GetComponent<PlayerHealth>().IncrementHealth(healAmmout);
        }
    }

    //When the player touches the heal item they heal
    private void OnCollisionEnter(Collision collision)
    {
        HealPlayer(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        HealPlayer(collision.gameObject);
    }

    //Adding support for triggers
    private void OnTriggerEnter(Collider other)
    {
        HealPlayer(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        HealPlayer(other.gameObject);
    }
}
