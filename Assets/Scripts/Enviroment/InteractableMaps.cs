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

using UnityEngine;

public class InteractableMaps : MonoBehaviour
{
    Map map;
    [SerializeField] private int mapType;

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //map = GameObject.Find("Map").GetComponent<Map>();
    }

    private void OnLevelWasLoaded(int level)
    {
        map = GameObject.Find("Map").GetComponent<Map>();
    }

    public bool Interect()
    {
        if (map == null) { return false; }
        Debug.Log("Saving Map");
        switch (mapType)
        {
            case 0:
                map.Button1();
                break;
            case 1:
                map.Button2();
                break;
            case 2:
                map.Button3();
                break;
        }
        return true;
    }
}
