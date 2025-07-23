using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    PlayerMovement playerMovement;
    SettimgsTracker gameManager;
    [SerializeField] TextMeshProUGUI textX;
    [SerializeField] TextMeshProUGUI textY;
    [SerializeField] Slider sliderX;
    [SerializeField] Slider sliderY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
        gameManager = GameObject.Find("GameManager").GetComponent<SettimgsTracker>();

        sliderX.value = gameManager.mouseSensitivityX;
        sliderY.value = gameManager.mouseSensitivityY;

        if (playerMovement == null) { return; }
        playerMovement.MouseSensitivityX(gameManager.mouseSensitivityX);
        playerMovement.MouseSensitivityY(gameManager.mouseSensitivityY);
    }

    public void UpdateX(float value)
    {
        gameManager.mouseSensitivityX = value;
        textX.text = value.ToString();
        if (playerMovement == null) { return; }
        playerMovement.MouseSensitivityX(value);
    }

    public void UpdateY(float value)
    {
        gameManager.mouseSensitivityY = value;
        textY.text = value.ToString();
        if (playerMovement == null) { return; };
        playerMovement.MouseSensitivityY(value);
    }
}
