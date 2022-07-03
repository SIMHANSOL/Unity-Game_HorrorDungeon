using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(AudioSource))]

public class Player : MonoBehaviour 
{
	private float mx = 0f;
	private float my = 0f;

	public float move_speed = 2f;
	public float run_speed = 2f;
	public float crouch_speed = 1f;
	public float jump_speed = 6f;
	private float gravity = 0f;
	public float gravity_speed = 0.2f;

	public float mouse_x_speed = 2f;
	public float mouse_y_speed = 2f;
	public float mouse_y_max_angle = 50f;

	private float camera_offset_y = 0f;
	private float camera_offset_y_origin = 0f;
	private float camera_offset_y_crouch = 0f;

	private float foot_camera_y_move = 0f;
	private float foot_camera_y_range = 0.02f;
	public float foot_camera_y_range_origin = 0.02f;
	public float foot_camera_y_range_run = 0.025f;
	public float foot_camera_y_speed = 0.2f;

	public float ray_range = 1.4f;

	public float die_time = 3f;

	public enum set_state
	{
		stand = 0,
		walk,
		run,
		crouch,
		jump,
		die
	}
	public set_state state = set_state.stand;

	private Transform CT;
	private CharacterController CC;
	private AudioSource AS;
	public AudioClip [] AC;

	private void Start ()
	{
		CT = transform.Find("Player_Camera");
		CC = GetComponent <CharacterController> ();
		AS = GetComponent <AudioSource> ();

		camera_offset_y = CT.localPosition.y;
		camera_offset_y_origin = camera_offset_y;
		camera_offset_y_crouch = camera_offset_y - 1f;
	}

	private void Update ()
	{
		Cursor.lockState = CursorLockMode.Locked;

		if (state != set_state.die) 
		{
			Camera_View (); 
			Active ();
		}
		else
		{
			if (die_time <= 0)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
			else die_time -= 1 * Time.deltaTime;
		}
	}

	private void FixedUpdate ()
	{
		if (state != set_state.die) 
		{
			Camera_Move ();
		}
		else
		{
			CT.localPosition = new Vector3(CT.localPosition.x, Mathf.Lerp(CT.localPosition.y, -1.5f, 0.2f), CT.localPosition.z);
			CT.localRotation = Quaternion.Euler(Mathf.LerpAngle(CT.eulerAngles.x, -60f, 0.1f), 0f, 0f);
		}
	}

	private void Camera_Move () // Move Character.
	{
		// Move.
		float run_check = 0f;
		float forward = Input.GetAxis("Vertical");
		float forward_raw = Input.GetAxisRaw("Vertical");
		float side = Input.GetAxis("Horizontal");
		float side_raw = Input.GetAxisRaw("Horizontal");

		if (forward_raw == 1) {run_check = Input.GetAxisRaw("Run");}

		if ((forward_raw != 0 || side_raw != 0) && run_check == 0)
		{
			state = set_state.walk; // State.
		}
		else if (run_check == 1)
		{
			state = set_state.run; // State.
		}
		else 
		{
			state = set_state.stand; // State.
		}

		// Crouch, Jump.
		float crouch_check = Input.GetAxisRaw("Crouch");
		float jump_check = Input.GetAxisRaw("Jump");

		float up = -gravity;
		if (Grounded () == true)
		{
			// Crouch.
			if (crouch_check != 0)
			{
				run_check = 0f;
				jump_check = 0f;
				state = set_state.crouch; // State.

				CC.center = new Vector3 (0f, -0.5f, 0f);
				CC.height = 3;

				camera_offset_y = camera_offset_y_crouch;
			}
			else
			{
				CC.center = new Vector3 (0f, 0f, 0f);
				CC.height = 4;

				camera_offset_y = camera_offset_y_origin;
			}

			// Jump.
			gravity = 0f;
			gravity -= jump_speed * jump_check;
		}
		else
		{
			state = set_state.jump; // State.
			gravity += gravity_speed;
		}

		if (Aired () == true) if (gravity < 0) gravity = 0f;

		// Result.
		float spd = (move_speed + (run_speed * run_check) - (crouch_speed * crouch_check));

		Vector3 vector = new Vector3(side * spd, up, forward * spd);
		vector = (transform.rotation * vector) * Time.deltaTime;

		if (state == set_state.walk) foot_camera_y_range = foot_camera_y_range_origin;
		else if (state == set_state.run) foot_camera_y_range = foot_camera_y_range_run;
		else if (state == set_state.crouch) foot_camera_y_range = foot_camera_y_range_origin;

		if ((forward_raw != 0 || side_raw != 0) && state != set_state.jump)
		{
			foot_camera_y_move += (move_speed / 10) + (0.1f * run_check);
			CT.localPosition = new Vector3(CT.localPosition.x, Mathf.Lerp(CT.localPosition.y, camera_offset_y, foot_camera_y_speed) + Mathf.Sin(foot_camera_y_move) * foot_camera_y_range, CT.localPosition.z);
		}
		else
		{
			foot_camera_y_move = 0f;
			CT.localPosition = new Vector3(CT.localPosition.x, Mathf.Lerp(CT.localPosition.y, camera_offset_y, foot_camera_y_speed), CT.localPosition.z);
		}

		if (foot_camera_y_move >= (Mathf.PI * 2)) foot_camera_y_move = 0f;

		if (foot_camera_y_move >= (Mathf.PI * 1.5) && state != set_state.jump)
		{
			AS.clip = AC[0];
			if (AS.clip != null) {AS.volume = 0.35f; AS.Play();}
		}

		CC.Move (vector);
	}

	private void Camera_View () // Camera Angle.
	{
		mx = Input.GetAxis("Mouse X") * mouse_x_speed;
		my -= Input.GetAxis("Mouse Y") * mouse_y_speed;
		my = Mathf.Clamp(my, -mouse_y_max_angle, mouse_y_max_angle);

		transform.Rotate(0f, mx, 0f);
		CT.localRotation = Quaternion.Euler(my, 0f, 0f);
	}

	private bool Grounded ()
	{
		RaycastHit hit_info;
		bool hit = Physics.Raycast(transform.position + CC.center, Vector3.down, out hit_info, (CC.height / 2) + CC.stepOffset);
		if (hit == true) return true; return false;
	}

	private bool Aired ()
	{
		RaycastHit hit_info;
		bool hit = Physics.Raycast(transform.position, Vector3.up, out hit_info, (CC.height / 2) + CC.stepOffset);

		if (hit == true) return true; return false;
	}

	private GameObject Ray_Serch ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, ray_range)) return hit.transform.gameObject;
		return null;
	}

	private void Active ()
	{
		bool active = Input.GetButtonDown ("Active");
		if (active == true)
		{
			GameObject ins = Ray_Serch ();
			if (ins != null)
			{
				if (ins.CompareTag("Door_Mesh"))
				{
					ins = ins.transform.parent.gameObject;
					if (ins != null)
					{
						Door_Open scr = ins.GetComponent <Door_Open> ();
						if (scr._lock == false && scr.get_switch() == false)
						{
							float _x = transform.position.x - ins.transform.position.x;
							if (scr.get_rotate() == 0) if (_x < 0) scr.Open(-1); else scr.Open(1);
							else scr.Open(0);
						}
					}
				}
				else if (ins.CompareTag("Lever"))
				{
					Lever scr = ins.GetComponent<Lever> ();
					scr.Set_Switch(!scr.state);
				}
			}
		}
	}
}