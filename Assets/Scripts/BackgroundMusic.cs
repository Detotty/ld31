using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{

	private static BackgroundMusic instance = null;
	public static BackgroundMusic Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}