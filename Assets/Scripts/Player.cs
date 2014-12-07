using UnityEngine;

public class Player : MonoBehaviour
{

	public Bullet BulletPrefab;
	public Arena Arena;

	public int Health = 100;
	public float Acceleration = 1.0f;
	public float MaxVelocity = 1.0f;
	public float RateOfFire = 4.0f;

	private Rigidbody2D rigidbodyCache;
	private Animator animator;

	public Transform TransformCache { get; private set; }

	private int horizontal;
	private int vertical;
	private Vector2 direction;

	private float shootDelay;

	public void Hit(int damage)
	{
		//Health -= damage;
		Arena.Timer -= 1.0f;
		Arena.Score -= 100;
		Arena.TimerText.rectTransform.localScale = new Vector3(1.5f, 1.5f);
		LeanTween.scale(Arena.TimerText.rectTransform, Vector3.one, 0.1f).setEase(LeanTweenType.easeInExpo);
		//if (Health < damage)
		//Kill();
	}

	public void Kill()
	{
		LeanTween.scale(gameObject, new Vector3(0, 0, 1.0f), 0.5f).setDestroyOnComplete(true);
	}

	private void Shoot()
	{
		if (shootDelay > 0)
			return;

		Bullet bullet = BulletPrefab.Spawn();
		bullet.Initialize(direction, TransformCache.position);

		shootDelay = 1.0f / RateOfFire;

		CameraShake.Shake(0.1f, 0.1f);
	}

	void Awake()
	{
		rigidbodyCache = rigidbody2D;
		TransformCache = transform;
		animator = GetComponent<Animator>();
	}

	// Use this for initialization
	void Start()
	{
		BulletPrefab.CreatePool();
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.timeScale == 0)
			return;

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");
		direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - TransformCache.position;

		shootDelay -= Time.deltaTime;
		shootDelay = Mathf.Max(shootDelay, 0);

		if (Input.GetAxisRaw("Fire") == 1.0f)
			Shoot();

		Vector3 scale = TransformCache.localScale;
		if (direction.x < 0 && scale.x > 0 || (direction.x > 0 && scale.x < 0))
		{
			scale.x *= -1;
			TransformCache.localScale = scale;
		}
		animator.SetBool("Idle", horizontal == 0 && vertical == 0);

		Vector3 pos = TransformCache.position;
		pos.z = pos.y + Arena.Rows;
		TransformCache.position = pos;
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