using System;
using System.Collections;
using UnityEngine;

public class PlataformaGameManager : MonoBehaviour
{
    [SerializeField] private float timer = 7f;
    
    [Header("Layouts disponibles")]
    [SerializeField] private GameObject[] layouts;
    
    [Header("Panel de Inicio")]
    public GameObject panelControles;
    public float tiempoEsperaIntro = 4f;
    private bool introFinalizada = false;
    
    #region Variables de Estado
    private bool gameFinished = false;
    private float tiempoRestante; 
    #endregion

    private void Start()
    {
        tiempoRestante = timer;
        
        // Lógica de inicio con panel
        if (panelControles != null)
        {
            StartCoroutine(SecuenciaIntro());
        }
        else
        {
            introFinalizada = true;
        }
    }

    private void Update()
    {
        if (!introFinalizada || gameFinished) return;
        
        tiempoRestante -= Time.deltaTime;
        
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinalizarJuego(false);
        }
        
    }
    
    // Corrutina para gestionar la espera inicial de 4 segundos
    IEnumerator SecuenciaIntro()
    {
        introFinalizada = false;
        panelControles.SetActive(true);
        
        yield return new WaitForSeconds(tiempoEsperaIntro);
        
        panelControles.SetActive(false);
        introFinalizada = true;
        ActivarLayoutAleatorio();
    }
    
    private void ActivarLayoutAleatorio()
    {
        if (layouts == null || layouts.Length == 0)
        {
            Debug.LogWarning("No hay layouts asignados.");
            return;
        }

        // Desactivar todos
        foreach (GameObject layout in layouts)
        {
            layout.SetActive(false);
        }

        // Elegir uno aleatorio
        int index = UnityEngine.Random.Range(0, layouts.Length);
        layouts[index].SetActive(true);
    }

    public void Win()
    {
        FinalizarJuego(true);
        Debug.Log("¡Has ganado!");
    }

    public void Lose()
    {
        FinalizarJuego(false);
        Debug.Log("Has perdido...");
    }
    
    private void FinalizarJuego(bool victoria)
    {
        gameFinished = true;
        if (victoria)
        {
            Debug.Log("¡Has ganado!");
            GameManager.instancia.Ganar();
        }
        else
        {
            Debug.Log("¡Has perdido!");
            
            GameManager.instancia.Perder();
        }
    }
    
    public bool ObtenerIntroFinalizada() => introFinalizada;
    
    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}