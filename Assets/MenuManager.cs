using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject optionsPanel;

    [Header("Butonlar (Main Menu)")]
    public GameObject startButton;
    public GameObject optionsButton;
    public GameObject quitButton;

    public void OpenOptions()
    {
        startButton.SetActive(false);
        optionsButton.SetActive(false);
        quitButton.SetActive(false);

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        startButton.SetActive(true);
        optionsButton.SetActive(true);
        quitButton.SetActive(true);
    }
}
