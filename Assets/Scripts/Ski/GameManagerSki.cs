using System;
using UnityEngine;

public class GameManagerSki : MonoBehaviour
{
    [SerializeField] private float timer = 7f;
    
    #region Variables de Estado
    private bool gameFinished = false;
    private float tiempoRestante; 
    #endregion
    
    private void Start()
    {
        tiempoRestante = timer;
    }

    private void Update()
    {
        if (gameFinished) return;
        tiempoRestante -= Time.deltaTime;
        
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            FinalizarJuego(false);
        }
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
    
    #region Getters para UI
    public float ObtenerTiempoLimite() => timer;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
    #endregion
}