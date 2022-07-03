using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]

public class Skeleton : MonoBehaviour
{
	public float move_speed = 2f;
	public float run_speed = 4f;
	public float sight_range = 20f;
	public float sight_angle = 90f;

	Transform head;
	GameObject target;
	Vector3 target_vector = Vector3.zero;

	public int path = 1;
	GameObject path_target = null;

	public bool serch_time_switch = false;
	public float serch_time = 0f;
	public float serch_time_min = 2f;
	public float serch_time_max = 6f;
	public float serch_time_range = 1f;

	public float detect_time = 0f;
	public float detect_time_max = 0.4f;

	public enum set_state
	{
		stand = 0,
		walk,
		run,
		view
	};

	public enum set_action
	{
		stand = 0,
		serch,
		detect
	};

	public set_state state = set_state.stand;
	public set_action action = set_action.stand;

	private NavMeshAgent NMA;
	private SkinnedMeshRenderer MR;
	private Animator A;
	private AudioSource AS;

	void Start ()
	{
		head = gameObject.transform.Find("Head");
		target = GameObject.FindGameObjectWithTag("Player");

		NMA = GetComponent <NavMeshAgent> ();
		A = GetComponent <Animator> ();

		AS = GetComponent <AudioSource> ();

		GameObject Child = transform.Find("SM_Skeleton_Base").gameObject;
		MR = Child.GetComponent <SkinnedMeshRenderer> ();
	}

	void Update ()
	{
		GameObject obj = Sight_Serch (head, target);
		if (obj != null)
		{
			target_vector = obj.transform.position;
			if (detect_time <= 0f)
			{
				action = set_action.detect;
				detect_time = 0f;
			}
			else detect_time -= 1 * Time.deltaTime;
		}
		else if (action != set_action.detect) detect_time = detect_time_max;

		switch ((int) action)
		{
			case 0: break;

			case 1:
			if (path_target != null)
			{
				if (serch_time_switch == false)
				{
					if (NMA.remainingDistance < serch_time_range)
					{
						serch_time = Random.Range(serch_time_min, serch_time_max);
						serch_time_switch = true;
					}
				}
				else if (serch_time > 0f) {state = set_state.view; serch_time -= 1 * Time.deltaTime;}
				else {path_target = null; serch_time = 0f; serch_time_switch = false;}
			}

			if (path_target == null) Path_Move ();
			break;

			case 2: 
			if (target_vector != Vector3.zero)
			{
				state = set_state.run;

				NMA.SetDestination(target_vector);

				Vector3 M = new Vector3(transform.position.x, 0f, transform.position.z);
				Vector3 N = new Vector3(target_vector.x, 0f, target_vector.z);

				if (serch_time_range > Vector3.Distance(M, N))
				{
					action = set_action.serch;
					target_vector = Vector3.zero;
					detect_time = detect_time_max;
				}
			}
			break;
		}

		if (state == set_state.run)
		{
			NMA.speed = run_speed;
			AS.pitch = 1.4f;
		}
		else
		{
			NMA.speed = move_speed;
			AS.pitch = 1f;
		}

		A.SetInteger("State", (int) state);
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.name == "Player")
		{
			target.GetComponent <Player>().state = Player.set_state.die;
		}
	}

	public void Path_Move ()
	{
		int path_num = 0;
		GameObject [] find_object = GameObject.FindGameObjectsWithTag("Path");
		foreach (GameObject obj in find_object)
		{
			if (obj.name == "Path_" + path) path_num ++;
		}

		int array = 0;
		GameObject [] path_object = new GameObject[path_num];
		foreach (GameObject obj in find_object)
		{
			if (obj.name == "Path_" + path)
			{
				path_object[array] = obj.gameObject;
				array ++;
			}
		}

		int random = Random.Range(0, array);
		if (path_object[random] != null)
		{
			state = set_state.walk;
			NMA.SetDestination(path_object[random].transform.position);
			path_target = path_object[random];
		}
	}

	public GameObject Sight_Serch (Transform view, GameObject obj)
	{
		if (obj != null)
		{
			float distance = Vector3.Distance(obj.transform.position, view.transform.position);
			if (distance <= sight_range)
			{
				Vector3 range = (obj.transform.position - view.transform.position);
				float angle = Vector3.Angle(range, view.transform.forward);

				if (angle < sight_angle * 0.5f)
				{
					Debug.DrawRay(view.transform.position, range.normalized * 40f);

					RaycastHit hit;
					if(Physics.Raycast(view.transform.position, range.normalized, out hit))
                	{
	                    if(hit.collider.gameObject == obj) return obj;
	                }
				}
			}
		}
		return null;
	}

	public void Set_View (int num)
	{
		switch (num)
		{
			case -1: MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly; break;
			case 0: MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; break;
			case 1: MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On; break;
		}
	}
}