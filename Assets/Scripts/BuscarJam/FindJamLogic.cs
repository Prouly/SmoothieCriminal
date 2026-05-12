using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Lógica del minijuego Busca a JAM con fase de presentación y selección de prefabs individuales.
 * Última modificación: 12/05/2026 -> Creado Panel de los 3 posibles personajes a buscar adaptado para usar Prefabs específicos por personaje.
 */

public class FindJamLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Ajustes de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f;
    [SerializeField] private float tiempoPresentacion = 4f;

    [Header("Pool de Personajes (Prefabs)")]
    public GameObject[] prefabsPosiblesObjetivos; 
    public Sprite[] spritesParaUI; // Sprites para UI (en el mismo orden que los prefabs)
    public GameObject[] prefabsRivales; 

    [Header("Referencias de UI Intro")]
    public GameObject panelPresentacion;
    public Image imagenObjetivoUI;

    [Header("Puntos de Spawn")]
    public Transform contenedorSpawns; 

    [Header("Ajustes de Sonido")]
    public AudioClip winSound; 
    public AudioClip loseSound; 
    #endregion

    private bool juegoTerminado = false;
    private bool juegoIniciado = false;
    private float tiempoRestante;
    private List<Transform> puntosDisponibles = new List<Transform>();
    private AudioSource audioSource;
    
    private GameObject prefabSeleccionadoObjetivo;
    private Sprite spriteParaMostrarEnUI;

    void Start()
    {
        tiempoRestante = tiempoLimite;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        ObtenerPuntosDeSpawn();
        ElegirObjetivoAleatorio();
        StartCoroutine(SecuenciaIntro());
    }

    void Update()
    {
        if (!juegoIniciado || juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0) FinalizarJuego(false);
    }

    private void ElegirObjetivoAleatorio()
    {
        if (prefabsPosiblesObjetivos.Length > 0)
        {
            int index = Random.Range(0, prefabsPosiblesObjetivos.Length);
            prefabSeleccionadoObjetivo = prefabsPosiblesObjetivos[index];
            
            // Usamos el array de sprites para evitar errores de lectura del prefab
            if (index < spritesParaUI.Length)
            {
                spriteParaMostrarEnUI = spritesParaUI[index];
                if (imagenObjetivoUI != null)
                {
                    imagenObjetivoUI.sprite = spriteParaMostrarEnUI;
                    // Forzamos que la imagen sea visible
                    imagenObjetivoUI.enabled = true;
                }
            }
        }
    }

    IEnumerator SecuenciaIntro()
    {
        // Aseguramos que el panel esté activo
        if (panelPresentacion != null) panelPresentacion.SetActive(true);
        juegoIniciado = false;

        yield return new WaitForSeconds(tiempoPresentacion);

        if (panelPresentacion != null) panelPresentacion.SetActive(false);
        ConfigurarEscena(); 
        juegoIniciado = true;
    }

    void ObtenerPuntosDeSpawn()
    {
        puntosDisponibles.Clear();
        foreach (Transform hijo in contenedorSpawns)
        {
            puntosDisponibles.Add(hijo);
        }
    }

    void ConfigurarEscena()
    {
        if (puntosDisponibles.Count == 0 || prefabSeleccionadoObjetivo == null) return;

        // 1. Elegir UN solo punto para el objetivo
        int indiceAleatorio = Random.Range(0, puntosDisponibles.Count);
        Transform puntoObjetivo = puntosDisponibles[indiceAleatorio];
        puntosDisponibles.RemoveAt(indiceAleatorio);

        InstanciarPersonaje(puntoObjetivo, prefabSeleccionadoObjetivo, true);

        // 2. Rellenar el RESTO de puntos con rivales aleatorios
        foreach (Transform punto in puntosDisponibles)
        {
            GameObject prefabRival = prefabsRivales[Random.Range(0, prefabsRivales.Length)];
            InstanciarPersonaje(punto, prefabRival, false);
        }
    }

    void InstanciarPersonaje(Transform punto, GameObject prefab, bool esObjetivo)
    {
        GameObject nuevo = Instantiate(prefab, punto.position, Quaternion.identity, punto);
        
        SpriteRenderer sr = nuevo.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = Mathf.RoundToInt(punto.position.y * -100f);
        }

        if (esObjetivo)
        {
            // Solo el objetivo recibe el script de click
            JamTarget target = nuevo.GetComponent<JamTarget>();
            if(target == null) target = nuevo.AddComponent<JamTarget>();
            target.Configurar(this);
        }
        else
        {
            // Opcional: Asegurarnos de que los rivales no tengan el script JamTarget por error
            JamTarget targetInadecuado = nuevo.GetComponent<JamTarget>();
            if(targetInadecuado != null) Destroy(targetInadecuado);
        }
    }

    public void FinalizarJuego(bool ganado)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (ganado)
        {
            if (winSound != null) audioSource.PlayOneShot(winSound);
            GameManager.instancia?.Ganar();
        }
        else
        {
            if (loseSound != null) audioSource.PlayOneShot(loseSound);
            GameManager.instancia?.Perder();
        }
    }

    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
}