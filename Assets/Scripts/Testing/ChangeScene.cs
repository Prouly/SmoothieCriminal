using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luismi Muñoz
 * Descripción: Carga la scene Random
 * Última modificación: 14/04/2026
 */
public class ChangeScene : MonoBehaviour
{

    [SerializeField] private int opcion = -1;
    
    [SerializeField] private Button botonConfirmar;
    
    public void SeleccionarOpcion(int o)
    {
        opcion = o;
        if (o == 0)
        {
            Debug.Log("Has seleccionado el personaje Haznarito");
        }
        else if (o == 1)
        {
            Debug.Log("Has seleccionado el personaje J.A.M.");
        }
        else
        {
            Debug.Log("No has seleccionado ningún personaje");
        }
        
        // Activa el boton de ir a los minijuegos solo si ha seleccionado a uno (no aparece al inicio donde no hay seleccionado ninguno)
        if (botonConfirmar != null) botonConfirmar.gameObject.SetActive(opcion == 0 || opcion == 1);
    }
    
    public void IrAjuego()
    {
        if (opcion == 0)
        {
            Debug.Log("Cargando Scene Haznarito");
            SceneManager.LoadScene("RandomHaznarito");
        }
        else
        {
            Debug.Log("Cargando Scene Haznarito JAM");
            SceneManager.LoadScene("RandomJAM");
        }
    }
}