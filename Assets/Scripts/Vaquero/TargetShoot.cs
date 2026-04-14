using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Detecta el disparo del jugador y notifica a la lógica principal.
 * Última modificación: 10/04/2026
 */

public class TargetShoot : MonoBehaviour
{
    #region Variables de Configuración
    [SerializeField] private bool esBandido = true;
    #endregion

    #region Variables Privadas
    private VaqueroLogic logica;
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Buscamos la lógica en la escena para reportar el disparo
        logica = FindFirstObjectByType<VaqueroLogic>();
    }

    /// <summary>
    /// Detecta el click del ratón (disparo) sobre el collider del personaje.
    /// </summary>
    private void OnMouseDown()
    {
        if (logica == null) return;

        if (esBandido)
        {
            logica.BandidoEliminado();
        }
        else
        {
            logica.InocenteDisparado();
        }

        // El personaje desaparece al ser disparado
        gameObject.SetActive(false);
    }
    #endregion
}