using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 10f;
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] public StoreObjectsInTrigger groundCheck;
	[SerializeField] public StoreObjectsInTrigger ceilingCheck;

	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	public CheckPoint checkpoint;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		RemoveControl();
	}



	private void FixedUpdate()
	{
		m_Grounded = groundCheck.objects.Count > 0;
	}

	public void Translate(Vector2 movement)
	{
		//m_Rigidbody2D.MovePosition(m_Rigidbody2D.position + movement);
		foreach (GameObject g in ceilingCheck.objects)
		{
			if (g.GetComponent<CharacterController2D>())
				g.GetComponent<CharacterController2D>().m_Rigidbody2D.position += movement;
		}
	}

    public void Move(float move, bool jump)
	{
		
		List<GameObject> ceiling = ceilingCheck.objects;
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * moveSpeed, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			foreach(GameObject g in ceiling)
			{
				if(g.GetComponent<CharacterController2D>())
				g.GetComponent<CharacterController2D>().m_Rigidbody2D.position += new Vector2(m_Rigidbody2D.velocity.x * Time.deltaTime, 0);
			}

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump && ceiling.Count <= 0)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

	public void RemoveControl()
    {
		m_Rigidbody2D.velocity = new Vector3(0, m_Rigidbody2D.velocity.y);
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX;
		m_Rigidbody2D.freezeRotation = true;
	}

	public void TakeControl()
    {
		m_Rigidbody2D.velocity = new Vector3(0, m_Rigidbody2D.velocity.y);
		m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
		m_Rigidbody2D.freezeRotation = true;
	}

	public void Die()
    {
		if(checkpoint)
        {
			transform.position = checkpoint.transform.position;
			m_Rigidbody2D.velocity = Vector3.zero;
        }
    }

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}