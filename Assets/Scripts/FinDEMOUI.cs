using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinDEMOUI : MonoBehaviour
{
    public TextMeshProUGUI textoPuntosFinales;

    void Start()
    {
        if (GameManager.instancia != null)
        {
            textoPuntosFinales.text = "Puntos: " + GameManager.instancia.puntosFinales;
        }
    }
    
    public void Volver()
    {
        SceneManager.LoadScene("Main");
    }
}