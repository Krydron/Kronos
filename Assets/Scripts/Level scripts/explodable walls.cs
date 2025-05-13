using UnityEngine;

public class explodablewalls : MonoBehaviour
{

    public int wallPerAxis = 8;
    public float delay = 1f;
    public float force = 300;
    public float radius = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Update", delay);
    }

    // Update is called once per frame
    void Update()
    {
        for(int x = 0; x < wallPerAxis; x++)
        {
            for (int y = 0; y < wallPerAxis; y++)
            {
                for (int z = 0; z < wallPerAxis; z++)
                {
                    Createwall(new Vector3(x, y, z));
                }
            }
        }
        Destroy(gameObject);
    }

    void Createwall(Vector3 coordinates)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Renderer rd = cube.GetComponent<Renderer>();
        rd.material = GetComponent<Renderer>().material;
        cube.transform.localScale = transform.localScale / wallPerAxis;
        Vector3 firstCube = transform.position - transform.localScale / 2 + cube.transform.localScale;
        cube.transform.position = firstCube + Vector3.Scale(coordinates, cube.transform.localScale);
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.AddExplosionForce(force, transform.position, radius);
    }
}
