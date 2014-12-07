using UnityEngine;
using System.Collections;

public class TimeScale : MonoBehaviour
{

	private static TimeScale instance;
	public static TimeScale Instance
	{
		get {
			return instance ??
				(instance = (Instantiate(new GameObject("TimeScale")) as GameObject).AddComponent<TimeScale>());
		}
	}

	public static void Set(float scale, float duration)
	{
		Instance.Scale(scale, duration);
	}



	private float duration;
	private float previousTime;

	public void Scale(float value, float duration)
	{
		Time.timeScale = value;
		this.duration = duration;
		previousTime = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	void Update()
	{
		if (duration < 0)
			return;
		if (Time.realtimeSinceStartup - previousTime >= duration)
		{
			Time.timeScale = 1.0f;
			duration = -1;
		}
	}
}
