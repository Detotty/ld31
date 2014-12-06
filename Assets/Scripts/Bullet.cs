using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

	public float Speed = 10.0f;
	public int Damage = 100;

	// Use this for initialization
	void Start()
	{

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
	}

}
