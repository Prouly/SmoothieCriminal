/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona el flujo de caza de patos, control de tiempo y estado del juego.
 * Última modificación: 26/04/2026
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

        // Descontamos el tiempo cada frame para una barra de UI fluida
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
    /// Devuelve si la partida ha finalizado. Necesario para ClickableDuck.
    /// </summary>
    public bool EstaJuegoTerminado()
    {
        return juegoTerminado;
    }

    private void GenerarPatos()
    {
        if (patoPrefab == null || contenedorPuntosSpawn == null) return;

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

    public void RegistrarBaja()
    {
        if (juegoTerminado) return;

        patosEliminados++;
        if (patosEliminados >= patosAGenerar)
        {
            Debug.Log("SISTEMA: ¡Victoria! Todos los patos abatidos.");
            FinalizarJuego(true);
        }
    }

    private void FinalizarJuego(bool victoria)
    {
        if (juegoTerminado) return;
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
    #endregion
}