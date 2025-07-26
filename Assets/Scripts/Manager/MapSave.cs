using UnityEngine;

public class MapSave : MonoBehaviour
{
    bool[] mapButtonsState;
    int mapAmount = (int)MapType.Length;
    public Vector3[][][] list;

    private void Start()
    {
        mapButtonsState = new bool[mapAmount];
        for (int i = 0; i < mapButtonsState.Length; i++)
        {
            mapButtonsState[i] = false;
        }
        list = new Vector3[mapAmount][][];
    }

    public void MSave(int i, bool value)
    {
        i = Mathf.Clamp(i, 1, mapAmount);
        mapButtonsState[i-1] = value;
    }

    public bool MSave(int i)
    {
        if (mapButtonsState == null) { return false; }
        i = Mathf.Clamp(i, 1, mapAmount);
        return mapButtonsState[i-1];
        //return false;
    }

    public bool[] MSave()
    {
        return mapButtonsState;
    }

    public void MSave(bool[] value)
    {
        mapButtonsState = value;
    }

    public void ResetValues()
    {
        for (int i = 0; i < mapButtonsState.Length; i++)
        {
            mapButtonsState[i] = false;
        }
        list = new Vector3[mapAmount][][];
    }
}
