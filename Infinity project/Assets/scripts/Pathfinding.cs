using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;
	public gameworld currentGameWorld;
	Vector3 endGoal; // actual place of impact
	void Awake(){
		grid = GetComponent<Grid> ();
		requestManager = GetComponent<PathRequestManager> ();

	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos){
		StartCoroutine(FindPath(startPos,targetPos));
		endGoal = targetPos;
	}


	IEnumerator FindPath(Vector3 start, Vector3 target){


		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		Node startNode = grid.NodeFromWorldPoint (start); 
		Node targetNode = grid.NodeFromWorldPoint (target); 
		//startNode.walkable && 
		if (targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize); // to be evaluated
			HashSet<Node> closedSet = new HashSet<Node> (); // already evaluated
			openSet.Add (startNode);

			while (openSet.Count > 0) {

				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);
				if (currentNode == targetNode) {
					pathSuccess = true;
					break;

				}
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
			
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
				
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
						} else {
							openSet.UpdateItem (neighbour);
					
						}
				
					}

				}
			

			}
		}
		yield return null;
		if (pathSuccess) {
					waypoints = RetracePath (startNode,targetNode);
				}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
		
	}

			Vector3[] RetracePath(Node start, Node end){
		List<Node> path = new List<Node> ();
		Node currentNode = end;
		while (currentNode != start) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		//simplify here--------VVVVVV
		Vector3[] waypoints;
			//waypoints = SimplifyPath (path);
			waypoints = RegurgitatePath (path);
		
		Array.Reverse (waypoints);
		return waypoints;

		}

	Vector3[] RegurgitatePath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();

		// adds the actual point of contact!
		waypoints.Add (grid.NodeFromWorldPoint(endGoal).worldPosition);
		//
		for(int i = 1; i < path.Count; i++){
		waypoints.Add(path[i].worldPosition);
		}
		return waypoints.ToArray ();
	}


			Vector3[] SimplifyPath(List<Node> path){
		List<Vector3> waypoints = new List<Vector3> ();

		// adds the actual point of contact!
		waypoints.Add (grid.NodeFromWorldPoint(endGoal).worldPosition);
		//
				Vector2 directionOld = Vector2.zero;
				for(int i = 1; i < path.Count; i++){
					Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
					if(directionNew != directionOld){
						waypoints.Add(path[i].worldPosition);

					}
					directionOld = directionNew;
				}
				return waypoints.ToArray();
			}
		

	int GetDistance(Node nodeA, Node nodeB){

		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY) {
		
			return 14 * distY + 10 * (distX - distY);
		}
		return 14 * distX + 10 * (distY- distX);
	}

}
