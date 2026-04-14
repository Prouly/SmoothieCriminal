using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private float timer = 6f;    
    private bool gameFinished = false;

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }
    
    
    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timer);

        if (!gameFinished)
        {
            gameFinished = true;
            Debug.Log("¡Has perdido!");
            
        }
    }
}
