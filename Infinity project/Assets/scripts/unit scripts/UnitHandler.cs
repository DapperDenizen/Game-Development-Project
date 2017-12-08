using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT CONTROLS THE AI AND STATS SCRIPT, IT IS ALSO RESPONSIBLE FOR MOVEMENT

public class UnitHandler : MonoBehaviour {
	public bool aiPresent;
	public Enemy aiController;
	public UnitStats stats;
	bool turnInProgress= false;
	public bool UnabletoFight = false; // this can be a generic to stop an entity from fighting ( i will use it so a player who loses health is just unconcious )
	gameworld parentGameworld;
	Grid grid;

	//
	public float speed = 50;
	float yOreintation;
	Vector3[] path;
	int targetIndex;

	//
	void Awake () {
		yOreintation = transform.position.y;
		parentGameworld = this.GetComponentInParent<gameworld> ();
		grid = GameObject.Find ("Pathfinding").GetComponent<Grid> ();

	}

	public void CombatGeneric(){
		
		if (aiPresent) {
			if(parentGameworld.MyTurnYet(gameObject)){
			aiController.CombatController ();
			}
		} else {
			if (UnabletoFight) {
				parentGameworld.MyTurnDone ();
				return;
			}
			parentGameworld.PlayersTurn ();
			return;
		}

	}
	public void DeathState(){
		if (aiPresent) {
			parentGameworld.ImDead (this.gameObject);
		} else {
			//put some indicator of death here
			UnabletoFight = true;
		} 
	}

	public void StartCombatMode(){
		if(aiPresent){
		speed = speed *10; //this is so the enemy who is in a slower state doesnt drag out the movement
		}
		StopCoroutine ("FollowPath");
		//snap to 
		StartPathGoing(this.transform.position);

	}

	public void StartPathGoing(Vector3 target){
		PathRequestManager.RequestPath (transform.position, target, OnPathFound);

	}
	public void CombatPathGoing(Vector3 target){
		turnInProgress = true;
		PathRequestManager.RequestPath (transform.position, target, OnCombatPathFound);
	}

	public void OnCombatPathFound(Vector3[] newPath, bool pathSuccessful){
		if (pathSuccessful) {
			List<Vector3> temp = new List<Vector3> ();
			int tempCap;
			if (stats.quickness > newPath.Length) {
				tempCap = newPath.Length;
			} else {
				tempCap = stats.quickness;
			}
			for (int i = 0; i < tempCap; i++) {
				temp.Add (newPath [i]);
			}
			if (aiPresent) {
				//temp.RemoveAt (temp.Count - 1);
			}
			path = temp.ToArray ();
			//print (path.ToString ());
			//print (newPath.ToString());
			StartCoroutine ("FollowPath");
		} else {
			print ("woopsy");
		}
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful){
		if (pathSuccessful) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}

	}
	IEnumerator FollowPath(){
		targetIndex = 0;
		Vector3 currentWaypoint = path [0];
		while (true) {

			if (transform.position == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					if(turnInProgress){
						parentGameworld.MyTurnDone ();

					}
					yield break;

				} 
				currentWaypoint = path [targetIndex];
			}
			currentWaypoint.y = yOreintation;
			Node temp = grid.NodeFromWorldPoint (transform.position);
			temp.Occupied (false);
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed* Time.deltaTime);
			temp = grid.NodeFromWorldPoint (transform.position);
			temp.Occupied (true);
			yield return null;

		}
	}

	public void EndTurn(){
		parentGameworld.MyTurnDone ();
	}

}
