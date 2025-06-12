using UnityEngine;

public class destroywalls : MonoBehaviour
{
    public float hp;
    public GameObject Exploded;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Explosive")
        {
            hp--;
        }
    }

    private void Update()
    {
        if(hp == 0)
        {
            Instantiate(Exploded, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
