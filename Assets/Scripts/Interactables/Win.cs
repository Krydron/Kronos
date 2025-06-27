using UnityEngine;

public class Win : Interactable
{
    GameObject gameManager, player;
    //[SerializeField] float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        gameManager = GameObject.Find("GameManager");
        //player = GameObject.FindGameObjectWithTag("Player");
    }

    public override bool Interact()
    {
        gameManager.GetComponent<SceneLoader>().Win();
        return true;
    }
}

    
