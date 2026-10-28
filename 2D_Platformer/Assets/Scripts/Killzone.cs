using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<CharacterController2D>())
			collision.GetComponent<CharacterController2D>().Die();
	}
}
