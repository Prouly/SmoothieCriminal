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
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        // Mantenemos la escena base dinámica según el personaje elegido
        escenaBase = SceneManager.GetActiveScene().name; 
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
        GameObject canvasGO = GameObject.Find("Canvas"); 
        if (canvasGO != null)
        {
            imagenGanar = canvasGO.transform.Find("Win")?.GetComponent<Image>(); 
            imagenPerder = canvasGO.transform.Find("Lose")?.GetComponent<Image>(); 
            textoPuntos = canvasGO.transform.Find("Puntos")?.GetComponent<TextMeshProUGUI>(); 
            textoVelocidad = canvasGO.transform.Find("SpeedWarning")?.GetComponent<TextMeshProUGUI>(); 
            
            // Aplicamos texto Puntos + salto de línea (\n)
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

        if (vidas < 0) yield break; 

        if (ultimoResultado == Resultado.Ganar && imagenGanar != null)
        {
            imagenGanar.gameObject.SetActive(true); 
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(false); 
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota); 
            imagenGanar.gameObject.SetActive(false); 
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(true); 
            
            if (mostrarSpeedUp && textoVelocidad != null)
            {
                textoVelocidad.gameObject.SetActive(true); 
                yield return new WaitForSeconds(1f); 
                textoVelocidad.gameObject.SetActive(false); 
                mostrarSpeedUp = false; 
            }
        }
        else if (ultimoResultado == Resultado.Perder && imagenPerder != null)
        {
            imagenPerder.gameObject.SetActive(true); 
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(false); 
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenPerder.gameObject.SetActive(false); 
            if (imagenCargaMinijuego != null) imagenCargaMinijuego.SetActive(true); 
        }

        ultimoResultado = Resultado.Ninguno; 
        StartCoroutine(Temporizador()); 
    }
}