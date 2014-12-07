using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{

	private static Explosion prefab;

	public static void Explode(Vector2 pos, float radius, float duration)
	{
		if (prefab == null)
		{
			prefab = Resources.Load<Explosion>("Explosion");
			prefab.CreatePool();
		}

		Explosion expl = prefab.Spawn(pos);
		expl.Radius = radius;
		expl.Duration = duration;
		expl.Begin();

		CameraShake.Shake(radius / 10.0f, duration);
	}



	public Transform ExplosionSinglePrefab;
	public AudioClip Sound;

	public float Radius;
	public float Duration;

	private Transform transformCache;

	private float explodeCreateDelay;
	private int numOfExplosions;

	void CreateSingle()
	{
		Transform expl = ExplosionSinglePrefab.Spawn();
		expl.parent = transformCache;
		expl.localPosition = Random.insideUnitCircle * Radius;
		expl.localScale = new Vector3(0, 0, 1.0f);
		LeanTween.scale(expl.gameObject, new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), 1.0f),
			Random.Range(0.2f, 0.3f))
			.setEase(LeanTweenType.easeOutSine)
			.setOnComplete(OnExplodeSingleFinish, expl);

		// add some red
		expl.GetComponent<SpriteRenderer>().color = new HSBColor(0, Random.Range(0, 0.25f), 1.0f).ToColor();

		numOfExplosions--;
	}

	void OnExplodeSingleFinish(object explosion)
	{
		(explosion as Transform).Recycle();

		if (numOfExplosions <= 0)
		{
			this.Recycle();
		}
	}

	public void Begin()
	{
		ExplosionSinglePrefab.CreatePool();

		float delayMin = 0.1f / Radius;
		float delayMax = 0.15f / Radius;

		explodeCreateDelay = Random.Range(delayMax, delayMax);
		numOfExplosions = (int)Random.Range(Duration / delayMin, Duration / delayMax);

		CreateSingle();

		if (Sound != null)
		{
			AudioSource.PlayClipAtPoint(Sound, transformCache.position, 0.5f);
		}
	}

	void Awake()
	{
		transformCache = transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (numOfExplosions == 0)
		{
			return;
		}

		explodeCreateDelay -= Time.deltaTime;
		if (explodeCreateDelay <= 0)
		{
			explodeCreateDelay += Random.Range(0.05f, 0.15f);
			CreateSingle();
		}
	}
}
