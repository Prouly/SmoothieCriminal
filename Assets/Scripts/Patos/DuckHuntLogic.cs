/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestiona el flujo de caza de patos, control de tiempo y estado del cursor.
 * Última modificación: 15/04/2026
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
    #endregion

    #region Métodos de Unity
    void Start()
    {
        // Ocultamos y bloqueamos el cursor para usar la mirilla
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        GenerarPatos();
        StartCoroutine(CronometroPartida());
    }
    #endregion

    #region Lógica del Juego
    /// <summary>
    /// Devuelve si la partida ha finalizado para bloquear interacciones.
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

    IEnumerator CronometroPartida()
    {
        yield return new WaitForSeconds(tiempoLimite);

        if (!juegoTerminado)
        {
            Debug.Log("SISTEMA: Tiempo agotado. Fin de la partida.");
            FinalizarJuego(false);
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

    /// <summary>
    /// Finaliza el juego, restaura el cursor y notifica el resultado.
    /// </summary>
    private void FinalizarJuego(bool victoria)
    {
        juegoTerminado = true;
        StopAllCoroutines();

        // Reactivación vital del cursor para el usuario
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameManager.instancia != null)
        {
            if (victoria) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }
    #endregion
}