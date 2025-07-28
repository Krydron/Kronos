using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    GameObject map1,map2,map3, button1, button2, button3;
    //List<GameObject> buttons = new List<GameObject>();
    MapSave mapSave;

    private void Awake()
    {
        map1 = transform.Find("Map1").gameObject;
        map2 = transform.Find("Map2").gameObject;
        map3 = transform.Find("Map3").gameObject;
        map1.SetActive(false);
        map2.SetActive(false);
        map3.SetActive(false);

        button1 = transform.Find("1").gameObject;
        button2 = transform.Find("2").gameObject;
        button3 = transform.Find("3").gameObject;
    }

    private void OnEnable()
    {
        mapSave = GameObject.Find("GameManager")?.GetComponent<MapSave>();
        if (mapSave == null ) { return; }
        for (int i = 1; i < 4; i++)
        {
            switch(i)
            {
                case 1:
                    button1.SetActive(mapSave.MSave(i));
                    break;
                case 2:
                    button2.SetActive(mapSave.MSave(i));
                    break;
                case 3:
                    button3.SetActive(mapSave.MSave(i));
                    break;
                default: break;
            }
        }
    }

    private void HideAll()
    {
        map1.SetActive(false);
        map2.SetActive(false);
        map3.SetActive(false);
    }

    public void MapLoad(int num)
    {
        HideAll();
        switch (num) {
            case 1:
                map1.SetActive(true);
                break;
            case 2:
                map2.SetActive(true);
                break;
            case 3:
                map3.SetActive(true);
                break;
            default: break;
        }
        
    }

    public void Button1() 
    {
        mapSave.MSave(1, true);
        //button1.SetActive(true);
        Debug.Log("Map1 Added");
    }
    public void Button2() 
    {
        mapSave.MSave(2, true);
        //button2.SetActive(true);
        Debug.Log("Map2 Added");
    }
    public void Button3() 
    {
        mapSave.MSave(3, true);
        //button3.SetActive(true);
        Debug.Log("Map3 Added");
    }
}
