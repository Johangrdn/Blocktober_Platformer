using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public Transform position_1;
    public Transform position_2;

    public float speed = 5f;

    private List<GameObject> touchingObjects = new();

    [SerializeField]
    public bool isMoving;

    [SerializeField]
    private bool reverse = false;

    void Update()
    {
        if (!isMoving) return;

        Vector3 oldPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, (reverse?position_1:position_2).position, speed * Time.deltaTime);

        if(transform.position == (reverse ? position_1 : position_2).position)
            reverse = !reverse;

        Vector3 displacement = transform.position - oldPosition;

        foreach(GameObject obj in touchingObjects)
        {
            if (obj.transform.position.y <= transform.position.y) continue;

            obj.transform.position += displacement;
            if(obj.GetComponent<CharacterController2D>())
                obj.GetComponent<CharacterController2D>().Translate(displacement);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        touchingObjects.Add(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        touchingObjects.Remove(collision.gameObject);
    }

    public void StartMoving()
    {
        isMoving = true;
    }
    public void StopMoving()
    {
        isMoving = false;
    }
}
