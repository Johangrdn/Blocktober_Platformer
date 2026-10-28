using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 10f;

	[Header("Saut en hauteur (sur place)")]
	[FormerlySerializedAs("m_JumpForce")]
	[SerializeField] private float highJumpForce = 400f;                        // Force verticale du saut sur place

	[Header("Saut en longueur (en mouvement)")]
	[SerializeField] private float longJumpForce = 280f;                        // Force verticale du saut en longueur (plus bas)
	[SerializeField] private float longJumpHorizontalSpeed = 14f;               // Vitesse horizontale pendant le saut en longueur
	[Range(0f, 1f)] [SerializeField] private float movingThreshold = 0.2f;     // Input minimum pour etre considere "en mouvement"
	[SerializeField] private float longJumpAirAcceleration = 40f;              // Reactivite du controle en l'air pendant le saut en longueur

	[Header("Deplacement")]
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] public StoreObjectsInTrigger groundCheck;
	[SerializeField] public StoreObjectsInTrigger ceilingCheck;

	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool m_IsLongJumping;       // Vrai pendant un saut en longueur, jusqu'a l'atterrissage
	private float m_JumpTime;           // Moment du dernier saut (evite de "re-atterrir" la frame du saut)

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

		// Fin du saut en longueur a l'atterrissage
		if (m_IsLongJumping && m_Grounded && Time.time - m_JumpTime > 0.1f && m_Rigidbody2D.velocity.y <= 0.01f)
			m_IsLongJumping = false;
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
		if (m_IsLongJumping)
		{
			// Pendant un saut en longueur : on peut corriger la trajectoire en l'air
			float targetX = move * longJumpHorizontalSpeed;
			float newX = Mathf.MoveTowards(m_Rigidbody2D.velocity.x, targetX, longJumpAirAcceleration * Time.deltaTime);
			m_Rigidbody2D.velocity = new Vector2(newX, m_Rigidbody2D.velocity.y);

			if (move > 0 && !m_FacingRight)
				Flip();
			else if (move < 0 && m_FacingRight)
				Flip();
		}
		else if (m_Grounded || m_AirControl)
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
			m_Grounded = false;
			m_JumpTime = Time.time;

			if (Mathf.Abs(move) > movingThreshold)
			{
				// En mouvement : saut en longueur (moins haut, mais loin)
				m_IsLongJumping = true;
				float direction = Mathf.Sign(move);
				m_Rigidbody2D.velocity = new Vector2(direction * longJumpHorizontalSpeed, 0f);
				m_Rigidbody2D.AddForce(new Vector2(0f, longJumpForce));
			}
			else
			{
				// Sur place : saut en hauteur
				m_IsLongJumping = false;
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
				m_Rigidbody2D.AddForce(new Vector2(0f, highJumpForce));
			}
		}
	}

	public void RemoveControl()
    {
		m_IsLongJumping = false;
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
			m_IsLongJumping = false;
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