using UnityEngine;
using UnityEngine.UIElements;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] bool lookAtPlayer;
    [SerializeField] bool lockX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lookAtPlayer)
        {
            target = GameObject.Find("Main Camera").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) { return; }
        transform.LookAt(target);
        transform.Rotate(0, -180f, 0);
        if (lockX)
        {
            //transform.localRotation = new Quaternion(0, transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);
            //transform.Rotate(-transform.rotation.x,0,0);
            transform.eulerAngles = new Vector3( 0, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        //transform.rotation = Quaternion.Euler(0,transform.rotation.y,0);
    }
}
