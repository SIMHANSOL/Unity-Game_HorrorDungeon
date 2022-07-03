using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour 
{
	public bool state = false;

	public float y_position;
	public float y_offset = 0.28f;
	public float y_offset_speed = 6f;

	public GameObject target;
	private AudioSource AS;

	private void Awake ()
	{
		AS = GetComponent <AudioSource> ();
		y_position = transform.position.y;
	}

	private void Update ()
	{
		Vector3 vector;
		if (state == false) vector = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, y_position + y_offset, y_offset_speed * Time.deltaTime), transform.position.z);
		else vector = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, y_position - y_offset, y_offset_speed * Time.deltaTime), transform.position.z);

		transform.position = vector;

		if (target != null)
		{
			if (target.CompareTag("Door"))
			{
				target.GetComponent<Door_Open> ()._lock = !state;
			}
		}
	}

	public void Set_Switch (bool num)
	{
		if (!AS.isPlaying)
		{
			state = num;
			AS.Play();
		}
	}
}
