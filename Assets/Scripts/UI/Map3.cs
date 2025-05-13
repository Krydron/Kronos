using UnityEngine;

public class Map3 : MonoBehaviour
{
    static GameObject instance;
    void Start()
    {
        if (instance == null)
        {
            instance = this.gameObject;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Object.DontDestroyOnLoad(gameObject);
    }
}
