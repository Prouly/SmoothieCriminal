using UnityEngine;
using System.Collections;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión del minijuego de natación. Control de meta, tiempo y colisión con el pez.
 * Última modificación: 28/04/2026
 */

public class NatacionLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Ajustes de Carrera")]
    [SerializeField] private float xMeta = 8.5f; // Posición X donde están las boyas de "Finish"
    [SerializeField] private Transform jugador;
    [SerializeField] private Transform pezEnemigo;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoChapoteo;
    #endregion

    #region Variables de Estado
    private float tiempoRestante;
    private bool juegoTerminado = false;
    #endregion

    #region Métodos de Unity
    void Start()
    {
        tiempoRestante = tiempoLimite;
        // Ocultar cursor como en los otros juegos
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (juegoTerminado) return;

        ManejarCronometro();
        VerificarCondiciones();
    }
    #endregion

    #region Lógica de Juego
    private void ManejarCronometro()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("SISTEMA: El tiempo se agotó. ¡El pez te atrapó!");
            FinalizarPartida(false);
        }
    }

    private void VerificarCondiciones()
    {
        // Victoria: El jugador cruza la meta
        if (jugador.position.x >= xMeta)
        {
            Debug.Log("SISTEMA: ¡Libre de peligro! Has ganado.");
            FinalizarPartida(true);
        }

        // Derrota: El pez alcanza la posición del jugador
        if (pezEnemigo.position.x >= jugador.position.x)
        {
            Debug.Log("SISTEMA: El pez te alcanzó.");
            FinalizarPartida(false);
        }
    }

    public void ReproducirChapoteo()
    {
        if (sonidoChapoteo != null)
        {
            // Sonido persistente estilo Smoothie Criminal
            AudioSource.PlayClipAtPoint(sonidoChapoteo, Camera.main.transform.position);
        }
    }

    private void FinalizarPartida(bool victoria)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }
    #endregion

    #region Getters para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    public bool EstaJuegoTerminado() => juegoTerminado;
    #endregion
}