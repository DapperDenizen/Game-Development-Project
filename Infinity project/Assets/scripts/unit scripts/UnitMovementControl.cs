using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementControl : MonoBehaviour {

	//stats
	public int quicknessStat; // initiative stat that shows who goes first
	public int moveStat;	// how many tiles the user can go
	//
	public float speed = 50;
	float yOreintation;
	Vector3[] path;
	int targetIndex;
	void Start(){
		yOreintation = transform.position.y;
		//PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

	}
	public void StartPathGoing(Vector3 target){
		PathRequestManager.RequestPath (transform.position, target, OnPathFound);

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
					yield break;
				
				} 
				currentWaypoint = path [targetIndex];
			}
			currentWaypoint.y = yOreintation;
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed* Time.deltaTime);
			yield return null;

		}

	}

	public int GetQuickness(){
		return quicknessStat;
	}

}
