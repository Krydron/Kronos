using UnityEngine;

public class Win : MonoBehaviour
{
    GameObject gameManager, player;
    [SerializeField] float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        gameManager = GameObject.Find("GameManager");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public bool Interact()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= distance)
        {
            gameManager.GetComponent<SceneLoader>().Win();
            return true;
        }
        return false;
    }
}

    
