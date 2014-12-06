using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Arena : MonoBehaviour
{

	public Transform FloorPrefab;
	public Transform WallPrefab;

	public int Rows = 10;
	public int Columns = 10;

	private Transform transformCache;
	private List<Transform> walls;

	public void Expand()
	{
		Rows += 2;
		Columns += 2;

		CreateWalls();
		Camera.main.orthographicSize = Mathf.Min(Rows, Columns) / 2;
	}

	private void CreateWalls()
	{
		foreach (Transform wall in walls)
		{
			Destroy(wall.gameObject);
		}
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
		Transform wall = Instantiate(WallPrefab,
			new Vector3(column - Columns / 2 + 0.5f, row - Rows / 2 + 0.5f), Quaternion.identity) as Transform;
		wall.parent = transformCache;
		walls.Add(wall);
	}

	void Awake()
	{
		transformCache = transform;
	}

	// Use this for initialization
	void Start()
	{
		walls = new List<Transform>();
		CreateWalls();

		Camera.main.orthographicSize = Mathf.Min(Rows, Columns) / 2;
	}

	// Update is called once per frame
	void Update()
	{

	}
}
