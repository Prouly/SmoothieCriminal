using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSelector : MonoBehaviour
{
    public static string escenaPendiente;

    public void OnClickPlayJAM(string escenaMinijuego)
    {
        escenaPendiente = escenaMinijuego;
        SceneManager.LoadScene("JAMSelectorMinijuego");
    }
    
    public void OnClickPlayHaznarito(string escenaMinijuego)
    {
        escenaPendiente = escenaMinijuego;
        SceneManager.LoadScene("HaznaritoSelectorMinijuego");
    }
    
    public void OnClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}