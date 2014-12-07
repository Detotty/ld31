using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour
{

	private CanvasGroup canvasGroup;

	private bool paused;
	private LTDescr tween;

	// Use this for initialization
	void Start()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	// Update is called once per frame
	void Update()
	{
		if (paused && Input.GetKeyDown(KeyCode.Escape))
		{
			Time.timeScale = 1.0f;
			Application.LoadLevel("MenuScene");
		}
		else if (paused && Input.GetMouseButtonDown(0))
		{
			Unpause();
		}
		else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.P))
		{
			if (!paused)
				Pause();
			else
				Unpause();
		}
	}

	void Pause()
	{
		paused = true;
		Time.timeScale = 0;

		if (tween != null)
			tween.cancel();
		tween = LeanTween.value(gameObject, OnFade, canvasGroup.alpha, 1.0f, 1.0f)
			.setEase(LeanTweenType.easeOutSine).setUseEstimatedTime(true);
	}

	void Unpause()
	{
		paused = false;
		Time.timeScale = 1.0f;

		if (tween != null)
			tween.cancel();
		tween = LeanTween.value(gameObject, OnFade, canvasGroup.alpha, 0, 1.0f)
			.setEase(LeanTweenType.easeOutSine).setUseEstimatedTime(true);
	}

	void OnFade(float val)
	{
		canvasGroup.alpha = val;
	}

}
