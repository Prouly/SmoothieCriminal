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

    public string escenaBase = "Random";      
    public string[] minijuegos;               
    public int vidas = 4;
    [SerializeField] private float puntosPorIncrementoVelocidad = 3f;
    private float velocidadAnterior = 1f;
    public float puntos = 0;
    [SerializeField] private float puntosMaximos = 12f;
    public float tiempoPantallaVictoriaDerrota = 1f; 

    public float tiempoEspera = 1f;           
    public float tiempoParaSiguiente = 2f;    

    public Image imagenGanar;    
    public Image imagenPerder;
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
            DontDestroyOnLoad(gameObject); //
        }
        else
        {
            Destroy(gameObject); //
        }
    }

    void Start()
    {
        // Mantenemos la escena base dinámica según el personaje elegido
        escenaBase = SceneManager.GetActiveScene().name; 
        if (SceneManager.GetActiveScene().name == escenaBase) StartCoroutine(Temporizador()); //
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; } //[cite: 5]
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; } //[cite: 5]

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main" || scene.name == "FinDEMO")
        {
            Destroy(gameObject); //[cite: 5]
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
        if (vidas <= 0 || puntos >= puntosMaximos) yield break; //[cite: 5]
        yield return new WaitForSeconds(tiempoParaSiguiente); //[cite: 5]
        if (vidas <= 0 || puntos >= puntosMaximos) yield break; //[cite: 5]
        CargarMinijuego();
    }

    public void Ganar()
    {
        if (enTransicion) return; //[cite: 5]

        float nuevoTimeScale = 1f + (Mathf.Floor(puntos / puntosPorIncrementoVelocidad) * 0.25f); //[cite: 5]
        Time.timeScale = Mathf.Min(nuevoTimeScale, 2f); //[cite: 5]
        
        bool speedUp = nuevoTimeScale > velocidadAnterior; //[cite: 5]
        velocidadAnterior = nuevoTimeScale; //[cite: 5]

        ultimoResultado = Resultado.Ganar; //[cite: 5]
        puntos++; //[cite: 5]
        
        if (speedUp) mostrarSpeedUp = true; //[cite: 5]
        StartCoroutine(VolverAEscenaBase()); //[cite: 5]
    }

    public void Perder()
    {
        if (enTransicion) return; //[cite: 5]
        ultimoResultado = Resultado.Perder; //[cite: 5]
        vidas--; //[cite: 5]
        puntos++; //[cite: 5]
        StartCoroutine(VolverAEscenaBase()); //[cite: 5]
    }

    IEnumerator VolverAEscenaBase()
    {
        enTransicion = true; //[cite: 5]
        yield return new WaitForSeconds(tiempoEspera); //[cite: 5]

        bool gameOver = (vidas <= 0); //[cite: 5]
        bool winOver = (puntos >= puntosMaximos); //[cite: 5]
        puntosFinales = puntos; //[cite: 5]

        SceneManager.LoadScene(escenaBase); //[cite: 5]

        yield return new WaitForSeconds(0.1f); 

        if (gameOver || winOver)
        {
            Time.timeScale = 1f; //[cite: 5]
            yield return new WaitForSeconds(5f); //[cite: 5]
            vidas = 4; //[cite: 5]
            puntos = 0; //[cite: 5]
            SceneManager.LoadScene("FinDEMO"); //[cite: 5]
        }
        enTransicion = false; //[cite: 5]
    }

    /**
     * CAMBIO: Implementación de lógica anti-repetición inmediata.
     */
    void CargarMinijuego()
    {
        if (minijuegos.Length == 0) return; //[cite: 5]

        int r;
        // Si hay más de un juego, nos aseguramos de que el nuevo no sea igual al último
        if (minijuegos.Length > 1)
        {
            do
            {
                r = Random.Range(0, minijuegos.Length); //[cite: 5]
            } while (r == ultimoIndiceMinijuego);
        }
        else
        {
            r = 0;
        }

        ultimoIndiceMinijuego = r;
        SceneManager.LoadScene(minijuegos[r]); //[cite: 5]
    }

    /**
     * CAMBIO: El texto de puntos ahora incluye un salto de línea.
     */
    void ManageUIOnReturn()
    {
        GameObject canvasGO = GameObject.Find("Canvas"); //[cite: 5]
        if (canvasGO != null)
        {
            imagenGanar = canvasGO.transform.Find("Win")?.GetComponent<Image>(); //[cite: 5]
            imagenPerder = canvasGO.transform.Find("Lose")?.GetComponent<Image>(); //[cite: 5]
            textoPuntos = canvasGO.transform.Find("Puntos")?.GetComponent<TextMeshProUGUI>(); //[cite: 5]
            textoVelocidad = canvasGO.transform.Find("SpeedWarning")?.GetComponent<TextMeshProUGUI>(); //[cite: 5]
            
            // Aplicamos el salto de línea solicitado (\n)
            if (textoPuntos != null) textoPuntos.text = "Puntos:\n" + puntos; 
        }
        
        imagenCargaMinijuego = GameObject.Find("TransicionNormal"); //[cite: 5]
            
        Transform vidasParent = canvasGO.transform.Find("Vidas"); //[cite: 5]
        if (vidasParent != null)
        {
            imagenesVidas = new Image[vidasParent.childCount]; //[cite: 5]
            for (int i = 0; i < vidasParent.childCount; i++)
            {
                imagenesVidas[i] = vidasParent.GetChild(i).GetComponent<Image>(); //[cite: 5]
            }
        }
    }
    
    IEnumerator MostrarPantallaYTemporizador()
    {
        yield return null; //[cite: 5]

        if (imagenesVidas == null)
        {
            StartCoroutine(Temporizador()); //[cite: 5]
            yield break;
        }

        for (int i = 0; i < imagenesVidas.Length; i++)
        {
            if (imagenesVidas[i] != null) imagenesVidas[i].gameObject.SetActive(i < vidas); //[cite: 5]
        }

        if (vidas < 0) yield break; //[cite: 5]

        if (ultimoResultado == Resultado.Ganar && imagenGanar != null)
        {
            imagenGanar.gameObject.SetActive(true); //[cite: 5]
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(false); //[cite: 5]
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota); //[cite: 5]
            imagenGanar.gameObject.SetActive(false); //[cite: 5]
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(true); //[cite: 5]
            
            if (mostrarSpeedUp && textoVelocidad != null)
            {
                textoVelocidad.gameObject.SetActive(true); //[cite: 5]
                yield return new WaitForSeconds(1f); //[cite: 5]
                textoVelocidad.gameObject.SetActive(false); //[cite: 5]
                mostrarSpeedUp = false; 
            }
        }
        else if (ultimoResultado == Resultado.Perder && imagenPerder != null)
        {
            imagenPerder.gameObject.SetActive(true); //[cite: 5]
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(false); //[cite: 5]
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota); //[cite: 5]
            imagenPerder.gameObject.SetActive(false); //[cite: 5]
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(true); //[cite: 5]
        }

        ultimoResultado = Resultado.Ninguno; //[cite: 5]
        StartCoroutine(Temporizador()); //[cite: 5]
    }
}