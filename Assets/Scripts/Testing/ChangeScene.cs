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
        Debug.Log("Has seleccionado: " + o);
        
        //Activa el boton de ir a los minijuegos solo si ha seleccionado a uno (no aparece al inicio donde no hay seleccionado ninguno)
        if (botonConfirmar != null) botonConfirmar.gameObject.SetActive(opcion == 0 || opcion == 1);
    }
    
    public void IrAjuego()
    {
        if (opcion == 0)
        {
            Debug.Log("Haznaritooooooooo");
            SceneManager.LoadScene("Random");
        }
        else
        {
            Debug.Log("Jamon");
            SceneManager.LoadScene("Random");
        }
    }
}