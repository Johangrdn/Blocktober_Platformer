using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{
    public GameObject startPoint;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D pCollision)
    {
        if (pCollision.gameObject.CompareTag("Player"))
        {
            player.transform.position = startPoint.transform.position;
        }
    }
}
