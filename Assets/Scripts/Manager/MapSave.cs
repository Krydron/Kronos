using UnityEngine;

public class MapSave : MonoBehaviour
{
    bool m1, m2, m3;

    private void Start()
    {
        m1 = false;
        m2 = false;
        m3 = false;
    }

    public void MSave(int i, bool value)
    {
        switch (i)
        {
            case 1:
                m1 = value;
                break;
            case 2:
                m2 = value;
                break;
            case 3:
                m3 = value;
                break;
        }
    }

    public bool MSave(int i)
    {
        switch (i)
        {
            case 1:
                return m1;
            case 2:
                return m2;
            case 3:
                return m3;
            default: return false;
        }
    }
}
