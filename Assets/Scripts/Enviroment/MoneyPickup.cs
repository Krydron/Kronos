using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [SerializeField] int value;

    private void Money(GameObject gameObject)
    {
        Debug.Log("Money");
        if (gameObject.transform.CompareTag("Player"))
        {
            //gameObject.GetComponent<Inventory>().IncreaseMoney(value);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Money(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        Money(collision.gameObject);
    }

    //Adding support for triggers
    private void OnTriggerEnter(Collider other)
    {
        Money(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        Money(other.gameObject);
    }
}
