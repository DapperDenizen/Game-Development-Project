using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

	public bool walkable;
	public Vector3 worldPosition;
	public int gCost;
	public int hCost;
	public int gridX;
	public int gridY;
	public Node parent;
	int heapIndex;

	public Node(bool walkable, Vector3 worldPosition, int gridX,  int gridY){
		this.walkable = walkable;
	    this.worldPosition = worldPosition;
		this.gridX = gridX;
		this.gridY = gridY;
	
	}
	public int fCost {
		get{ 
			return gCost + hCost;
		}

	}
	public int HeapIndex{

		get{ return heapIndex;}
		set{ heapIndex = value;}

	}
	public int CompareTo(Node nodeToCompare){
		int compare = fCost.CompareTo (nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo (nodeToCompare.hCost);
		
		}
		return -compare;
	}
	//stuff to make sure units dont collide when they move
	public void Occupied(bool state){
		//occupied is reversed, if its occupied its false, if its not its true!!!
		walkable = state;
	}
}
