using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//probably could have just had a bordermin and a bordermax instead of individual ones for width and height, but im making this like it goin to have more than one map
public class CameraControls : MonoBehaviour
{
	// variables
	bool rotating = false;
	public GameObject camera;
	public int speed;
	float cinemaSmooth = 10f;
	public float cinemaRotation =10f;
	public int scrollSpeed;
	public int zoomMax;
	public int zoomMin;
	public int boardWMax;
	// board Width max & min is the size of the playing field
	public int boardWMin;
	public int boardHMax;
	// board height max & min is the size of the playing field
	public int boardHMin;
	public int borderEdge;
	bool playerTouched = false;
	Vector3 movement = Vector3.zero;
	// Use this for initialization
	void Start ()
	{
		//Cursor.lockState = CursorLockMode.Confined;
	}

	// Update is called once per frame
	void Update ()
	{
		movement = Vector3.zero;
		playerTouched = false;
		//check if buttons down
		// KEY PRESSED W
		if (Input.GetKey (KeyCode.W)) {
			// + z
			if (transform.position.z < boardHMax) {
				movement.z = +speed * Time.deltaTime;
			}
		}
		// KEY PRESSED A
		if (Input.GetKey (KeyCode.A)) {
			//  - x
			if (transform.position.x > boardWMin) {
				movement.x = -speed * Time.deltaTime;
			}						
		}
		// KEY PRESSED S
		if (Input.GetKey (KeyCode.S)) {
			// - z
			if (transform.position.z > boardHMin) {
				movement.z = -speed * Time.deltaTime;
			}
		}
		// KEY PRESSED D
		if (Input.GetKey (KeyCode.D)) {
			//  +x
			if (transform.position.x < boardWMax) {
				movement.x = +speed * Time.deltaTime;
			}	

		}
		//
		//check for edge hovering
		//Left boarder
		if (Input.mousePosition.x < borderEdge) {
			if (transform.position.x > boardWMin) {
				movement.x = -speed * Time.deltaTime;
			}
		}

		//right boarder
		if (Input.mousePosition.x > Screen.width - borderEdge) {
			if (transform.position.x < boardWMax) {
				movement.x = +speed * Time.deltaTime;
			}
		}
		//bottom
		if (Input.mousePosition.y < borderEdge) {
			
			if (transform.position.z > boardHMin) {
				movement.z = -speed * Time.deltaTime;
			}

		}
		//top
		if (Input.mousePosition.y > Screen.height - borderEdge) {
			if (transform.position.z < boardHMax) {
				movement.z = +speed * Time.deltaTime;
			}

		}
		//
		if (movement != Vector3.zero) {
			playerTouched = true;
		}

		transform.Translate (movement);
		

		//scroll check
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			if (transform.position.y >= zoomMin) {
				transform.Translate (new Vector3 (0, -scrollSpeed * Time.deltaTime, 0));
			}

		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			if (transform.position.y <= zoomMax) {
				transform.Translate (new Vector3 (0, scrollSpeed * Time.deltaTime, 0));
			}

		}
		//transform.position = player.transform.position + offset;

		//Xcom style camera rotation
		if (Input.GetKeyDown (KeyCode.Q)) {
			//move counter-clockwise
			print("To the left");
			if (rotating == false) {
				rotating = true;
				StartCoroutine ("CinematicRotate",90f);
			}
		}
		if (Input.GetKey (KeyCode.E)) {
			//move clockwise	
			print("To the right");

			if (rotating == false) {
				rotating = true;
				StartCoroutine ("CinematicRotate",-90f);
			}
		}

	}

	//centre the camera on a Vector3
	public void CentreCamera (Vector3 here)
	{
		//transform.position = here;
		StopCoroutine ("CinematicMove ");
		StartCoroutine ("CinematicMove", here);
	}

	IEnumerator CinematicMove (Vector3 here)
	{
		while (true) {
			transform.position = Vector3.Lerp (transform.position, here, cinemaSmooth * Time.deltaTime);
			if (Vector3.Distance (transform.position, here) < 0.5f || playerTouched) {
				yield break;
			}
			yield return null;
		}
	}

	//cinematic rotation
	IEnumerator CinematicRotate (float amount)
	{
		//Vector3 targetDir = new Vector3 (transform.rotation.x, transform.rotation.y + amount, transform.rotation.z);
		//float step = 90 * Time.deltaTime;
		//Vector3 newDir = Vector3.RotateTowards (transform.up, targetDir, step, 0.0f);
		//transform.rotation = Quaternion.LookRotation (newDir);
		Quaternion newDir = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y+amount,transform.rotation.eulerAngles.z));
		//newDir.eulerAngles.y = newDir.eulerAngles.y+90;


		while (true) {
			transform.rotation = Quaternion.Lerp (transform.rotation, newDir, cinemaSmooth * Time.deltaTime);
			if (Mathf.Abs(transform.rotation.eulerAngles.y - newDir.eulerAngles.y)<1f ) {
				transform.Rotate (0,newDir.eulerAngles.y-transform.rotation.eulerAngles.y  ,0);
				yield return new WaitForSecondsRealtime (.2f);
				rotating = false;
				yield break;
			}
			yield return null;
		}




			//transform.Rotate (0, 90*Time.deltaTime, 0);



	}
}