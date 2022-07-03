using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Event_Set : MonoBehaviour 
{
	public int number = 0;

	void OnTriggerEnter (Collider col)
	{
		switch (number)
		{
			case 0: 
			Skeleton scr = GameObject.Find("Skeleton").GetComponent<Skeleton> ();
			scr.Set_View(1);
			scr.action = Skeleton.set_action.serch;

			gameObject.SetActive(false);
			break;

			default: break;
		}
	}
}