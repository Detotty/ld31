using UnityEngine;
using System.Collections;

public class BoneEmitter : MonoBehaviour
{

	public Bone BonePrefab;

	private Transform transformCache;

	public void Emit()
	{
		int count = Random.Range(0, 5);
		while (count-- > 0)
		{
			Vector2 pos = transformCache.position;
			pos += Random.insideUnitCircle;
			BonePrefab.Spawn(pos, Quaternion.AngleAxis(Random.Range(0, 360.0f), Vector3.forward));
		}
	}

	void Awake()
	{
		BonePrefab.CreatePool();

		transformCache = transform;
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
