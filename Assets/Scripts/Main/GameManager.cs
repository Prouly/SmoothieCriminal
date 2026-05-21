using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luismi Muñoz / Álvaro Muñoz Adán
 * Descripción: Gestiona la lógica general, evita minijuegos repetidos y formatea la UI.
 * Última modificación: 04/05/2026
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instancia;
    public string escenaMinijuego;

    public string escenaBase = "Random";      
    public string[] minijuegos;               
    public int vidas = 4;
    [SerializeField] private float puntosPorIncrementoVelocidad = 3f;
    private float velocidadAnterior = 1f;
    public float puntos = 0;
    [SerializeField] private float puntosMaximos = 12f;
    public float tiempoPantallaVictoriaDerrota = 2.5f; 

    public float tiempoEspera = 2f;           
    public float tiempoParaSiguiente = 3.4f;    

    public GameObject imagenGanar;
    public GameObject imagenPerder;
    public GameObject imagenCargaMinijuego;
    public Image[] imagenesVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVelocidad;
    
    private bool enTransicion = false;
    private bool mostrarSpeedUp = false;
    public float puntosFinales;

    // Variables para control de repetición y estado
    private int ultimoIndiceMinijuego = -1; 
    private enum Resultado { Ninguno, Ganar, Perder }
    private Resultado ultimoResultado = Resultado.Ninguno;
    
    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
        if (!string.IsNullOrEmpty(MinigameSelector.escenaPendiente))
        {
            escenaMinijuego = MinigameSelector.escenaPendiente;
            MinigameSelector.escenaPendiente = "";
        }
    }

    void Start()
    {
        // Mantenemos la escena base dinámica según el personaje elegido
        escenaBase = SceneManager.GetActiveScene().name; 
        // Si estamos en la escena selector, forzamos el array
        if (minijuegos == null || minijuegos.Length == 0)
        {
            if (!string.IsNullOrEmpty(escenaMinijuego))
            {
                minijuegos = new string[] { escenaMinijuego };
            }
        }
        
        if (SceneManager.GetActiveScene().name == escenaBase) StartCoroutine(Temporizador()); 
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; } 
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; } 

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main" || scene.name == "FinDEMO")
        {
            Destroy(gameObject); 
            return;
        }
        
        if (scene.name == escenaBase)
        {
            ManageUIOnReturn();
            StartCoroutine(MostrarPantallaYTemporizador());
        }
    }

    IEnumerator Temporizador()
    {
        if (vidas <= 0 || puntos >= puntosMaximos) yield break; 
        yield return new WaitForSeconds(tiempoParaSiguiente); 
        if (vidas <= 0 || puntos >= puntosMaximos) yield break; 
        CargarMinijuego();
    }

    public void Ganar()
    {
        if (enTransicion) return; 

        float nuevoTimeScale = 1f + (Mathf.Floor(puntos / puntosPorIncrementoVelocidad) * 0.25f); 
        Time.timeScale = Mathf.Min(nuevoTimeScale, 2f); 
        
        bool speedUp = nuevoTimeScale > velocidadAnterior; 
        velocidadAnterior = nuevoTimeScale; 

        ultimoResultado = Resultado.Ganar; 
        puntos++; 
        
        if (speedUp) mostrarSpeedUp = true; 
        StartCoroutine(VolverAEscenaBase()); 
    }

    public void Perder()
    {
        if (enTransicion) return; 
        ultimoResultado = Resultado.Perder; 
        vidas--; 
        puntos++; 
        StartCoroutine(VolverAEscenaBase()); 
    }

    IEnumerator VolverAEscenaBase()
    {
        enTransicion = true; 
        yield return new WaitForSeconds(tiempoEspera); 

        bool gameOver = (vidas <= 0); 
        bool winOver = (puntos >= puntosMaximos); 
        puntosFinales = puntos; 

        SceneManager.LoadScene(escenaBase); 

        yield return new WaitForSeconds(0.1f); 

        if (gameOver || winOver)
        {
            Time.timeScale = 1f; 
            yield return new WaitForSeconds(5f); 
            vidas = 4; 
            puntos = 0; 
            SceneManager.LoadScene("FinDEMO"); 
        }
        enTransicion = false; 
    }
    
    void CargarMinijuego()
    {
        if (minijuegos.Length == 0) return; 

        int r;
        // Si hay más de un juego, nos aseguramos de que el nuevo no sea igual al último
        if (minijuegos.Length > 1)
        {
            do
            {
                r = Random.Range(0, minijuegos.Length); 
            } while (r == ultimoIndiceMinijuego);
        }
        else
        {
            r = 0;
        }

        ultimoIndiceMinijuego = r;
        SceneManager.LoadScene(minijuegos[r]); 
    }
    
    void ManageUIOnReturn()
{
    // RESET de referencias: Muy importante al usar DontDestroyOnLoad
    // Evitamos usar objetos "viejos" de la escena anterior
    imagenGanar = null;
    imagenPerder = null;
    imagenCargaMinijuego = null;

    GameObject canvasGO = GameObject.Find("Canvas");
    if (canvasGO != null)
    {
        textoPuntos = canvasGO.transform.Find("Ronda")?.GetComponent<TextMeshProUGUI>();
        textoVelocidad = canvasGO.transform.Find("SpeedWarning")?.GetComponent<TextMeshProUGUI>();
        if (textoPuntos != null) textoPuntos.text = "Ronda\n" + puntos;
    }

    // Buscamos en la raíz de la nueva escena
    GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

    foreach (GameObject go in rootObjects)
    {
        // Usamos nombres exactos. Asegúrate de que en Unity se llamen ASÍ exactamente.
        if (go.name == "TransicionGanar") imagenGanar = go;
        else if (go.name == "TransicionPerder") imagenPerder = go;
        else if (go.name == "TransicionNormal") imagenCargaMinijuego = go;
    }

    // Depuración: Si alguna sigue siendo null, te avisará en la consola
    if (imagenGanar == null) Debug.LogWarning("¡OJO! No se encontró TransicionGanar en esta escena.");
    if (imagenPerder == null) Debug.LogWarning("¡OJO! No se encontró TransicionPerder en esta escena.");

    if (canvasGO != null)
    {
        Transform vidasParent = canvasGO.transform.Find("Vidas");
        if (vidasParent != null)
        {
            imagenesVidas = new Image[vidasParent.childCount];
            for (int i = 0; i < vidasParent.childCount; i++)
            {
                imagenesVidas[i] = vidasParent.GetChild(i).GetComponent<Image>();
            }
        }
    }
}

IEnumerator MostrarPantallaYTemporizador()
{
    yield return null; 

    // Debug para saber qué resultado tiene el GameManager antes de mostrar nada
    Debug.Log("Mostrando pantalla. Resultado actual: " + ultimoResultado);

    if (imagenesVidas != null)
    {
        for (int i = 0; i < imagenesVidas.Length; i++)
        {
            if (imagenesVidas[i] != null) imagenesVidas[i].gameObject.SetActive(i < vidas); 
        }
    }

    if (vidas < 0) yield break; 

    if (ultimoResultado == Resultado.Ganar && imagenGanar != null)
    {
        Debug.Log("Activando objeto: " + imagenGanar.name);
        imagenGanar.SetActive(true); 
        if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(false); 
        yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota); 
        imagenGanar.SetActive(false); 
        if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(true); 
        // ... resto del código de SpeedUp
    }
    else if (ultimoResultado == Resultado.Perder && imagenPerder != null)
    {
        Debug.Log("Activando objeto: " + imagenPerder.name);
        imagenPerder.SetActive(true); 
        if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(false); 
        yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
        imagenPerder.SetActive(false); 
        if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(true); 
    }

    ultimoResultado = Resultado.Ninguno; 
    StartCoroutine(Temporizador()); 
}
}