using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObjectsInTrigger : MonoBehaviour
{
	public List<GameObject> objects = new();
	private void OnTriggerEnter2D(Collider2D collision)
	{
		objects.Add(collision.gameObject);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		objects.Remove(collision.gameObject);
	}
}
