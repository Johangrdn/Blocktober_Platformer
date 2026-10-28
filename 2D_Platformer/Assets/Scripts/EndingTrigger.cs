using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<ColoredCharacter> OnEntered;
    [SerializeField]
    public UnityEvent<ColoredCharacter> OnExited;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<ColoredCharacter>())
        {
            OnEntered.Invoke(collision.GetComponent<ColoredCharacter>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<ColoredCharacter>())
        {
            OnExited.Invoke(collision.GetComponent<ColoredCharacter>());
        }
    }
}
