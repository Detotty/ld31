using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

	public Image Logo;
	public Text ClickToPlayText;
	public AudioClip Sound;

	private bool blink;
	private float blinkDelay;

	// Use this for initialization
	void Start()
	{
		Time.timeScale = 1.0f;
		LeanTween.move(Logo.rectTransform, new Vector3(0, -32.0f), 2.0f)
			.setEase(LeanTweenType.easeInExpo)
			.setOnComplete(() => { blink = true; AudioSource.PlayClipAtPoint(Sound, Vector3.zero, 0.5f); } );
	}

	// Update is called once per frame
	void Update()
	{
		if (!blink)
			return;

		blinkDelay -= Time.deltaTime;
		if (blinkDelay <= 0)
		{
			blinkDelay += 1.0f;
			ClickToPlayText.enabled = !ClickToPlayText.enabled;
		}

		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
			Application.Quit();
		else if (Input.anyKeyDown)
		{
			Application.LoadLevel("PlayScene");
		}
	}
}
