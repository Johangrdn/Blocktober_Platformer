using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = playerController.ControlledCharacter.transform.position;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, -10);
    }
}
