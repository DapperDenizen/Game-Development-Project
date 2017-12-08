using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS IS RESPONSIBLE FOR THE STATS / combat stuff

public class UnitStats : MonoBehaviour {
	//
	Grid grid;
	UnitHandler handler;
	//
	//stats
	public int quickness;
	public float currentHealth;
	public float maxHealth;	//CHANGE THIS TO JUST HEALTH AND HAVE A PRIVITE INT DEDICATED TO THE OTHERONE
	public float attack;
	public float range;// 
	//

	void Awake () {
		grid = GameObject.Find ("Pathfinding").GetComponent<Grid> ();
		handler = gameObject.GetComponent<UnitHandler> ();
		range = Mathf.Sqrt (Mathf.Pow (grid.nodeRadius * 2, 2) * 2);
	}

	public void TakeDamage(float dam){
		currentHealth = currentHealth - dam;
		print ("oof! im at "+ currentHealth+ "/"+maxHealth);
		if (currentHealth <= 0) {
			print ("Im dead :,^(");
			handler.DeathState ();
		}
	}
	public void DoDamage(GameObject target){
		print ("Hyah "+ this.gameObject +" Does "+ attack+ " to "+ target);
		target.GetComponent<UnitStats> ().TakeDamage (attack);
		handler.EndTurn ();
	}
}
