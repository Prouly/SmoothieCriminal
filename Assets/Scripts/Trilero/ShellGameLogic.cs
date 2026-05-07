/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Gestión de trileros con soporte para temporizador visual en la fase de decisión.
 * Última modificación: 07/05/2026 -> Implementados sonidos de movimiento y de ganar/perder.
 */

using UnityEngine;
using System.Collections;

public class ShellGameLogic : MonoBehaviour
{
    #region Variables de Configuración
    [Header("Referencias de Objetos")]
    [SerializeField] private Transform[] vasos; 
    [SerializeField] private GameObject bolita; 

    [Header("Ajustes de Tiempos y Velocidad")]
    [SerializeField] private float alturaLevantado = 2f;
    [SerializeField] private float tiempoMezcla = 5f;
    [SerializeField] private float tiempoDecision = 3f; 
    [SerializeField] private float duracionIntercambio = 0.5f; 

    [Header("Ajustes de Sonido")]
    [SerializeField] private AudioClip sonidoMovimiento; // Sonido mientras se mezclan
    [SerializeField] private AudioClip sonidoAcierto;    // Sonido al ganar
    [SerializeField] private AudioClip sonidoFallo;      // Sonido al perder
    #endregion

    #region Variables de Estado
    private int indiceGanador;
    private bool puedeElegir = false;
    private bool juegoTerminado = false;
    private float tiempoRestante; 
    private AudioSource audioSource; // Referencia al emisor de sonido
    #endregion

    void Start()
    {
        // Configuramos el AudioSource automáticamente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        tiempoRestante = tiempoDecision; 
        StartCoroutine(SecuenciaJuego());
    }

    #region Corrutinas de Juego
    IEnumerator SecuenciaJuego()
    {
        // 1. Configuración inicial
        indiceGanador = Random.Range(0, vasos.Length);
        bolita.transform.position = vasos[indiceGanador].position;
        bolita.transform.SetParent(null); 

        // 2. Revelación
        Vector3 posOriginal = vasos[indiceGanador].position;
        yield return MoverVaso(vasos[indiceGanador], posOriginal + new Vector3(0, alturaLevantado, 0), 0.5f);
        yield return new WaitForSeconds(1f);
        yield return MoverVaso(vasos[indiceGanador], posOriginal, 0.5f);

        bolita.transform.SetParent(vasos[indiceGanador]);

        // 3. Mezcla
        float tiempoPasado = 0;
        while (tiempoPasado < tiempoMezcla)
        {
            int a = Random.Range(0, vasos.Length);
            int b = Random.Range(0, vasos.Length);
            while (a == b) b = Random.Range(0, vasos.Length);

            // REPRODUCIR SONIDO DE MOVIMIENTO
            if (sonidoMovimiento != null) audioSource.PlayOneShot(sonidoMovimiento);

            yield return IntercambiarVasos(a, b);
            tiempoPasado += duracionIntercambio; 
        }

        // 4. Fase de Selección
        puedeElegir = true;
        while (tiempoRestante > 0 && !juegoTerminado)
        {
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        if (!juegoTerminado)
        {
            FinalizarJuego(false);
        }
    }

    IEnumerator IntercambiarVasos(int idxA, int idxB)
    {
        Vector3 posA = vasos[idxA].position;
        Vector3 posB = vasos[idxB].position;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionIntercambio;
            float curvaSeno = Mathf.Sin(t * Mathf.PI) * 0.5f;
            vasos[idxA].position = Vector3.Lerp(posA, posB, t) + new Vector3(0, curvaSeno, 0);
            vasos[idxB].position = Vector3.Lerp(posB, posA, t) + new Vector3(0, -curvaSeno, 0);
            yield return null;
        }
        vasos[idxA].position = posB;
        vasos[idxB].position = posA;
    }

    IEnumerator MoverVaso(Transform vaso, Vector3 destino, float tiempo)
    {
        Vector3 inicio = vaso.position;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / tiempo;
            vaso.position = Vector3.Lerp(inicio, destino, t);
            yield return null;
        }
        vaso.position = destino;
    }
    #endregion

    #region Interacción y Getters
    public void IntentarSeleccion(Transform vasoClickeado)
    {
        if (!puedeElegir || juegoTerminado) return;

        juegoTerminado = true;
        bool esCorrecto = (vasoClickeado == vasos[indiceGanador]);

        // REPRODUCIR SONIDO SEGÚN EL RESULTADO
        if (esCorrecto && sonidoAcierto != null) 
            audioSource.PlayOneShot(sonidoAcierto);
        else if (!esCorrecto && sonidoFallo != null) 
            audioSource.PlayOneShot(sonidoFallo);

        if (esCorrecto) bolita.transform.SetParent(null); 

        StartCoroutine(MoverVaso(vasoClickeado, vasoClickeado.position + new Vector3(0, alturaLevantado, 0), 0.3f));
        FinalizarJuego(esCorrecto);
    }

    void FinalizarJuego(bool ganado)
    {
        if (GameManager.instancia != null)
        {
            if (ganado) GameManager.instancia.Ganar();
            else GameManager.instancia.Perder();
        }
    }

    public float ObtenerTiempoLimite() => tiempoDecision;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}