using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

	public float Acceleration = 1.0f;
	public float MaxVelocity = 1.0f;

	private Rigidbody2D rigidbodyCache;

	private int horizontal;
	private int vertical;

	void Awake()
	{
		rigidbodyCache = rigidbody2D;
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

	}

	void FixedUpdate()
	{
		if (horizontal == 0 && vertical == 0)
			rigidbodyCache.velocity =
				Vector2.MoveTowards(rigidbodyCache.velocity, Vector2.zero, Acceleration * Time.fixedDeltaTime);
		rigidbodyCache.velocity += Vector2.ClampMagnitude(
			new Vector2(horizontal, vertical) * Acceleration, Acceleration) * Time.fixedDeltaTime;
		rigidbodyCache.velocity = Vector2.ClampMagnitude(rigidbodyCache.velocity, MaxVelocity);
	}

}