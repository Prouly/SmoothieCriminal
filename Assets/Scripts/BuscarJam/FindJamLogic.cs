/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Lógica del minijuego Busca a JAM con sorting dinámico y gestión de contenedor de spawns.
 * Última modificación: 10/05/2026 -> Agregados sonidos en victoria o derrota
 */
using UnityEngine;
using System.Collections.Generic;

public class FindJamLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Referencias de Personajes")]
    [SerializeField] private GameObject prefabHaznarito; 
    [SerializeField] private GameObject[] prefabsRivales; // Los otros 10 personajes
    
    [Header("Puntos de Spawn")]
    [SerializeField] private Transform contenedorSpawns; // Objeto padre que contiene los 41 puntos

    [Header("Ajustes de Sonido")]
    [SerializeField] private AudioClip winSound; // Sonido al ganar
    [SerializeField] private AudioClip loseSound; // Sonido al perder por tiempo
    #endregion

    #region Variables de Estado
    private bool juegoTerminado = false;
    private float tiempoRestante;
    private List<Transform> puntosDisponibles = new List<Transform>();
    private AudioSource audioSource; // Referencia interna
    #endregion

    void Start()
    {
        tiempoRestante = tiempoLimite;

        // Configuración de audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

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
        if (contenedorSpawns == null) return;
        foreach (Transform t in contenedorSpawns)
        {
            puntosDisponibles.Add(t);
        }
    }

    private void ConfigurarEscena()
    {
        if (puntosDisponibles.Count == 0) return;

        // Spawn Haznarito
        int indiceAzar = Random.Range(0, puntosDisponibles.Count);
        SpawnPersonaje(prefabHaznarito, puntosDisponibles[indiceAzar], true);
        puntosDisponibles.RemoveAt(indiceAzar);

        // Spawn Rivales
        foreach (Transform punto in puntosDisponibles)
        {
            GameObject prefabRival = prefabsRivales[Random.Range(0, prefabsRivales.Length)];
            SpawnPersonaje(prefabRival, punto, false);
        }
    }

    private void SpawnPersonaje(GameObject prefab, Transform punto, bool esObjetivo)
    {
        GameObject nuevoPersonaje = Instantiate(prefab, punto.position, Quaternion.identity, punto);
        
        SpriteRenderer sr = nuevoPersonaje.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = Mathf.RoundToInt(punto.position.y * -100f);
        }

        if (esObjetivo)
        {
            JamTarget target = nuevoPersonaje.AddComponent<JamTarget>();
            target.Configurar(this);
        }
    }

    public void FinalizarJuego(bool ganado)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (ganado)
        {
            // --- EFECTO DE SONIDO: VICTORIA ---
            if (winSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(winSound);
            }

            Debug.Log("SISTEMA: ¡GANASTE! Haznarito encontrado.");
            if (GameManager.instancia != null) GameManager.instancia.Ganar();
        }
        else
        {
            // --- EFECTO DE SONIDO: DERROTA POR TIEMPO ---
            if (loseSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(loseSound);
            }

            Debug.Log("SISTEMA: ¡PERDISTE! Se agotó el tiempo.");
            if (GameManager.instancia != null) GameManager.instancia.Perder();
        }
    }
    
    #region Getters Públicos para UI
    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}