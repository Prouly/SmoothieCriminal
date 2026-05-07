using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Proyecto: Smoothie Criminal
 * Autor: Álvaro Muñoz Adán
 * Descripción: Lógica del minijuego Cartas.
 * Última modificación: 07/05/2026
 */

public class CartasGameLogic : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Carta[] cartas; // Las 6 cartas de la escena
    [SerializeField] private Sprite[] spritesFrontales; // Los 3 sprites diferentes de cada carta
    [SerializeField] private float tiempoMemorizacion = 3f;
    [SerializeField] private float tiempoLimite = 7f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioCorrecto;
    [SerializeField] private AudioSource audioIncorrecto;

    private float tiempoRestante;
    private bool juegoIniciado = false;
    private bool juegoFinalizado = false;
    public bool PuedeJugar { get; private set; } = false;

    private Carta primeraCarta;
    private Carta segundaCarta;
    private int parejasEncontradas = 0;

    private void Start()
    {
        tiempoRestante = tiempoLimite;
        ConfigurarTablero();
        StartCoroutine(SecuenciaInicial());
    }

    private void ConfigurarTablero()
    {
        // Creamos una lista con los IDs de las parejas (0,0, 1,1, 2,2)
        List<int> ids = new List<int> { 0, 0, 1, 1, 2, 2 };
        
        // Mezclamos la lista (Shuffle)
        for (int i = 0; i < ids.Count; i++)
        {
            int temp = ids[i];
            int randomIndex = Random.Range(i, ids.Count);
            ids[i] = ids[randomIndex];
            ids[randomIndex] = temp;
        }

        // Asignamos a cada objeto Carta su ID y su Sprite
        for (int i = 0; i < cartas.Length; i++)
        {
            cartas[i].Configurar(ids[i], spritesFrontales[ids[i]], this);
        }
    }

    private IEnumerator SecuenciaInicial()
    {
        // 1. Mostrar cartas durante X segundos
        yield return new WaitForSeconds(tiempoMemorizacion);

        // 2. Voltear cartas (Animación de reverso)
        foreach (Carta c in cartas) c.Ocultar();
        
        // 3. Comenzar el juego y el timer
        PuedeJugar = true;
        juegoIniciado = true;
        Debug.Log("¡Empieza el tiempo! Encuentra las parejas.");
    }

    private void Update()
    {
        if (!juegoIniciado || juegoFinalizado) return;

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0)
        {
            FinalizarJuego(false);
        }
    }

    public void CartaSeleccionada(Carta carta)
    {
        if (primeraCarta == null)
        {
            primeraCarta = carta;
        }
        else
        {
            segundaCarta = carta;
            StartCoroutine(ComprobarPareja());
        }
    }

    private IEnumerator ComprobarPareja()
    {
        PuedeJugar = false; // Bloqueamos clicks mientras comprobamos

        if (primeraCarta.idPareja == segundaCarta.idPareja)
        {
            // ACIERTO
            Debug.Log("<color=green>¡Pareja Correcta!</color>");
            if (audioCorrecto != null) audioCorrecto.Play();
            parejasEncontradas++;

            if (parejasEncontradas >= 3)
            {
                FinalizarJuego(true);
            }
        }
        else
        {
            // FALLO
            Debug.Log("<color=red>Pareja Incorrecta...</color>");
            if (audioIncorrecto != null) audioIncorrecto.Play();
            
            yield return new WaitForSeconds(0.6f); // Pequeña espera para ver la carta
            primeraCarta.Ocultar();
            segundaCarta.Ocultar();
        }

        primeraCarta = null;
        segundaCarta = null;
        if (!juegoFinalizado) PuedeJugar = true;
    }

    private void FinalizarJuego(bool victoria)
    {
        juegoFinalizado = true;
        PuedeJugar = false;

        if (victoria)
        {
            Debug.Log("<color=green>¡HAS GANADO!</color>");
            if (GameManager.instancia != null) GameManager.instancia.Ganar();
        }
        else
        {
            Debug.Log("<color=red>¡TIEMPO AGOTADO!</color>");
            if (GameManager.instancia != null) GameManager.instancia.Perder();
        }
    }

    public float ObtenerTiempoLimite() => tiempoLimite;
    public float ObtenerTiempoRestante() => Mathf.Max(0f, tiempoRestante);
}