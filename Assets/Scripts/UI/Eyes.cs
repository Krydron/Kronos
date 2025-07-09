/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Owen Clifton> 
* Date: <need to add>
*
***************************************************************************************************************/

using System.Collections;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    GameObject eyelidUp, eyelidDown;
    float startUpY = 810;
    float startDownY = 270;
    float endUpY = 1350;
    float endDownY = -270;
    //float xOffset = 960;
    //float yOffset = 490;

    [SerializeField] ushort delay;
    [SerializeField] float speed;
    [SerializeField] float animationFrameDelay;

    IEnumerator OpenEyes()
    {
        eyelidUp.transform.position = new Vector3(960, startUpY , 0);
        eyelidDown.transform.position = new Vector3(960, startDownY, 0);
        float timer = 0;
        yield return new WaitForSeconds(delay);
        while (eyelidUp.transform.position.y < endUpY)
        {
            timer = (timer + Time.deltaTime * speed) % 1.1f;
            eyelidUp.transform.position = new Vector3(960, Mathf.Lerp(startUpY, endUpY, timer), 0);
            eyelidDown.transform.position = new Vector3(960, Mathf.Lerp(startDownY, endDownY, timer), 0);
            yield return new WaitForSeconds(animationFrameDelay);
        }
    }

    IEnumerator CloseEyes()
    {
        float timer = 0;
        while (eyelidUp.transform.position.y > startUpY)
        {
            timer = (timer + Time.deltaTime * speed) % 1.1f;
            eyelidUp.transform.position = new Vector3(960, Mathf.Lerp(endUpY, startUpY, timer), 0);
            eyelidDown.transform.position = new Vector3(960, Mathf.Lerp(endDownY, startDownY, timer), 0);
            yield return new WaitForSeconds(animationFrameDelay);
        }
    }

    public void ShutEyes()
    {
        transform.Find("Eye").GetComponent<Animator>().SetBool("Open",false);
        //StartCoroutine(CloseEyes());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eyelidUp = transform.Find("Eyelid_Up").gameObject;
        eyelidDown = transform.Find("Eyelid_Down").gameObject;
        //StartCoroutine(OpenEyes());
    }

    // Update is called once per frame
    void Update()
    {
    }
}
