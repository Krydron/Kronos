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

using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdatDisplay(int ammo, int rounds)
    {
        text.text = (ammo + " / " + rounds);
    }
}
