using UnityEngine;
using UnityEngine.UIElements;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.Rotate(0, -180f, 0);
        //transform.rotation = Quaternion.Euler(0,transform.rotation.y,0);
    }
}
