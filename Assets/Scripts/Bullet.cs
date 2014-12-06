using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

	public float Speed = 10.0f;
	public int Damage = 100;

	private Transform transformCache;
	private Rigidbody2D rigidbodyCache;

	public void Initialize(Vector2 dir, Vector3 pos)
	{
		transformCache.rotation = Quaternion.LookRotation(Vector3.forward, dir);
		transformCache.position = new Vector3(pos.x, pos.y, transformCache.position.z);
		transformCache.position += transformCache.up;

		rigidbodyCache.velocity = transformCache.up * Speed;
	}

	void Awake()
	{
		transformCache = transform;
		rigidbodyCache = rigidbody2D;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			other.GetComponent<Enemy>().Hit(Damage);
		}
		this.Recycle();
	}

}
