/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona el flujo de caza de patos, control de tiempo y estado del juego con sonido persistente.
 * Última modificación: 27/04/2026 (Corrección definitiva de sonido en último pato)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuckHuntLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Spawn")]
    [SerializeField] private GameObject patoPrefab;
    [SerializeField] private Transform contenedorPuntosSpawn;
    [SerializeField] private int patosAGenerar = 4;

    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoDisparo;
    #endregion

    #region Variables de Estado
    private int patosEliminados = 0;
    private bool juegoTerminado = false;
    private float tiempoRestante; 
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Configuración inicial del cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        // Inicializamos el tiempo para que el TimerUI lo detecte desde el segundo 0
        tiempoRestante = tiempoLimite; 

        GenerarPatos();
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Gestión de disparo general (cuando el jugador falla o dispara al aire)
        if (Input.GetMouseButtonDown(0))
        {
            // Nota: El sonido se reproduce aquí para disparos al aire, 
            // pero para los impactos lo llamamos desde RegistrarBaja para asegurar prioridad.
            ReproducirSonidoDisparo();
        }

        // Descontamos el tiempo cada frame
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("SISTEMA: Tiempo agotado. Fin de la partida.");
            FinalizarJuego(false);
        }
    }
    #endregion

    #region Lógica del Juego
    /// <summary>
    /// Reproduce el sonido de disparo de forma persistente.
    /// </summary>
    private void ReproducirSonidoDisparo()
    {
        if (sonidoDisparo != null)
        {
            // PlayClipAtPoint permite que el sonido no se corte si el objeto que dispara el código desaparece
            AudioSource.PlayClipAtPoint(sonidoDisparo, Camera.main.transform.position);
        }
    }

    private void GenerarPatos()
    {
        if (contenedorPuntosSpawn == null) return;

        List<Transform> puntosDisponibles = new List<Transform>();
        foreach (Transform hijo in contenedorPuntosSpawn) puntosDisponibles.Add(hijo);

        for (int i = 0; i < patosAGenerar; i++)
        {
            if (puntosDisponibles.Count == 0) break;
            int index = Random.Range(0, puntosDisponibles.Count);
            Instantiate(patoPrefab, puntosDisponibles[index].position, Quaternion.identity);
            puntosDisponibles.RemoveAt(index);
        }
    }

    /// <summary>
    /// Registra la baja de un pato y gestiona la victoria con un breve retraso para el sonido.
    /// </summary>
    public void RegistrarBaja()
    {
        if (juegoTerminado) return;

        // Forzamos la reproducción del sonido aquí para asegurar que el último pato suene
        ReproducirSonidoDisparo();

        patosEliminados++;
        if (patosEliminados >= patosAGenerar)
        {
            Debug.Log("SISTEMA: ¡Victoria! Todos los patos abatidos.");
            // Iniciamos la corrutina de espera para que no se corte el audio
            StartCoroutine(RetrasoFinalizacion(true));
        }
    }

    /// <summary>
    /// Espera un breve tiempo antes de finalizar para permitir que el feedback auditivo llegue al jugador.
    /// </summary>
    private IEnumerator RetrasoFinalizacion(bool victoria)
    {
        juegoTerminado = true; // Bloqueamos nuevas acciones inmediatamente
        yield return new WaitForSeconds(0.3f); // Tiempo suficiente para iniciar el sonido de 2.3s
        FinalizarJuego(victoria);
    }

    private void FinalizarJuego(bool victoria)
    {
        juegoTerminado = true;

        // Restauramos el cursor para que el usuario pueda navegar tras el juego
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }
    #endregion

    #region Getters de Tiempo para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    public bool EstaJuegoTerminado() => juegoTerminado;
    #endregion
}