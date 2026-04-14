using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private float inputTime = 3f; // Tiempo para que el jugador haga la secuencia

    private List<KeyCode> sequence;    // La secuencia generada
    private int currentStep = 0;       // Paso actual del jugador
    private bool waitingForInput = false;

    void Start()
    {
        GenerateSequence();
        StartCoroutine(WaitForPlayer());
    }

    // Genera 3 direcciones aleatorias: W, A, S, D
    private void GenerateSequence()
    {
        sequence = new List<KeyCode>();
        KeyCode[] directions = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, directions.Length);
            sequence.Add(directions[index]);
        }

        // Mostrar la secuencia en la consola
        string seqStr = "";
        foreach (var key in sequence)
        {
            seqStr += key.ToString() + " ";
        }
        Debug.Log("Secuencia generada: " + seqStr);
    }

    private IEnumerator WaitForPlayer()
    {
        currentStep = 0;
        waitingForInput = true;
        float timer = inputTime;

        while (timer > 0 && waitingForInput)
        {
            timer -= Time.deltaTime;

            // Detectar input solo de WASD
            KeyCode? pressedKey = null;
            if (Input.GetKeyDown(KeyCode.W)) pressedKey = KeyCode.W;
            else if (Input.GetKeyDown(KeyCode.A)) pressedKey = KeyCode.A;
            else if (Input.GetKeyDown(KeyCode.S)) pressedKey = KeyCode.S;
            else if (Input.GetKeyDown(KeyCode.D)) pressedKey = KeyCode.D;

            if (pressedKey.HasValue)
            {
                if (CheckInput(pressedKey.Value))
                {
                    currentStep++;
                    if (currentStep >= sequence.Count)
                    {
                        GameManager.instancia.Ganar();
                        Debug.Log("¡Correcto! Has completado la secuencia.");
                        waitingForInput = false;
                    }
                }
                else
                {
                    GameManager.instancia.Perder();
                    Debug.Log("¡Error! Has fallado la secuencia.");
                    waitingForInput = false;
                }
            }

            yield return null;
        }

        if (waitingForInput)
        {
            GameManager.instancia.Perder();
            Debug.Log("¡Se acabó el tiempo! Has perdido.");
            waitingForInput = false;
        }
    }

    // Compara la tecla presionada con la secuencia actual
    private bool CheckInput(KeyCode keyPressed)
    {
        KeyCode expectedKey = sequence[currentStep];
        return keyPressed == expectedKey;
    }
}