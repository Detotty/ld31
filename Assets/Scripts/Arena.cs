using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Arena : MonoBehaviour
{

	public Transform Floor;
	public Wall WallPrefab;
	public Enemy EnemyPrefab;
	public Player Player;
	public Text TimerText;

	public int Rows = 10;
	public int Columns = 10;

	public float Timer = 10.0f;
	public int Score = 0;

	private Transform transformCache;

	private List<Wall> walls;
	private List<Wall> wallsDestroying;
	private List<Enemy> enemies;

	private int level;
	private int highestRows;

	private bool destroyingWalls;
	private float destroyWallsDelay;
	private bool willCompress;
	private bool gameOver;

	private void Expand()
	{
		Rows += 2;
		Columns += 2;
		willCompress = false;

		if (Rows > highestRows)
			highestRows = Rows;

		DestroyWalls();
		CreateWalls();
	}

	private void Compress()
	{
		Rows -= 2;
		Columns -= 2;
		willCompress = true;

		DestroyWalls();
		CreateWalls();
	}

	private void ExpandCameraSize(float value)
	{
		Camera.main.orthographicSize = value;
	}

	private void DestroyWalls()
	{
		destroyingWalls = true;
		destroyWallsDelay = 0.1f;
		wallsDestroying = new List<Wall>(walls);
	}

	private void CreateWalls()
	{
		walls.Clear();

		// have to create walls clockwise so that when they get destroyed they'll be in order...
		for (int col = 0; col < Columns; col++)
			CreateWallAt(0, col);
		for (int row = 1; row < Rows - 1; row++)
			CreateWallAt(row, Columns - 1);
		for (int col = Columns - 1; col >= 0; col--)
			CreateWallAt(Rows - 1, col);
		for (int row = Rows - 2; row > 0; row--)
			CreateWallAt(row, 0);
	}

	private void CreateWallAt(int row, int column)
	{
		if (row == 0)
		{
			Wall wall = WallPrefab.Spawn(transformCache,
				new Vector3(column - Columns / 2 + 0.5f, -row + Rows / 2 - 0.5f, Rows));
			walls.Add(wall);
			wall.Back();
		}
		else
		{
			Wall wall = WallPrefab.Spawn(transformCache,
				new Vector3(column - Columns / 2 + 0.5f, -row + Rows / 2 - 0.5f, (float)-row / Rows));
			walls.Add(wall);
			wall.Front();
		}
	}

	private void SpawnEnemies()
	{
		enemies.Clear();

		//int numOfEnemies = Rows * Columns / 25; // simple formula first...
		int numOfEnemies = Mathf.CeilToInt(Mathf.Pow(Mathf.Sin(level * 2 + 10) / 2 + (level * 2 + 10) / 4.0f, 1.5f));

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
		walls = new List<Wall>();
		enemies = new List<Enemy>();

		WallPrefab.CreatePool();
		EnemyPrefab.CreatePool();

		CreateWalls();
		SpawnEnemies();
		foreach (Wall wall in walls)
		{
			wall.Spawn();
		}

		highestRows = Rows;
		Floor.localScale = new Vector3(Columns, Rows, 1.0f);
		Camera.main.orthographicSize = Mathf.Min(Rows, Columns) / 2;
	}

	// Update is called once per frame
	void Update()
	{
		if (destroyingWalls)
		{
			destroyWallsDelay -= Time.deltaTime;
			if (destroyWallsDelay <= 0)
			{
				destroyWallsDelay += 0.1f;

				wallsDestroying[0].Explode();
				wallsDestroying.RemoveAt(0);

				if (wallsDestroying.Count == 0)
				{
					if (gameOver)
					{
						Rows = highestRows;
						Columns = highestRows;
					}

					LeanTween.value(gameObject,
						ExpandCameraSize, Camera.main.orthographicSize, Mathf.Min(Rows, Columns) / 2, 1.0f)
							.setEase(LeanTweenType.easeOutSine);
					LeanTween.scale(Floor.gameObject, new Vector3(Columns, Rows, 1.0f), 1.0f)
						.setEase(LeanTweenType.easeOutSine);

					destroyingWalls = false;

					if (gameOver)
						return;

					if (!willCompress)
					{
						foreach (Wall wall in walls)
							wall.Spawn();
						SpawnEnemies();
					}
					else
						willCompress = false;
					Timer = level * 2 + 10;
				}
			}
		}
		else if (gameOver)
		{
			if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Backspace))
				; // TODO load menu
			else if (Input.anyKeyDown)
			{
				ObjectCache.Clear();
				Time.timeScale = 1.0f;
				Application.LoadLevel(Application.loadedLevel);
			}
			return;
		}
		else if (EnemyPrefab.CountSpawned() == 0)
		{
			level++;
			Score += Mathf.CeilToInt(Timer * 1000 + level * 1000);
			Expand();
		}
		else if (!willCompress)
		{
			Timer -= Time.deltaTime;
			if (Timer <= 0)
			{
				Timer = 0;
				Compress();
				foreach (Wall wall in walls)
					wall.Spawn();

				if (Rows == 2)
				{
					GameOver();
					return;
				}

				Vector3 ppos = Player.TransformCache.position;
				if (ppos.x <= -Columns / 2 + 2)
					ppos.x += 1.0f;
				else if (ppos.x >= Columns / 2 - 2)
					ppos.x -= 1.0f;
				if (ppos.y <= -Rows / 2 + 2)
					ppos.y += 1.0f;
				else if (ppos.y >= Rows / 2 - 2)
					ppos.y -= 1.0f;
				if (ppos != Player.TransformCache.position)
					LeanTween.move(Player.gameObject, ppos, 1.0f);

				foreach (Enemy enemy in enemies)
				{
					if (enemy != null && enemy.enabled)
					{
						Vector3 epos = enemy.TransformCache.position;
						if (epos.x <= -Columns / 2 + 2)
							epos.x += 1.0f;
						else if (epos.x >= Columns / 2 - 2)
							epos.x -= 1.0f;
						if (epos.y <= -Rows / 2 + 2)
							epos.y += 1.0f;
						else if (epos.y >= Rows / 2 - 2)
							epos.y -= 1.0f;
						if (epos != Player.TransformCache.position)
							LeanTween.move(enemy.gameObject, epos, 1.0f);
					}
				}

				//for (int col = 1; col < Columns - 1; col++)
				//	walls[col].TransformCache.localScale = new Vector3(1.0f, 0, 1.0f);
				//for (int row = 1; row < Rows - 1; row++)
				//	walls[Columns - 1 + row].TransformCache.localScale = new Vector3(0, 1.0f, 1.0f);
				//for (int col = Columns - 2; col >= 0; col--)
				//	walls[Columns - 1 + Rows - 1 + col].TransformCache.localScale = new Vector3(1.0f, 0, 1.0f);
				//for (int row = Rows - 2; row > 0; row--)
				//	walls[Columns - 1 + Rows - 1 + Columns - 1 + row].TransformCache.localScale = new Vector3(0, 1.0f, 1.0f);

				//foreach (Wall wall in walls)
				//{
				//	LeanTween.scale(wall.gameObject, new Vector3(1.0f, 1.0f), 0.5f);
				//}
			}
		}

		if (!gameOver)
			TimerText.text = Mathf.CeilToInt(Mathf.Max(0, Timer)).ToString();
	}

	private void GameOver()
	{
		foreach (Enemy enemy in EnemyPrefab.GetSpawned())
		{
			enemy.Kill();
		}
		Player.Kill();

		destroyingWalls = true;
		destroyWallsDelay = 1.1f;
		wallsDestroying = WallPrefab.GetSpawned();

		gameOver = true;

		TimerText.text = "GAME OVER\nSCORE: " + Score;
	}
}
