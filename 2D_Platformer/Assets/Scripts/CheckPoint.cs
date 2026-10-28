using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private ElementColor forColor;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ColoredCharacter character = collision.GetComponent<ColoredCharacter>();
        CharacterController2D controller = collision.GetComponent<CharacterController2D>();
        if(character && controller && character.color == forColor)
        {
            controller.checkpoint = this;
        }
    }
}
