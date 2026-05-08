using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSelector : MonoBehaviour
{
    public static string escenaPendiente;

    public void OnClickPlay(string escenaMinijuego)
    {
        escenaPendiente = escenaMinijuego;
        SceneManager.LoadScene("GameManagerSelectorMinijuego");
    }
    
    public void OnClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}