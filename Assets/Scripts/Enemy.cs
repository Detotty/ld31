using UnityEngine;

public class Enemy : MonoBehaviour
{

	public Bullet BulletPrefab;

	public int Health = 100;
	public float Acceleration = 1.0f;
	public float MaxVelocity = 1.0f;
	public float FollowPlayerDelay = 0.5f;
	public float RateOfFire = 4.0f;

	public Transform TransformCache { get; private set; }
	private Rigidbody2D rigidbodyCache;
	private Animator animator;
	private BoneEmitter boneEmitter;

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

	public void Kill()
	{
		arena.Score += Mathf.CeilToInt(arena.Timer * 100);

		boneEmitter.Emit();
		Explosion.Explode(TransformCache.position, Random.Range(1.0f, 2.0f), 0.5f);
		TimeScale.Set(0.75f, 0.05f);
		this.Recycle();
	}

	private void Shoot()
	{
		if (shootDelay > 0)
			return;

		Bullet bullet = BulletPrefab.Spawn();
		Vector2 offset = Random.insideUnitCircle;
		offset.x *= Mathf.Abs(offset.x);
		offset.y *= Mathf.Abs(offset.y);
		offset *= 1.5f;
		Vector3 shootAt = player.TransformCache.position + new Vector3(offset.x, offset.y);
		bullet.Initialize(shootAt - TransformCache.position, TransformCache.position);

		shootDelay = Random.Range(1.0f / RateOfFire, 2.0f / RateOfFire);
	}

	void Awake()
	{
		TransformCache = transform;
		rigidbodyCache = rigidbody2D;
		animator = GetComponent<Animator>();
		boneEmitter = GetComponent<BoneEmitter>();
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

			Vector3 scale = TransformCache.localScale;
			TransformCache.localScale = new Vector3(0, 0, 1.0f);

			if (followPlayerTarget.x < TransformCache.position.x)
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
		Vector3 p = TransformCache.position;
		p.z = p.y + arena.Rows;
		TransformCache.position = p;

		if (spawning || arena.Timer <= 0)
		{
			animator.SetBool("Idle", true);
			return;
		}

		animator.SetBool("Idle", false);

		followPlayerTimeElapsed += Time.deltaTime;
		if (followPlayerTimeElapsed >= FollowPlayerDelay)
		{
			followPlayerTimeElapsed -= FollowPlayerDelay;
			Vector2 pos = new Vector2(Random.Range(-arena.Columns + 1.5f, arena.Columns - 2.5f),
				Random.Range(-arena.Rows + 1.5f, arena.Rows - 2.5f));
			followPlayerTarget = pos;
		}

		Vector3 scale = TransformCache.localScale;
		if ((player.TransformCache.position.x < 0 && scale.x > 0) ||
			(player.TransformCache.position.x > 0 && scale.x < 0))
		{
			scale.x *= -1;
			TransformCache.localScale = scale;
		}

		shootDelay -= Time.deltaTime;
		shootDelay = Mathf.Max(shootDelay, 0);
		Shoot();
	}

	void FixedUpdate()
	{
		if (spawning || arena.Timer <= 0)
		{
			rigidbodyCache.velocity = Vector2.zero;
			return;
		}

		Vector3 dir = followPlayerTarget - TransformCache.position;
		dir.Normalize();
		rigidbodyCache.velocity += Vector2.ClampMagnitude(
			dir * Acceleration, Acceleration) * Time.fixedDeltaTime;
		rigidbodyCache.velocity = Vector2.ClampMagnitude(rigidbodyCache.velocity, MaxVelocity);
	}
}
