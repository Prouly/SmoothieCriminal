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
    [SerializeField] private float tiempoPresentacionPersonaje = 4f;

    [Header("Panel de Controles")]
    public GameObject panelControles; 
    public float tiempoEsperaControles = 4f;
    private bool introFinalizada = false;

    [Header("Pool de Personajes (Prefabs)")]
    public GameObject[] prefabsPosiblesObjetivos; 
    public Sprite[] spritesParaUI; 
    public GameObject[] prefabsRivales; 

    [Header("Referencias de UI Intro Personaje")]
    public GameObject panelPresentacionPersonaje;
    public Image imagenObjetivoUI;

    [Header("Puntos de Spawn")]
    public Transform contenedorSpawns; 

    [Header("Ajustes de Sonido")]
    public AudioClip winSound; 
    public AudioClip loseSound; 
    #endregion

    private bool juegoTerminado = false;
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

        // Iniciamos la secuencia que encadena ambos paneles
        StartCoroutine(SecuenciaCompletaIntro());
    }

    void Update()
    {
        // El juego solo corre cuando AMBAS intros han terminado
        if (!introFinalizada || juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0) FinalizarJuego(false);
    }

    private void ElegirObjetivoAleatorio()
    {
        if (prefabsPosiblesObjetivos.Length > 0)
        {
            int index = Random.Range(0, prefabsPosiblesObjetivos.Length);
            prefabSeleccionadoObjetivo = prefabsPosiblesObjetivos[index];
            
            if (index < spritesParaUI.Length)
            {
                spriteParaMostrarEnUI = spritesParaUI[index];
                if (imagenObjetivoUI != null)
                {
                    imagenObjetivoUI.sprite = spriteParaMostrarEnUI;
                    imagenObjetivoUI.enabled = true;
                }
            }
        }
    }

    // NUEVA CORRUTINA: Encadena el Panel de Controles y luego el del Personaje
    IEnumerator SecuenciaCompletaIntro()
    {
        introFinalizada = false;

        // 1. Fase de Controles
        if (panelControles != null)
        {
            panelControles.SetActive(true);
            yield return new WaitForSeconds(tiempoEsperaControles);
            panelControles.SetActive(false);
        }

        // 2. Fase de Personaje a encontrar
        if (panelPresentacionPersonaje != null)
        {
            panelPresentacionPersonaje.SetActive(true);
            yield return new WaitForSeconds(tiempoPresentacionPersonaje);
            panelPresentacionPersonaje.SetActive(false);
        }

        // 3. Generar Spawns (Solo cuando todo lo visual ha terminado)
        ConfigurarEscena(); 
        introFinalizada = true;
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

        int indiceAleatorio = Random.Range(0, puntosDisponibles.Count);
        Transform puntoObjetivo = puntosDisponibles[indiceAleatorio];
        puntosDisponibles.RemoveAt(indiceAleatorio);

        InstanciarPersonaje(puntoObjetivo, prefabSeleccionadoObjetivo, true);

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
            JamTarget target = nuevo.GetComponent<JamTarget>();
            if(target == null) target = nuevo.AddComponent<JamTarget>();
            target.Configurar(this);
        }
        else
        {
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