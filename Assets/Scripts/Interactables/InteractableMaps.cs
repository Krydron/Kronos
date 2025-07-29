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

public class InteractableMaps : Interactable
{
    [SerializeField] Map map;
    [SerializeField] private int mapType;

    public override bool Interact()
    {
        if (map == null) { Debug.LogError("Map is null"); return false; }
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
