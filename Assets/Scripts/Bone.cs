using UnityEngine;

public class Bone : MonoBehaviour
{

	public float Lifetime = 60.0f;

	private Rigidbody2D rigidbodyCache;
	private Transform transformCache;
	private SpriteRenderer spriteRenderer;
	private Arena arena;

	private float lifetimeElapsed;

	void Awake()
	{
		rigidbodyCache = rigidbody2D;
		transformCache = transform;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		arena = ObjectCache.Get<Arena>("Arena");
	}

	void OnEnable()
	{
		lifetimeElapsed = 0;

		Vector2 vel = Random.insideUnitCircle;
		vel.x *= Mathf.Abs(vel.x);
		vel.y *= Mathf.Abs(vel.y);
		vel *= 10.0f;
		rigidbodyCache.velocity = vel;

		rigidbodyCache.angularVelocity = Mathf.Pow(Random.Range(-45.0f, 45.0f), 2);
	}

	void Update()
	{
		Vector3 pos = transformCache.position;
		if ((pos.x <= -arena.Columns / 2 + 1)
			|| (pos.x >= arena.Columns / 2 - 1)
			|| (pos.y <= -arena.Rows / 2 + 1)
			|| (pos.y >= arena.Rows / 2 - 1))
			spriteRenderer.enabled = false;
		else
		{
			spriteRenderer.enabled = true;
		}

		lifetimeElapsed += Time.deltaTime;
		if (lifetimeElapsed >= Lifetime)
		{
			this.Recycle();
			return;
		}

		if (rigidbodyCache.velocity == Vector2.zero)
			rigidbodyCache.angularVelocity = 0;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		rigidbodyCache.velocity = Vector2.zero;
		rigidbodyCache.angularVelocity = 0;
		rigidbodyCache.isKinematic = true; // turn off collision
	}

}
