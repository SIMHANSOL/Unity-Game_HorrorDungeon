using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(AudioSource))]

public class Door_Open : MonoBehaviour
{
	public bool left = true;
	public bool _lock = false;

	public float rotate_speed = 3.5f;

	public float open_volume = 1f;
	public float close_volume = 0.8f;
	public AudioClip [] AC; // 0: Close, 1: Open.

	private bool _switch = false;
	private float _switch_range = 6f;

	private float rotate = 0f;
	private float rotate_open = 90f;
	private float rotate_close = -90f;

	private MeshCollider MC;
	private NavMeshObstacle NMO;

	private AudioSource AS;

	private void Start ()
	{
		if (left == true)
		{
			rotate_open = 90f;
			rotate_close = -90f;
		}
		else
		{
			rotate_open = -90f;
			rotate_close = 90f;
		}

		Transform [] tran = GetComponentsInChildren <Transform> ();
		foreach(Transform obj in tran)
		{
			if (obj.gameObject.CompareTag("Door_Mesh"))
			{
				MC = obj.gameObject.GetComponent <MeshCollider> ();
				NMO = obj.gameObject.GetComponent <NavMeshObstacle> ();
				break;
			}
		}

		AS = GetComponent <AudioSource> ();
	}

	private void Update ()
	{
		transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(transform.eulerAngles.y, rotate, rotate_speed * Time.deltaTime), 0f);

		float euler = transform.eulerAngles.y;

		if (_switch == false)
		{
			if (MC != null) MC.enabled = true;
			if (NMO != null) NMO.enabled = true;
		}
		else
		{
			if (MC != null) MC.enabled = false;
			if (NMO != null) NMO.enabled = false;
		}

		if (left == true)
		{
			if (rotate == rotate_open)
			{
				if (Mathf.Abs(euler - rotate) < _switch_range) _switch = false;
			}
			else if (rotate == rotate_close)
			{
				if (Mathf.Abs((euler - 360) - rotate) < _switch_range) _switch = false;
			}
			else if (rotate == 0f)
			{
				if (euler > 270) euler -= 360;
				if (Mathf.Abs(euler - rotate) < _switch_range) _switch = false;
			}
		}
		else // Right.
		{
			if (rotate == rotate_close)
			{
				if (Mathf.Abs(euler - rotate) < _switch_range) _switch = false;
			}
			else if (rotate == rotate_open)
			{
				if (Mathf.Abs((euler - 360) - rotate) < _switch_range) _switch = false;
			}
			else if (rotate == 0f)
			{
				if (euler > 270) euler -= 360;
				if (Mathf.Abs(euler - rotate) < _switch_range) _switch = false;
			}
		}

		if (_lock == true) rotate = 0f;
	}

	public void Open (int num)
	{
		rotate = 0;
		switch(num)
		{
			case -1: rotate = rotate_close; break;
			case 0: rotate = 0f; break;
			case 1: rotate = rotate_open; break;
		}

		if (rotate != 0f) 
		{
			AS.clip = AC [1];
			AS.volume = 1f;
		} 
		else 
		{
			AS.clip = AC [0];
			AS.volume = 0.8f;
		}
		AS.Play ();

		_switch = true;
	}

	public bool get_switch ()
	{
		return _switch;
	}

	public float get_rotate ()
	{
		return rotate;
	}
}