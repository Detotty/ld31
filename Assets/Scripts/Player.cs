using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

	public Bullet BulletPrefab;

	public float Acceleration = 1.0f;
	public float MaxVelocity = 1.0f;

	private Rigidbody2D rigidbodyCache;
	private Transform transformCache;

	private int horizontal;
	private int vertical;

	private void Shoot()
	{
		Bullet bullet = BulletPrefab.Spawn();
		bullet.Initialize(transformCache.up, transformCache.position);
	}

	void Awake()
	{
		rigidbodyCache = rigidbody2D;
		transformCache = transform;
	}

	// Use this for initialization
	void Start()
	{
		BulletPrefab.CreatePool();
	}

	// Update is called once per frame
	void Update()
	{
		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		transformCache.rotation = Quaternion.LookRotation(
			Vector3.forward, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transformCache.position);

		if (Input.GetAxisRaw("Fire") == 1.0f)
			Shoot();
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