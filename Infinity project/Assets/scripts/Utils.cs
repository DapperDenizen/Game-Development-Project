﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Code based off  http://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/
public static class Utils{

	static Texture2D _whiteTexture;
	public static Texture2D WhiteTexture
	{
		get{ 
			if (_whiteTexture == null) {
				_whiteTexture = new Texture2D (1, 1);
				_whiteTexture.SetPixel (0, 0, Color.white);
				_whiteTexture.Apply ();
			}
			return _whiteTexture;
		}
	}
	// rectangle
	public static void DrawScreenRect(Rect rect, Color color){

		GUI.color = color;
		GUI.DrawTexture(rect, WhiteTexture);
		GUI.color = Color.white;
	
	}
	// borders
	public static void DrawScreenRectBorder(Rect rect, float thickness, Color color){
		// this draws 4 rectangles as the border(because you cant set thickness???)
		// Top
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
		// Left
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
		// Right
		Utils.DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color);
		// Bottom
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
	}
	// converting mouse position to something rect() will understand!
	public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
	{
		//move origin from bottom left to top left
		screenPosition1.y = Screen.height - screenPosition1.y;
		screenPosition2.y = Screen.height - screenPosition2.y;
		// calculate the corners
		var topLeft = Vector3.Min(screenPosition1,screenPosition2);
		var bottomRight = Vector3.Max(screenPosition1,screenPosition2);
		//create rect
		return Rect.MinMaxRect(topLeft.x,topLeft.y, bottomRight.x,bottomRight.y);

	}
	public static Bounds GetViewportBounds( Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
	{
		var v1 = Camera.main.ScreenToViewportPoint (screenPosition1);
		var v2 = Camera.main.ScreenToViewportPoint (screenPosition2);

		var min = Vector3.Min (v1, v2);
		var max = Vector3.Max (v1, v2);
		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;

		var bounds = new Bounds ();
		bounds.SetMinMax (min, max);
		return bounds;
	}




}
