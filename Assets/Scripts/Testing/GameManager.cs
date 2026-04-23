using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Necesario para la gestión de listas
using TMPro;
using UnityEngine.UI;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Luis Miguel Muñoz Vega
 * Descripción: Gestiona la lógica del Juego general desde Scene Random con rotación de minijuegos.
 * Última modificación: 23/04/2026 (Álvaro Muñoz Adán)
 * Cambiodo formato texto de puntuación y lógica para evitar se repitan juegos seguidamente
 */
public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    #region Variables de Configuración
    public string escenaBase = "Random";      // Escena base donde se espera
    public string[] minijuegos;               // Lista de minijuegos
    public int vidas = 4;
    [SerializeField] private float puntosPorIncrementoVelocidad = 3f;
    private float velocidadAnterior = 1f;
    public float puntos = 0;
    [SerializeField] private float puntosMaximos = 12f;
    
    public float tiempoPantallaVictoriaDerrota = 1.5f; //Duración de la imagen
    public float tiempoEspera = 2f;           // Espera tras ganar/perder
    public float tiempoParaSiguiente = 4f;    // Tiempo antes de cargar un minijuego
    #endregion

    #region Referencias UI
    public Image imagenGanar;    
    public Image imagenPerder;
    public GameObject imagenCargaMinijuego;
    public Image[] imagenesVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVelocidad;
    #endregion
    
    #region Variables de Estado
    private bool enTransicion = false;
    private enum Resultado { Ninguno, Ganar, Perder }
    private Resultado ultimoResultado = Resultado.Ninguno;
    
    // Almacena el último minijuego para reducir su probabilidad en la siguiente ronda
    private string ultimoMinijuegoJugado = "";
    #endregion

    #region Métodos de Unity
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
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == escenaBase) StartCoroutine(Temporizador());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Evita duplicados
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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
    #endregion

    #region Lógica de Juego
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

        // Calcula el multiplicador en base a los puntos
        float nuevoTimeScale = 1f + (Mathf.Floor(puntos / puntosPorIncrementoVelocidad) * 0.25f);

        // Limitar a máximo x2
        Time.timeScale = Mathf.Min(nuevoTimeScale, 2f);
        
        ultimoResultado = Resultado.Ganar;
        puntos++;
        
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

        SceneManager.LoadScene(escenaBase);

        yield return new WaitForSeconds(0.1f); // opcional

        if (gameOver)
        {
            yield return new WaitForSeconds(5f);
            Time.timeScale = 1f;
            vidas = 4;
            puntos = 0;
            SceneManager.LoadScene("Main");
        } 
        else if (winOver)
        {
            yield return new WaitForSeconds(5f);
            Time.timeScale = 1f;
            vidas = 4;
            puntos = 0;
            SceneManager.LoadScene("FinDEMO");
        }

        enTransicion = false;
    }

    /// <summary>
    /// Selecciona el siguiente minijuego evitando repetir el último jugado si hay más opciones.
    /// </summary>
    void CargarMinijuego()
    {
        if (minijuegos.Length == 0) return;

        string proximaEscena = "";

        // Si hay varios minijuegos, filtramos para no repetir el anterior inmediatamente
        if (minijuegos.Length > 1)
        {
            List<string> opcionesValidas = new List<string>();
            foreach (string juego in minijuegos)
            {
                if (juego != ultimoMinijuegoJugado) opcionesValidas.Add(juego);
            }

            int r = Random.Range(0, opcionesValidas.Count);
            proximaEscena = opcionesValidas[r];
        }
        else
        {
            proximaEscena = minijuegos[0];
        }

        ultimoMinijuegoJugado = proximaEscena;
        SceneManager.LoadScene(proximaEscena);
    }
    #endregion

    #region Interfaz de Usuario (UI)
    void ManageUIOnReturn()
    {
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO != null)
        {
            // Buscar imágenes dentro del Canvas
            imagenGanar = canvasGO.transform.Find("Win")?.GetComponent<Image>();
            imagenPerder = canvasGO.transform.Find("Lose")?.GetComponent<Image>();
            
            textoPuntos = canvasGO.transform.Find("Puntos")?.GetComponent<TextMeshProUGUI>();
            textoVelocidad = canvasGO.transform.Find("SpeedWarning")?.GetComponent<TextMeshProUGUI>();
            
            // Cambio solicitado: "Puntos" seguido de salto de línea y la puntuación
            if (textoPuntos != null) textoPuntos.text = "Puntos\n" + puntos;
        }
        imagenCargaMinijuego = GameObject.Find("TransicionNormal");
            
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
    
    IEnumerator MostrarPantallaYTemporizador()
    {
        yield return null;

        if (imagenesVidas == null)
        {
            StartCoroutine(Temporizador());
            yield break;
        }

        for (int i = 0; i < imagenesVidas.Length; i++)
        {
            if (imagenesVidas[i] != null) imagenesVidas[i].gameObject.SetActive(i < vidas);
        }

        if (vidas < 0)
        {
            yield break;
        }

        if (ultimoResultado == Resultado.Ganar && imagenGanar != null)
        {
            imagenGanar.gameObject.SetActive(true);  
            imagenCargaMinijuego.SetActive(false);
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenGanar.gameObject.SetActive(false);
            imagenCargaMinijuego.SetActive(true);
        }
        else if (ultimoResultado == Resultado.Perder && imagenPerder != null)
        {
            imagenPerder.gameObject.SetActive(true);
            imagenCargaMinijuego.SetActive(false);
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenPerder.gameObject.SetActive(false);
            imagenCargaMinijuego.SetActive(true);
        }

        ultimoResultado = Resultado.Ninguno;
        
        StartCoroutine(Temporizador());
    }
    #endregion
}