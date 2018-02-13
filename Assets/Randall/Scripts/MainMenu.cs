using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Button start;
	public Button exit;

	LevelManager levelManager;

	// Use this for initialization
	void Start () 
	{
		levelManager = 
			GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

		start.onClick.AddListener(StartButton);
		exit.onClick.AddListener(ExitButton);

		Cursor.lockState = CursorLockMode.None;
	}

	void StartButton()
	{
		levelManager.LoadSceneAsync(1);
	}

	void ExitButton()
	{
		 LevelManager.ExitGame();
	}
}
