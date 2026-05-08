using System.Collections;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán / IA
 * Descripción: Detección precisa con Offset para la punta del dedo.
 * Última modificación: 08/05/2026
 */

[RequireComponent(typeof(AudioSource))]
public class NarizGameLogic : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform mano;

    [Header("Ajuste de Sensor (Punta del Dedo)")]
    [SerializeField] private Vector2 offsetDedo = new Vector2(0, 1.5f); 

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadOscilacion = 4f;
    [SerializeField] private float amplitudOscilacion = 2.5f;
    [SerializeField] private float fuerzaSubida = 12f; // Un poco más rápido para mejor sensación

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoLimite = 7f; 

    [Header("Audio (AudioClips)")]
    [SerializeField] private AudioClip clipAcierto;
    [SerializeField] private AudioClip clipFallo;

    private AudioSource miAudioSource;
    private float tiempoRestante;
    private bool juegoFinalizado = false;
    private bool haLanzadoDedo = false;
    private Vector3 posicionInicialMano;

    private void Awake()
    {
        miAudioSource = GetComponent<AudioSource>();
        if (mano == null) mano = transform;
    }

    private void Start()
    {
        tiempoRestante = tiempoLimite;
        posicionInicialMano = mano.position;
    }

    private void Update()
    {
        if (juegoFinalizado) return;

        ManejarTiempo();

        if (!haLanzadoDedo)
        {
            OscilarMano();
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(SecuenciaUnicoIntento());
            }
        }
    }

    private void ManejarTiempo()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0) FinalizarJuego(false, "¡TIEMPO AGOTADO!");
    }

    private void OscilarMano()
    {
        float offset = Mathf.Sin(Time.time * velocidadOscilacion) * amplitudOscilacion;
        mano.position = posicionInicialMano + new Vector3(offset, 0, 0);
    }

    private IEnumerator SecuenciaUnicoIntento()
    {
        haLanzadoDedo = true;

        // 1. Subida
        float t = 0;
        while (t < 0.12f)
        {
            t += Time.deltaTime;
            mano.position += Vector3.up * fuerzaSubida * Time.deltaTime;
            yield return null;
        }

        // 2. FORZAR ACTUALIZACIÓN DE FÍSICA
        yield return new WaitForFixedUpdate();

        // 3. COMPROBACIÓN FILTRADA (Ignorar la mano)
        Vector2 puntoExactoDedo = (Vector2)mano.position + offsetDedo;
        
        // Obtenemos TODOS los colliders en ese punto
        Collider2D[] hits = Physics2D.OverlapPointAll(puntoExactoDedo);
        bool aciertoDetectado = false;

        foreach (Collider2D hit in hits)
        {
            // Si alguno de los objetos tocados es la fosa nasal, es victoria
            if (hit.CompareTag("FosaNasal"))
            {
                aciertoDetectado = true;
                break; 
            }
        }
        
        if (aciertoDetectado)
        {
            FinalizarJuego(true, "¡DENTRO!");
        }
        else
        {
            // Debug para ver qué estamos tocando si no es la fosa
            if (hits.Length > 0) 
                Debug.Log("El punto verde tocó: " + hits[0].name + " (y otros " + (hits.Length-1) + " objetos), pero ninguno es fosa.");
            
            FinalizarJuego(false, "¡FUERA!");
        }
    }

    private void FinalizarJuego(bool victoria, string mensaje)
    {
        if (juegoFinalizado) return;
        juegoFinalizado = true;

        if (victoria)
        {
            if (clipAcierto != null) miAudioSource.PlayOneShot(clipAcierto);
            if (GameManager.instancia != null) GameManager.instancia.Ganar();
        }
        else
        {
            if (clipFallo != null) miAudioSource.PlayOneShot(clipFallo);
            if (GameManager.instancia != null) GameManager.instancia.Perder();
        }
        Debug.Log(mensaje);
    }

    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);

    private void OnDrawGizmos()
    {
        if (mano != null)
        {
            Gizmos.color = Color.green; // Cambiado a verde para diferenciar
            Vector3 posicionGizmo = mano.position + (Vector3)offsetDedo;
            Gizmos.DrawSphere(posicionGizmo, 0.1f);
        }
    }
}