using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public int Health = 100;

	public void Hit(int damage)
	{
		Health -= damage;
		if (Health < damage)
			Kill();
	}

	private void Kill()
	{
		Destroy(gameObject);
		// TODO: spawn explosion!
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
