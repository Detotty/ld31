using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

	public AudioClip Sound;

	public float Speed = 10.0f;
	public int Damage = 100;

	private Transform transformCache;
	private Rigidbody2D rigidbodyCache;

	public void Initialize(Vector2 dir, Vector3 pos)
	{
		transformCache.rotation = Quaternion.LookRotation(Vector3.forward, dir);
		transformCache.position = new Vector3(pos.x, pos.y, transformCache.position.z);
		transformCache.position += transformCache.up * 0.5f;

		rigidbodyCache.velocity = transformCache.up * Speed;
	}

	void Awake()
	{
		transformCache = transform;
		rigidbodyCache = rigidbody2D;
	}

	void OnEnable()
	{
		if (Sound != null)
		{
			AudioSource.PlayClipAtPoint(Sound, transformCache.position, 0.25f);
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (CompareTag("Bullet"))
		{
			if (other.CompareTag("Enemy"))
				other.GetComponent<Enemy>().Hit(Damage);
			else if (other.CompareTag("Player"))
				return;
		}
		if (CompareTag("EnemyBullet"))
		{
			if (other.CompareTag("Player"))
				other.GetComponent<Player>().Hit(Damage);
			else if (other.CompareTag("Enemy"))
				return;
		}

		if (!other.CompareTag("Bullet") && !other.CompareTag("EnemyBullet"))
			this.Recycle();
	}

}
