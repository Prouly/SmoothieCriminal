<<<<<<< HEAD
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public string escenaBase = "Random";      //Escena base donde se espera
    public string[] minijuegos;               //Lista de minijuegos
    public int vidas = 4;
    [SerializeField] private float puntosPorIncrementoVelocidad = 3f;
    private float velocidadAnterior = 1f;
    public float puntos = 0;
    [SerializeField] private float puntosMaximos = 12f;
    public float tiempoPantallaVictoriaDerrota = 1f; //Duración de la imagen

    public float tiempoEspera = 1f;           //Espera tras ganar/perder
    public float tiempoParaSiguiente = 2f;    //Tiempo antes de cargar un minijuego

    public Image imagenGanar;    
    public Image imagenPerder;
    public Image[] imagenesVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVelocidad;
    
    private bool enTransicion = false;

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
        if (SceneManager.GetActiveScene().name == escenaBase) StartCoroutine(Temporizador());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Evita duplicados
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

        //Calcula el multiplicador en base a los puntos
        float nuevoTimeScale = 1f + (Mathf.Floor(puntos / puntosPorIncrementoVelocidad) * 0.25f);

        //Limitar a máximo x2
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
        } else if (winOver)
        {
            yield return new WaitForSeconds(5f);
            Time.timeScale = 1f;
            vidas = 4;
            puntos = 0;
            SceneManager.LoadScene("FinDEMO");
        }

        enTransicion = false;
    }

    void CargarMinijuego()
    {
        if (minijuegos.Length == 0) return;

        int r = Random.Range(0, minijuegos.Length);
        SceneManager.LoadScene(minijuegos[r]);
    }

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
            if (textoPuntos != null) textoPuntos.text = "Puntos: " + puntos;
        }
            
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
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenGanar.gameObject.SetActive(false);
            
            if (textoVelocidad != null && Time.timeScale > 1f)
            {
                textoVelocidad.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                textoVelocidad.gameObject.SetActive(false);
            }
        }
        else if (ultimoResultado == Resultado.Perder && imagenPerder != null)
        {
            imagenPerder.gameObject.SetActive(true);
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenPerder.gameObject.SetActive(false);
        }

        ultimoResultado = Resultado.Ninguno;
        
        StartCoroutine(Temporizador());
    }
=======
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public string escenaBase = "Random";      //Escena base donde se espera
    public string[] minijuegos;               //Lista de minijuegos
    public int vidas = 4;
    [SerializeField] private float puntosPorIncrementoVelocidad = 3f;
    private float velocidadAnterior = 1f;
    public float puntos = 0;
    [SerializeField] private float puntosMaximos = 12f;
    public float tiempoPantallaVictoriaDerrota = 1f; //Duración de la imagen

    public float tiempoEspera = 1f;           //Espera tras ganar/perder
    public float tiempoParaSiguiente = 2f;    //Tiempo antes de cargar un minijuego

    public Image imagenGanar;    
    public Image imagenPerder;
    public Image[] imagenesVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoVelocidad;
    
    private bool enTransicion = false;

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
        if (SceneManager.GetActiveScene().name == escenaBase) StartCoroutine(Temporizador());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Evita duplicados
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

        //Calcula el multiplicador en base a los puntos
        float nuevoTimeScale = 1f + (Mathf.Floor(puntos / puntosPorIncrementoVelocidad) * 0.25f);

        //Limitar a máximo x2
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
        } else if (winOver)
        {
            yield return new WaitForSeconds(5f);
            Time.timeScale = 1f;
            vidas = 4;
            puntos = 0;
            SceneManager.LoadScene("FinDEMO");
        }

        enTransicion = false;
    }

    void CargarMinijuego()
    {
        if (minijuegos.Length == 0) return;

        int r = Random.Range(0, minijuegos.Length);
        SceneManager.LoadScene(minijuegos[r]);
    }

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
            if (textoPuntos != null) textoPuntos.text = "Puntos: " + puntos;
        }
            
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
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenGanar.gameObject.SetActive(false);
            
            if (textoVelocidad != null && Time.timeScale > 1f)
            {
                textoVelocidad.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                textoVelocidad.gameObject.SetActive(false);
            }
        }
        else if (ultimoResultado == Resultado.Perder && imagenPerder != null)
        {
            imagenPerder.gameObject.SetActive(true);
            yield return new WaitForSeconds(tiempoPantallaVictoriaDerrota);
            imagenPerder.gameObject.SetActive(false);
        }

        ultimoResultado = Resultado.Ninguno;
        
        StartCoroutine(Temporizador());
    }
>>>>>>> 6582b7c27b7bb627f7c1a50f0e38a056e4d593a0
}