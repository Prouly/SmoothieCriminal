using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luismi Muñoz
 * Descripción: Carga la scene Random y gestiona la selección visual de personajes.
 * Última modificación: 06/05/2026 (Álvaro Muñoz Adán) -> Ajuste de selección mediante Sprite Renderer con efectos de Escala y Opacidad
 */
public class ChangeScene : MonoBehaviour
{
    [Header("Configuración de Selección")]
    [SerializeField] private int opcion = -1;
    [SerializeField] private Button botonConfirmar;
    [SerializeField] private TextMeshProUGUI textoSeleccion;

    [Header("Referencias de Personajes (Scene)")]
    [SerializeField] private SpriteRenderer spriteHaznarito;
    [SerializeField] private SpriteRenderer spriteJAM;

    [Header("Ajustes Visuales")]
    [SerializeField] private float escalaSeleccionado = 1.2f;
    [SerializeField] private float escalaNoSeleccionado = 0.8f;
    [SerializeField] private float opacidadNoSeleccionada = 0.5f;

    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource musicaMenu;       // La música en bucle
    [SerializeField] private AudioSource sonidoStart;      // Sonido tras pulsar Start
    [SerializeField] private float tiempoFadeOut = 1.5f;   // Cuánto tarda en apagarse la música

    private Vector3 escalaOriginal;
    private bool comenzandoJuego = false; // Evita múltiples clics al botón Start

    private void Start()
    {
        if (spriteHaznarito != null) escalaOriginal = spriteHaznarito.transform.localScale;
        if (botonConfirmar != null) botonConfirmar.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Si falta alguna referencia, no ejecutamos nada.
        if (comenzandoJuego || spriteHaznarito == null || spriteJAM == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                // Usamos la referencia directa del objeto clicado
                if (hit.collider.gameObject == spriteHaznarito.gameObject) SeleccionarOpcion(0);
                else if (hit.collider.gameObject == spriteJAM.gameObject) SeleccionarOpcion(1);
            }
        }
    }

    public void SeleccionarOpcion(int o)
    {
        opcion = o;
        if (textoSeleccion != null)
        {
            textoSeleccion.gameObject.SetActive(true);
            string nombre = (opcion == 0) ? "Haznarito" : "JAM";
            textoSeleccion.text = "Has seleccionado a:\n" + nombre;
        }

        ActualizarFeedbackVisual();
        if (botonConfirmar != null) botonConfirmar.gameObject.SetActive(true);
    }

    private void ActualizarFeedbackVisual()
    {
        if (opcion == 0)
        {
            AplicarEfecto(spriteHaznarito, escalaSeleccionado, 1f);
            AplicarEfecto(spriteJAM, escalaNoSeleccionado, opacidadNoSeleccionada);
        }
        else if (opcion == 1)
        {
            AplicarEfecto(spriteJAM, escalaSeleccionado, 1f);
            AplicarEfecto(spriteHaznarito, escalaNoSeleccionado, opacidadNoSeleccionada);
        }
    }

    private void AplicarEfecto(SpriteRenderer sr, float multEscala, float alfa)
    {
        if (sr == null) return;
        sr.transform.localScale = escalaOriginal * multEscala;
        Color c = sr.color;
        c.a = alfa;
        sr.color = c;
    }
    
    // Método que debe llamar el OnClick() del botón Confirmar
    public void IrAjuego()
    {
        if (!comenzandoJuego)
        {
            StartCoroutine(SecuenciaInicioJuego());
        }
    }

    private IEnumerator SecuenciaInicioJuego()
    {
        comenzandoJuego = true;

        // 1. Reproducir el sonido de Start
        if (sonidoStart != null)
        {
            // Usamos PlayOneShot para asegurar que se dispare el clip asignado
            sonidoStart.PlayOneShot(sonidoStart.clip);
            Debug.Log("<color=cyan>ESCENA: El sonido de Start debería estar sonando ahora.</color>");
        }
        else
        {
            Debug.LogWarning("¡OJO! No hay un AudioSource asignado en 'Sonido Start'");
        }

        // 2. Hacer Fade Out de la música de fondo
        float tiempoTranscurrido = 0;
        float volInicial = (musicaMenu != null) ? musicaMenu.volume : 0;

        while (tiempoTranscurrido < tiempoFadeOut)
        {
            tiempoTranscurrido += Time.deltaTime;
            if (musicaMenu != null) musicaMenu.volume = Mathf.Lerp(volInicial, 0, tiempoTranscurrido / tiempoFadeOut);
            yield return null;
        }

        // 3. Esperar el resto del tiempo hasta completar los 4 segundos
        // Si el fade out duró 1.5s, esperamos los 2.5s restantes aquí
        float tiempoRestante = 4.0f - tiempoFadeOut;
        if (tiempoRestante > 0)
        {
            yield return new WaitForSecondsRealtime(tiempoRestante);
        }

        // 4. Finalmente, cargar la escena
        if (opcion == 0) SceneManager.LoadScene("RandomHaznarito");
        else if (opcion == 1) SceneManager.LoadScene("RandomJAM");
    }
    
       public void OnClickButtonBack()
        {
            SceneManager.LoadScene("MainMenu");
        }
}