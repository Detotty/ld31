using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Arena : MonoBehaviour
{

	public Transform FloorPrefab;
	public Transform WallPrefab;
	public Enemy EnemyPrefab;

	public int Rows = 10;
	public int Columns = 10;

	private Transform transformCache;

	private List<Transform> walls;
	private List<Enemy> enemies;

	public void Expand()
	{
		Rows += 2;
		Columns += 2;

		CreateWalls();
		LeanTween.value(gameObject, ExpandCameraSize, Camera.main.orthographicSize, Mathf.Min(Rows, Columns) / 2, 1.0f)
			.setEase(LeanTweenType.easeOutCubic);
	}

	private void ExpandCameraSize(float value)
	{
		Camera.main.orthographicSize = value;
	}

	private void CreateWalls()
	{
		foreach (Transform wall in walls)
			wall.Recycle();
		walls.Clear();

		for (int row = 0; row < Rows; row++)
		{
			if (row == 0 || row == Rows - 1)
			{
				for (int col = 1; col < Columns - 1; col++)
				{
					CreateWallAt(row, col);
				}
			}
			CreateWallAt(row, 0);
			CreateWallAt(row, Columns - 1);
		}
	}

	private void CreateWallAt(int row, int column)
	{
		Transform wall = WallPrefab.Spawn(transformCache,
			new Vector3(column - Columns / 2 + 0.5f, row - Rows / 2 + 0.5f));
		walls.Add(wall);
	}

	private void SpawnEnemies()
	{
		int numOfEnemies = Rows * Columns / 25; // simple formula first...

		// subdivisions. 4, 4, 8, 16, 32...
		// add up: 4, 8, 16, 32
		List<float> spawnPoints = new List<float>();
		List<int> spawnIndexes = new List<int>() { 0, 1, 2, 3 };
		for (int i = 0; i < numOfEnemies; i++)
		{
			if (i < 4)
			{
				// spawn in corners!
				int random = Random.Range(0, spawnIndexes.Count);
				int spawnIndex = spawnIndexes[random];
				spawnIndexes.RemoveAt(random);
				SpawnEnemy(spawnPoints, spawnIndex);

				if (i == 3)
				{
					spawnPoints.Add(0.5f);
					for (int j = 0; j < 4; j++)
						spawnIndexes.Add(j);
				}
			}
			else if (i < 8)
			{
				// spawn in wall mids
				int random = Random.Range(0, spawnIndexes.Count);
				int spawnIndex = spawnIndexes[random];
				spawnIndexes.RemoveAt(random);
				SpawnEnemy(spawnPoints, spawnIndex);
			}
			else
			{
				// check if is power of two, in this case 8, 16, 32...
				// would signify the start of each subdivision
				if ((i & (i - 1)) == 0)
				{
					float offset = spawnPoints[0] / 2.0f;
					float count = spawnPoints.Count;
					for (int j = 0; j < count; j++)
					{
						spawnPoints.Add(spawnPoints[j] + offset);
						spawnPoints[j] -= offset;
					}
					for (int j = 0; j < i; j++)
						spawnIndexes.Add(j);
				}

				int random = Random.Range(0, spawnIndexes.Count);
				int spawnIndex = spawnIndexes[random];
				spawnIndexes.RemoveAt(random);
				SpawnEnemy(spawnPoints, spawnIndex);
			}
		}
	}

	private void SpawnEnemy(List<float> spawnPoints, int spawnIndex)
	{
		int side = 0;
		float spawnPoint = 0;

		if (spawnPoints.Count == 0)
			side = spawnIndex;
		else
		{
			bool endLoop = false;
			for (int i = 0; i < 4; i++)
			{
				foreach (float p in spawnPoints)
				{
					if (spawnIndex == 0)
					{
						spawnPoint = p;
						endLoop = true;
						break;
					}
					spawnIndex--;
				}

				if (endLoop)
					break;

				side++;
			}
		}

		spawnPoint -= 0.5f;
		Vector2 pos = new Vector2(spawnPoint * Columns, spawnPoint * Rows);

		switch (side)
		{
			case 0:
				pos.y = Rows / 2 - 1.5f;
				if (spawnPoints.Count == 0)
					pos.x = -Columns / 2 + 1.5f;
				break;
			case 1:
				pos.y = -Rows / 2 + 1.5f;
				if (spawnPoints.Count == 0)
					pos.x = Columns / 2 - 1.5f;
				break;
			case 2:
				pos.x = Columns / 2 - 1.5f;
				if (spawnPoints.Count == 0)
					pos.y = Rows / 2 - 1.5f;
				break;
			case 3:
				pos.x = -Columns / 2 + 1.5f;
				if (spawnPoints.Count == 0)
					pos.y = -Rows / 2 + 1.5f;
				break;
		}

		enemies.Add(EnemyPrefab.Spawn(pos));
	}

	void Awake()
	{
		transformCache = transform;
	}

	// Use this for initialization
	void Start()
	{
		walls = new List<Transform>();
		enemies = new List<Enemy>();

		WallPrefab.CreatePool();
		EnemyPrefab.CreatePool();

		CreateWalls();
		SpawnEnemies();

		Camera.main.orthographicSize = Mathf.Min(Rows, Columns) / 2;

		Expand();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
