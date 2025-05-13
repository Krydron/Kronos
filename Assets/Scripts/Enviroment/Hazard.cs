using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] float hurtAmmout;

    void HurtPlayer(GameObject player)
    {
        if (player.tag == "Player")
        {
            player.GetComponent<PlayerHealth>().DecrementHealth(hurtAmmout);
        }
    }

    //When the player touches the hazard they get hurt
    private void OnCollisionEnter(Collision collision)
    {
        HurtPlayer(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        HurtPlayer(collision.gameObject);
    }

    //Adding support for triggers
    private void OnTriggerEnter(Collider other)
    {
        HurtPlayer(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        HurtPlayer(other.gameObject);
    }
}
