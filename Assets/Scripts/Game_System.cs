using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_System : MonoBehaviour 
{
	private AudioSource AS;
	public AudioClip [] AC;

	private void Start ()
	{
		AS = GetComponent <AudioSource> ();
	}

	private void Update ()
	{
		Main_BGM ();
	}

	private void Main_BGM ()
	{
		if (AS.isPlaying == false)
		{
			int array = Random.Range(0, AC.Length - 1);
			AS.clip = AC[array];
			AS.Play ();
		}
	}
}
