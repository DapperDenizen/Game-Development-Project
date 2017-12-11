using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//INSIDE THIS CLASS
//ENEMY = PLAYER CONTROLLED UNITS
//note to self you will need to change how enemys are idenified as player friendly npcs wont show up on enemyunits[]


public class Enemy : MonoBehaviour
{
	UnitStats stats;
	private UnitHandler handler;
	private SphereCollider sCollider;
	public int maxWander;
	int minWander;
	//player controlled units
	gameworld parentGameworld;
	Grid grid;
	GameObject target;
	bool alerted;
	//if a player has alerted them
	bool goalInProgress;
	// currently got a wander destination
	Vector3 endGoal;
	Vector3 initPos;
	// if true combat is a go!
	float singleGridSize = 0;
	Vector3[] path;

	void Awake ()
	{
		minWander = 2;
		parentGameworld = this.GetComponentInParent<gameworld> ();
		grid = GameObject.Find ("Pathfinding").GetComponent<Grid> ();
		handler = this.GetComponent<UnitHandler> ();
		stats = this.GetComponent<UnitStats> ();
		//singleGridSize = grid.nodeRadius * 2;
		sCollider = GetComponent<SphereCollider> ();
		target = null;
		alerted = false;
		goalInProgress = false;
		initPos = transform.position;
		//singleGridSize = Mathf.Sqrt (Mathf.Pow (grid.nodeRadius * 2, 2) * 2); 
		//print ("distance = "+singleGridSize);
		// 2a^2 = b^2 if a=c
	}

	
	// Update is called once per frame
	void Update ()
	{
		//get if combat mode from parentgameworld and checking if this instance is close enough to detected player  
		//move randomly
		if (!alerted) {
			if (!goalInProgress) {
				float rX = Random.Range (-minWander, maxWander) + initPos.x;
				float rZ = Random.Range (-minWander, maxWander) + initPos.z;
				endGoal = new Vector3 (rX, initPos.y, rZ);
				Node potential = grid.NodeFromWorldPoint (endGoal);
				//print ("walkable? " + potential.walkable);
				if (potential.walkable) {
					RaycastHit hit;
					Vector3 direction = potential.worldPosition - (transform.position + Vector3.up);
					if (Physics.Raycast (transform.position + Vector3.up, direction, out hit)) {
						//print ("successfully hit something + its at " + hit.point + "which needs to be at " + potential.worldPosition);
						if (hit.point == potential.worldPosition) {
							handler.StartPathGoing (potential.worldPosition);
							endGoal = potential.worldPosition;
							goalInProgress = true;
						}
					}
				}
			}
			if (Vector3.Distance (endGoal, transform.position) == 1) {
				goalInProgress = false;
			}
		}
	}

	//CHECKS IF AN ENEMY(PLAYER) IS INSIDE ALERT CIRCLE AND IF THE PLAYER IS SEEN
	void OnTriggerStay (Collider other)
	{
		Vector3 direction = other.transform.position - transform.position;
		RaycastHit hit;
		if (Physics.Raycast (transform.position + transform.up, direction.normalized, out hit))
		if (hit.collider.gameObject.CompareTag ("Player")) {
			//COMBAT STARTS!
			if (!alerted) {
				alerted = true;
				target = hit.collider.gameObject;
				parentGameworld.switchCombat (true);
				parentGameworld.addcombatant (gameObject);
				handler.StartCombatMode ();
			}
			//
		} 

	}

	public void CombatController ()
	{
		if (target.GetComponent<UnitHandler> ().UnabletoFight) {
			//simple find new target;
			for (int i = 0; i < parentGameworld.battleLineUp.Count; i++) {
				if (parentGameworld.battleLineUp [i] != target && parentGameworld.battleLineUp [i].CompareTag ("Player")) {
					target = parentGameworld.battleLineUp [i];
					break;
				}
			}
			if (target == null) {
				print ("Enemy wins");
			}
		}
		// if not 1 space away from player move to player
		if (Vector3.Distance (transform.position, target.transform.position) > stats.range) {
			CombatMove ();
		} else {
			//if 1 space away from player attack
			stats.DoDamage (target);
		}
	}

	private void CombatMove ()
	{

		List<Node> possibleDests = grid.GetNeighbours (grid.NodeFromWorldPoint (target.transform.position));
		int currIndex = 0;
		float currLongest = float.MaxValue;
		for (int i = 0; i < possibleDests.Count; i++) {
			if (possibleDests [i].walkable) {
				print ("this is walkable");
				if (Vector3.Distance (transform.position, possibleDests [i].worldPosition) < currLongest) {
					print ("index number "+i+" chosen!");
					currIndex = i;
					currLongest = Vector3.Distance (transform.position, possibleDests [i].worldPosition);
				}
			} else {
				print ("this is not walkable!");
			}
		}
		Vector3 dest = possibleDests [currIndex].worldPosition;

		handler.CombatPathGoing (dest);
	
	}

}
