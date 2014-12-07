using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

	private static GameObject cameraInstance;
	private static CameraShake instance;
	private static CameraShake Instance
	{
		get
		{
			if (cameraInstance == null)
			{
				cameraInstance = Camera.main.gameObject;
			}
			if (instance == null)
			{
				instance = Camera.main.GetComponent<CameraShake>() ??
					cameraInstance.AddComponent<CameraShake>();
			}
			return instance;
		}
	}

	public static void Shake(float intensity, float duration)
	{
		Instance.AddShake(intensity, duration);
	}



	public float PerlinSpeed = 20.0f;

	private Transform transformCache;

	private List<CameraShakeInfo> shakes;
	private float totalDuration;
	private Vector3 center;

	private int perlinX;
	private int perlinY;

	public void AddShake(float intensity, float duration)
	{
		shakes.Add(new CameraShakeInfo(intensity, duration));
		if (shakes.Count == 0)
		{
			totalDuration = 0;
			perlinX = Random.Range(0, 1000000);
			perlinY = Random.Range(0, 1000000);
		}
	}

	void Awake()
	{
		transformCache = transform;
		shakes = new List<CameraShakeInfo>();
	}
	
	void Start()
	{
		center = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		float intensityX = 0;
		float intensityY = 0;
		float dt = Time.deltaTime;

		for (int i = 0; i < shakes.Count; i++)
		{
			CameraShakeInfo shake = shakes[i];
            shake.Duration -= dt;
            
            if(shake.Duration <= 0)
            {
                shakes.RemoveAt(i);
	            i--;
                
                if(shakes.Count == 0)
                {
	                transformCache.position = center;
                }
                
                continue;
            }

			if (shake.Intensity > intensityX)
				intensityX = shake.Intensity;
			if (shake.Intensity > intensityY)
				intensityY = shake.Intensity;
		}
        
        if(shakes.Count > 0)
        {
            totalDuration += dt;

	        float displacement = totalDuration * PerlinSpeed;

			Vector3 offset = new Vector3();
	        offset.x = (Mathf.PerlinNoise(perlinX, perlinY + displacement) - 0.5f) * intensityX * 2;
	        offset.y = (Mathf.PerlinNoise(perlinX + displacement, perlinY) - 0.5f) * intensityX * 2;

	        transformCache.position = center + offset;
        }
	}
}

public class CameraShakeInfo
{
	public float Intensity = 0;
	public float Duration = 0;

	public CameraShakeInfo(float intensity, float duration)
    {
        this.Intensity = intensity;
        this.Duration = duration;
    }
}