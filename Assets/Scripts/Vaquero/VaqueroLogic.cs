using UnityEngine;
using System.Collections.Generic;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona el spawn de personajes y el control de tiempo fluido para la UI.
 * Última modificación: 26/04/2026
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
    #endregion

    #region Variables de Estado
    private int bandidosRestantes = 0;
    private bool juegoTerminado = false;
    private float tiempoRestante; // Variable añadida para control fluido
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
    public bool EstaJuegoTerminado() => juegoTerminado;

    private void ConfigurarEscena()
    {
        if (puntosRespawnParent == null) return;

        List<Transform> puntosDisponibles = new List<Transform>();
        foreach (Transform t in puntosRespawnParent) puntosDisponibles.Add(t);

        int cantidadInocentes = Random.Range(1, 3); 
        int cantidadBandidos = 5 - cantidadInocentes; 
        bandidosRestantes = cantidadBandidos;

        for (int i = 0; i < 5; i++)
        {
            if (puntosDisponibles.Count == 0) break;
            int randomIndex = Random.Range(0, puntosDisponibles.Count);
            Transform puntoElegido = puntosDisponibles[randomIndex];
            
            GameObject prefabAErigir = (i < cantidadInocentes) ? inocentePrefab : bandidoPrefab;
            
            Instantiate(prefabAErigir, puntoElegido.position, Quaternion.identity);
            puntosDisponibles.RemoveAt(randomIndex);
        }
    }

    public void BandidoEliminado()
    {
        if (juegoTerminado) return;

        bandidosRestantes--;
        if (bandidosRestantes <= 0)
        {
            Debug.Log("RESULTADO: ¡Victoria! Todos los bandidos eliminados.");
            FinalizarPartida(true);
        }
    }

    public void InocenteDisparado()
    {
        if (juegoTerminado) return;

        Debug.Log("RESULTADO: ¡Derrota! Has disparado a un inocente.");
        FinalizarPartida(false);
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
    #endregion
}