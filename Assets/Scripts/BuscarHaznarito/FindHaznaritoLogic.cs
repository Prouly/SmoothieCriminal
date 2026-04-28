/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Lógica del minijuego Busca a Haznarito con sorting dinámico y gestión de contenedor de spawns.
 * Última modificación: 23/04/2026
 */
using UnityEngine;
using System.Collections.Generic;

public class FindHaznaritoLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Referencias de Personajes")]
    [SerializeField] private GameObject prefabHaznarito; 
    [SerializeField] private GameObject[] prefabsRivales; // Los otros 10 personajes
    
    [Header("Puntos de Spawn")]
    [SerializeField] private Transform contenedorSpawns; // Objeto padre que contiene los 41 puntos
    #endregion

    #region Variables de Estado
    private bool juegoTerminado = false;
    private float tiempoRestante;
    private List<Transform> puntosDisponibles = new List<Transform>();
    #endregion

    void Start()
    {
        tiempoRestante = tiempoLimite;
        ObtenerPuntosDeSpawn();
        ConfigurarEscena();
    }

    void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            FinalizarJuego(false);
        }
    }

    private void ObtenerPuntosDeSpawn()
    {
        if (contenedorSpawns == null)
        {
            Debug.LogError("SISTEMA: No se ha asignado el objeto padre de los Spawns.");
            return;
        }

        // Limpiamos y añadimos todos los hijos del contenedor
        puntosDisponibles.Clear();
        foreach (Transform hijo in contenedorSpawns)
        {
            puntosDisponibles.Add(hijo);
        }
        Debug.Log($"SISTEMA: Se han detectado {puntosDisponibles.Count} puntos de spawn.");
    }

    private void ConfigurarEscena()
    {
        if (puntosDisponibles.Count < 20)
        {
            Debug.LogError("SISTEMA: Faltan puntos de spawn en el contenedor (se necesitan al menos 20).");
            return;
        }

        // 1. Barajar los puntos de spawn (Algoritmo Fisher-Yates simple)
        for (int i = 0; i < puntosDisponibles.Count; i++)
        {
            Transform temp = puntosDisponibles[i];
            int randomIndex = Random.Range(i, puntosDisponibles.Count);
            puntosDisponibles[i] = puntosDisponibles[randomIndex];
            puntosDisponibles[randomIndex] = temp;
        }

        // 2. Instanciar a Haznarito (único) en la primera posición aleatoria
        InstanciarPersonaje(prefabHaznarito, puntosDisponibles[0], true);

        // 3. Instanciar 19 rivales (asegurando usar los 10 y luego repetir)
        int rivalIndex = 0;
        for (int i = 1; i < 20; i++)
        {
            InstanciarPersonaje(prefabsRivales[rivalIndex], puntosDisponibles[i], false);
            
            rivalIndex++;
            if (rivalIndex >= prefabsRivales.Length) rivalIndex = 0; 
        }

        Debug.Log("SISTEMA: Escena generada con 20 personajes. ¡Busca a Haznarito!");
    }

    private void InstanciarPersonaje(GameObject prefab, Transform punto, bool esObjetivo)
    {
        GameObject nuevoPersonaje = Instantiate(prefab, punto.position, Quaternion.identity, punto);
        
        // Ajuste de Sorting Order según posición Y (cuanto más abajo, mayor el orden)
        SpriteRenderer sr = nuevoPersonaje.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Multiplicamos por -100 para que una Y menor (más abajo) resulte en un Sort Order mayor
            sr.sortingOrder = Mathf.RoundToInt(punto.position.y * -100f);
        }

        if (esObjetivo)
        {
            HaznaritoTarget target = nuevoPersonaje.AddComponent<HaznaritoTarget>();
            target.Configurar(this);
        }
    }

    public void FinalizarJuego(bool ganado)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (ganado)
        {
            Debug.Log("SISTEMA: ¡GANASTE! Haznarito encontrado.");
            if (GameManager.instancia != null) GameManager.instancia.Ganar();
        }
        else
        {
            Debug.Log("SISTEMA: ¡PERDISTE! Se agotó el tiempo.");
            if (GameManager.instancia != null) GameManager.instancia.Perder();
        }
    }
    
    #region Getters Públicos para UI
    /// <summary>
    /// Devuelve el tiempo máximo configurable para este minijuego.
    /// </summary>
    public float ObtenerTiempoLimite()
    {
        return tiempoLimite;
    }

    /// <summary>
    /// Devuelve el tiempo que le queda al jugador antes de perder.
    /// </summary>
    public float ObtenerTiempoRestante()
    {
        // Aseguramos que no devuelva valores negativos
        return Mathf.Max(0f, tiempoRestante);
    }
    #endregion
}