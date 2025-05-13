using UnityEngine;

public class Trespass : MonoBehaviour
{

    public GameObject enemy;
    public Transform enemypos;
    private float spawnRate = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // this is just a placeholder and krystian or myself can modify it when enemy work is done.
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            InvokeRepeating("Trespassing", 5f, spawnRate);
            Destroy(gameObject, 11);
            gameObject.GetComponent<BoxCollider>().enabled = false;
                
        }
    }

    public void Trespassing()
    {
        Instantiate(enemy, enemypos.position, enemypos.rotation);
    }

}


