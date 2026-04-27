using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona el spawn de personajes y el control de tiempo fluido con sonido de disparo persistente.
 * Última modificación: 27/04/2026 (Blindaje de sonido en impactos y victoria)
 */

public class VaqueroLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Prefabs")]
    [SerializeField] private GameObject bandidoPrefab;
    [SerializeField] private GameObject inocentePrefab;

    [Header("Ajustes de Escena")]
    [SerializeField] private Transform puntosRespawnParent;
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Efectos de Sonido")]
    [SerializeField] private AudioClip sonidoDisparo;
    #endregion

    #region Variables de Estado
    private int bandidosRestantes = 0;
    private bool juegoTerminado = false;
    private float tiempoRestante; 
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Inicializamos el tiempo al límite configurado
        tiempoRestante = tiempoLimite; 
        ConfigurarEscena();
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Descontamos el tiempo cada frame para que la barra de UI se mueva suavemente
        tiempoRestante -= Time.deltaTime;

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            Debug.Log("RESULTADO: ¡Se acabó el tiempo! Perdiste.");
            FinalizarPartida(false);
        }
    }
    #endregion

    #region Lógica del Juego
    /// <summary>
    /// Centraliza la reproducción del sonido de disparo para evitar cortes al destruir objetos.
    /// </summary>
    private void ReproducirSonidoDisparo()
    {
        if (sonidoDisparo != null)
        {
            // PlayClipAtPoint garantiza que el sonido sobreviva al cambio de frame o destrucción de objetos
            AudioSource.PlayClipAtPoint(sonidoDisparo, Camera.main.transform.position);
        }
    }

    public bool EstaJuegoTerminado() => juegoTerminado;

    private void ConfigurarEscena()
    {
        if (puntosRespawnParent == null) return;

        List<Transform> puntosDisponibles = new List<Transform>();
        foreach (Transform punto in puntosRespawnParent) puntosDisponibles.Add(punto);

        int totalASpawnear = 5;
        int cantidadInocentes = Random.Range(1, 3); 
        bandidosRestantes = totalASpawnear - cantidadInocentes;

        for (int i = 0; i < totalASpawnear; i++)
        {
            if (puntosDisponibles.Count == 0) break;

            int randomIndex = Random.Range(0, puntosDisponibles.Count);
            Transform puntoElegido = puntosDisponibles[randomIndex];
            
            GameObject prefabAErigir = (i < cantidadInocentes) ? inocentePrefab : bandidoPrefab;
            
            Instantiate(prefabAErigir, puntoElegido.position, Quaternion.identity);
            puntosDisponibles.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// Registra la muerte de un bandido, reproduce sonido y finaliza si no quedan más.
    /// </summary>
    public void BandidoEliminado()
    {
        if (juegoTerminado) return;

        ReproducirSonidoDisparo();
        bandidosRestantes--;

        if (bandidosRestantes <= 0)
        {
            Debug.Log("RESULTADO: ¡Victoria! Todos los bandidos eliminados.");
            StartCoroutine(RetrasoFinalizacion(true));
        }
    }

    /// <summary>
    /// Finaliza la partida inmediatamente al disparar a un inocente con sonido previo.
    /// </summary>
    public void InocenteDisparado()
    {
        if (juegoTerminado) return;

        ReproducirSonidoDisparo();
        Debug.Log("RESULTADO: ¡Derrota! Has disparado a un inocente.");
        StartCoroutine(RetrasoFinalizacion(false));
    }

    /// <summary>
    /// Pequeña espera para asegurar que el motor de audio procese el disparo antes de cerrar la escena.
    /// </summary>
    private IEnumerator RetrasoFinalizacion(bool victoria)
    {
        juegoTerminado = true;
        yield return new WaitForSeconds(0.3f);
        FinalizarPartida(victoria);
    }

    private void FinalizarPartida(bool victoria)
    {
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
    #endregion
}