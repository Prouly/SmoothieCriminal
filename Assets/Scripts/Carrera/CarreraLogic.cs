/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona la lógica central, temporizador y estado de la carrera.
 * Última modificación: 26/04/2026
 */

using UnityEngine;

public class CarreraLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;
    #endregion

    #region Variables de Estado
    private float tiempoRestante;
    private bool juegoTerminado = false;
    #endregion

    void Start()
    {
        tiempoRestante = tiempoLimite;
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Descuento de tiempo fluido
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("RESULTADO: ¡Tiempo agotado! Perdiste la carrera.");
            FinalizarCarrera(false);
        }
    }

    #region Control de Partida
    public void FinalizarCarrera(bool victoria)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }

    public bool EstaJuegoTerminado() => juegoTerminado;
    #endregion

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}