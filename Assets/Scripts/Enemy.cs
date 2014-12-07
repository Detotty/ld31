using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

	public Bullet BulletPrefab;

	public int Health = 100;
	public float Acceleration = 1.0f;
	public float MaxVelocity = 1.0f;
	public float FollowPlayerDelay = 0.5f;
	public float RateOfFire = 4.0f;

	private Transform transformCache;
	private Rigidbody2D rigidbodyCache;

	private Player player;
	private Arena arena;

	private bool started;
	private float followPlayerTimeElapsed;
	private Vector3 followPlayerTarget;
	private float shootDelay;
	private bool spawning;

	public void Hit(int damage)
	{
		if (spawning || arena.Timer <= 0)
			return;

		Health -= damage;
		if (Health < damage)
			Kill();
	}

	private void Kill()
	{
		Explosion.Explode(transformCache.position, Random.Range(1.0f, 2.0f), 0.5f);
		TimeScale.Set(0.5f, 0.1f);
		this.Recycle();
	}

	private void Shoot()
	{
		if (shootDelay > 0 || arena.Timer <= 0)
			return;

		Bullet bullet = BulletPrefab.Spawn();
		bullet.Initialize(player.TransformCache.position - transformCache.position, transformCache.position);

		shootDelay = Random.Range(1.0f / RateOfFire, 2.0f / RateOfFire);
	}

	void Awake()
	{
		transformCache = transform;
		rigidbodyCache = rigidbody2D;
	}

	// Use this for initialization
	void Start()
	{
		started = true;
		player = ObjectCache.Get<Player>("Player");
		arena = ObjectCache.Get<Arena>("Arena");
		BulletPrefab.CreatePool();

		OnEnable();
	}

	void OnEnable()
	{
		if (started)
		{
			followPlayerTarget = player.TransformCache.position;
			shootDelay = Random.Range(0.5f, 1.0f);
			spawning = true;

			Vector3 scale = transformCache.localScale;
			transformCache.localScale = new Vector3(0, 0, 1.0f);

			if (followPlayerTarget.x < transformCache.position.x)
			{
				scale.x *= -1;
			}

			LeanTween.scale(gameObject, scale, 1.0f)
				.setEase(LeanTweenType.easeOutSine)
				.setOnComplete(() => spawning = false);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (spawning)
			return;

		followPlayerTimeElapsed += Time.deltaTime;
		if (followPlayerTimeElapsed >= FollowPlayerDelay)
		{
			followPlayerTimeElapsed -= FollowPlayerDelay;
			Vector2 offset = Random.insideUnitCircle * 5.0f;
			followPlayerTarget = player.TransformCache.position + new Vector3(offset.x, offset.y);
		}

		Vector3 scale = transformCache.localScale;
		if ((player.TransformCache.position.x < 0 && scale.x > 0) ||
			(player.TransformCache.position.x > 0 && scale.x < 0))
		{
			scale.x *= -1;
			transformCache.localScale = scale;
		}

		shootDelay -= Time.deltaTime;
		shootDelay = Mathf.Max(shootDelay, 0);
		Shoot();
	}

	void FixedUpdate()
	{
		if (spawning)
			return;

		Vector3 dir = followPlayerTarget - transformCache.position;
		dir.Normalize();
		rigidbodyCache.velocity += Vector2.ClampMagnitude(
			dir * Acceleration, Acceleration) * Time.fixedDeltaTime;
		rigidbodyCache.velocity = Vector2.ClampMagnitude(rigidbodyCache.velocity, MaxVelocity);
	}
}
