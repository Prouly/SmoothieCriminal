using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona el spawn aleatorio de bandidos e inocentes y la condición de victoria por tiempo.
 * Última modificación: 14/04/2026
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
    #endregion

    #region Métodos de Unity
    void Start()
    {
        ConfigurarEscena();
        StartCoroutine(CronometroJuego());
    }
    #endregion

    #region Lógica del Juego
    /// <summary>
    /// Selecciona 5 puntos aleatorios y distribuye bandidos e inocentes según las reglas.
    /// Indica si la partida ha finalizado para bloquear interacciones externas.
    /// </summary>
    /// <returns>Verdadero si el juego ha terminado.</returns>
    
    public bool EstaJuegoTerminado()
    {
        return juegoTerminado;
    }
    private void ConfigurarEscena()
    {
        // Obtenemos todos los puntos disponibles del objeto padre
        List<Transform> puntosDisponibles = new List<Transform>();
        foreach (Transform t in puntosRespawnParent) puntosDisponibles.Add(t);

        // Determinamos cuántos inocentes saldrán (mínimo 1, máximo 2)
        int cantidadInocentes = Random.Range(1, 3); 
        int cantidadBandidos = 5 - cantidadInocentes; // El resto hasta 5 son bandidos
        bandidosRestantes = cantidadBandidos;

        // Spawneamos los personajes en posiciones aleatorias sin repetir punto
        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, puntosDisponibles.Count);
            Transform puntoElegido = puntosDisponibles[randomIndex];
            
            // Decidimos qué prefab instanciar basándonos en el conteo restante
            GameObject prefabAErigir = (i < cantidadInocentes) ? inocentePrefab : bandidoPrefab;
            
            Instantiate(prefabAErigir, puntoElegido.position, Quaternion.identity);
            puntosDisponibles.RemoveAt(randomIndex); // Eliminamos el punto para no repetir
        }

        Debug.Log($"SISTEMA: Han aparecido {cantidadBandidos} bandidos y {cantidadInocentes} inocentes.");
    }

    /// <summary>
    /// Gestiona el tiempo límite de la partida.
    /// </summary>
    IEnumerator CronometroJuego()
    {
        yield return new WaitForSeconds(tiempoLimite);

        if (!juegoTerminado)
        {
            Debug.Log("RESULTADO: ¡Se acabó el tiempo! Perdiste.");
            FinalizarPartida(false);
        }
    }

    /// <summary>
    /// Registra la muerte de un bandido y finaliza si no quedan más.
    /// </summary>
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

    /// <summary>
    /// Finaliza la partida inmediatamente al disparar a un inocente.
    /// </summary>
    public void InocenteDisparado()
    {
        if (juegoTerminado) return;

        Debug.Log("RESULTADO: ¡Derrota! Has disparado a un inocente.");
        FinalizarPartida(false);
    }

    /// <summary>
    /// Comunica el resultado al GameManager o termina la ejecución local.
    /// </summary>
    /// <param name="victoria">Indica si el jugador ganó la partida.</param>
    private void FinalizarPartida(bool victoria)
    {
        juegoTerminado = true;
        StopAllCoroutines();

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }
    #endregion
}