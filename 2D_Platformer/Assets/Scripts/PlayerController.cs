using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController2D controlledCharacter;

    [SerializeField] private List<CharacterController2D> charactersList = new();

    public CharacterController2D ControlledCharacter { get { return controlledCharacter; } }
    // Start is called before the first frame update
    void Start()
    {
        controlledCharacter.TakeControl();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("SwapCharacter"))
        {
            controlledCharacter.RemoveControl();

            int index = charactersList.IndexOf(controlledCharacter);
            index = (index + 1) % charactersList.Count;
            controlledCharacter = charactersList[index];

            controlledCharacter.TakeControl();
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        bool jump = Input.GetButtonDown("Jump");

        controlledCharacter.Move(horizontalInput, jump);
    }
}
