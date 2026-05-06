using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MartilloGameManager : MonoBehaviour
{
    [SerializeField] private float timer = 7f;
    [SerializeField] private Scrollbar  sliderFuerza;
    [SerializeField] private float velocidad = 1.5f;
    [SerializeField] private RectTransform zonaVerde;
    [SerializeField] private float tamañoZona = 0.2f; // 20% de la barra

    [Header("Animaciones")]
    [SerializeField] private Image spriteObjetivo;
    [SerializeField] private TextMeshProUGUI textoResultado;
    [SerializeField] private Transform objetoMover;
    [SerializeField] private Transform posicionFinal;
    
    private float zonaMin;
    private float zonaMax;
    
    #region Variables de Estado
    private bool gameFinished = false;
    private float tiempoRestante; 
    private bool subiendo = true;
    private float valorFuerza = 0f;
    #endregion
    
    private void Start()
    {
        tiempoRestante = timer;
        GenerarZona();
    }
    
    private void Update()
    {
        if (gameFinished) return;
        
        ActualizarBarra();
        
        tiempoRestante -= Time.deltaTime;
        
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinalizarJuego(false);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool acierto = valorFuerza >= zonaMin && valorFuerza <= zonaMax;
            FinalizarJuego(acierto);
        }
    }
    
    private void ActualizarBarra()
    {
        if (subiendo)
        {
            valorFuerza += velocidad * Time.deltaTime;
            if (valorFuerza >= 1f)
            {
                valorFuerza = 1f;
                subiendo = false;
            }
        }
        else
        {
            valorFuerza -= velocidad * Time.deltaTime;
            if (valorFuerza <= 0f)
            {
                valorFuerza = 0f;
                subiendo = true;
            }
        }

        sliderFuerza.value = valorFuerza;
    }
    
    private void GenerarZona()
    {
        zonaMin = Random.Range(0f, 1f - tamañoZona);
        zonaMax = zonaMin + tamañoZona;

        // Ajustar visualmente la zona verde
        zonaVerde.anchorMin = new Vector2(zonaMin, 0f);
        zonaVerde.anchorMax = new Vector2(zonaMax, 1f);
    }
    
    private void FinalizarJuego(bool victoria)
    {
        gameFinished = true;
        objetoMover.position = posicionFinal.position;
        if (victoria)
        {
            Debug.Log("¡Has ganado!");
            spriteObjetivo.color = Color.red;
            textoResultado.text = ":D";
            GameManager.instancia.Ganar();
        }
        else
        {
            Debug.Log("¡Has perdido!");
            textoResultado.text = ":(";
            GameManager.instancia.Perder();
        }
    }
    
    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}
