using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour 
{
	public float sight_range = 150f;
	public float sight_angle = 90f;

	GameObject player;

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update ()
	{
		Sight_Serch (player);
	}

	public GameObject Sight_Serch (GameObject obj)
	{
		if (obj != null)
		{
			float distance = Vector3.Distance(transform.position, obj.transform.position);
			if (distance <= sight_range)
			{
				Vector3 range = (obj.transform.position - transform.position);
				float angle = Vector3.Angle(range, transform.forward);

				if (angle < sight_angle * 0.5f)
				{
					RaycastHit hit;
					if(Physics.Raycast(transform.position, range.normalized, out hit))
                	{
	                    if(hit.collider.gameObject == obj)
	                    {
	                    	return obj;
	                    }
	                }
				}
			}
		}
		return null;
	}
}
