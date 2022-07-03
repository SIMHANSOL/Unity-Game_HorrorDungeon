using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour 
{
	public TextAsset level_string = null;

	public void Click_Start ()
	{
		SceneManager.LoadScene("Chapter_1");
	}

	public void Click_Load ()
	{
		int num = load_string();
		if (num != 0) SceneManager.LoadScene(num);
	}

	public void Click_End ()
	{
		Application.Quit();
	}

	public int load_string ()
	{
		string level_texts = level_string.text;
		return int.Parse(level_texts);
	}
}
