using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 10f;

	[Header("Saut en hauteur (sur place)")]
	[FormerlySerializedAs("m_JumpForce")]
	[SerializeField] private float highJumpForce = 400f;

	[Header("Saut en longueur (en mouvement)")]
	[SerializeField] private float longJumpForce = 280f;
	[SerializeField] private float longJumpHorizontalSpeed = 14f;
	[Range(0f, 1f)] [SerializeField] private float movingThreshold = 0.2f;
	[SerializeField] private float longJumpAirAcceleration = 40f;

	[Header("Deplacement")]
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
	[SerializeField] private bool m_AirControl = false;
	[SerializeField] public StoreObjectsInTrigger groundCheck;
	[SerializeField] public StoreObjectsInTrigger ceilingCheck;

	private bool m_Grounded;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;
	private Vector3 m_Velocity = Vector3.zero;
	private bool m_IsLongJumping;
	private float m_JumpTime;

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

		if (m_IsLongJumping && m_Grounded && Time.time - m_JumpTime > 0.1f && m_Rigidbody2D.velocity.y <= 0.01f)
			m_IsLongJumping = false;
	}

	public void Translate(Vector2 movement)
	{
		foreach (GameObject g in ceilingCheck.objects)
		{
			if (g.GetComponent<CharacterController2D>())
				g.GetComponent<CharacterController2D>().m_Rigidbody2D.position += movement;
		}
	}

    public void Move(float move, bool jump)
	{
		List<GameObject> ceiling = ceilingCheck.objects;
		if (m_IsLongJumping)
		{
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
			Vector3 targetVelocity = new Vector2(move * moveSpeed, m_Rigidbody2D.velocity.y);
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			foreach(GameObject g in ceiling)
			{
				if(g.GetComponent<CharacterController2D>())
				g.GetComponent<CharacterController2D>().m_Rigidbody2D.position += new Vector2(m_Rigidbody2D.velocity.x * Time.deltaTime, 0);
			}

			if (move > 0 && !m_FacingRight)
			{
				Flip();
			}
			else if (move < 0 && m_FacingRight)
			{
				Flip();
			}
		}
		if (m_Grounded && jump && ceiling.Count <= 0)
		{
			m_Grounded = false;
			m_JumpTime = Time.time;

			if (Mathf.Abs(move) > movingThreshold)
			{
				m_IsLongJumping = true;
				float direction = Mathf.Sign(move);
				m_Rigidbody2D.velocity = new Vector2(direction * longJumpHorizontalSpeed, 0f);
				m_Rigidbody2D.AddForce(new Vector2(0f, longJumpForce));
			}
			else
			{
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
		m_FacingRight = !m_FacingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}