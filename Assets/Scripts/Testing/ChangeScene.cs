using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luismi Muñoz
 * Descripción: Carga la scene Random
 * Última modificación: 14/04/2026
 */
public class ChangeScene : MonoBehaviour
{
    public void IrAEscena()
    {
        SceneManager.LoadScene("Random");
    }
}