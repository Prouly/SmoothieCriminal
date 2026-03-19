using System.Collections;
using UnityEngine;

public class BalloonPop : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;    
    [SerializeField] private float scaleIncrement = 0.1f;
    [SerializeField] private int pressesToPop = 10;        
    [SerializeField] private float timer = 5f;            
    private int spaceCount = 0;  
    private bool gameFinished = false;

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && targetObject != null && !gameFinished)
        {
            spaceCount++;
            
            targetObject.transform.localScale += new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
            
            if (spaceCount >= pressesToPop)
            {
                targetObject.SetActive(false); 
                gameFinished = true;
                Debug.Log("¡Has ganado!");
            }
        }
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(timer);

        if (!gameFinished)
        {
            gameFinished = true;
            if (targetObject != null && targetObject.activeSelf)
            {
                Debug.Log("¡Has perdido!");
            }
        }
    }
}
