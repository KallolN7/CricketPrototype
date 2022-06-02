using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Games2Win
{

	/// <summary>
	/// Scriptable Object class to store all game related datas. Values can be tweaked here without changes in scene. 
	/// </summary>
	[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
	public class GameData : ScriptableObject
	{
		[SerializeField] private GamelayData gamelayData;
		[SerializeField] private BowlingData bowlingData;
		[SerializeField] private BattingData battingData;
		[SerializeField] private CameraData cameraData;
		[SerializeField] private MarkerDragData markerDragData;
		[SerializeField] private SwipeData swipeData;
		[SerializeField] private AudioData audioData;

		public GamelayData GamelayData =>  gamelayData;
		public BowlingData BowlingData => bowlingData;
		public BattingData BattingData => battingData;
		public CameraData CameraData => cameraData;
		public MarkerDragData MarkerDragData => markerDragData;
		public SwipeData SwipeData => swipeData;
		public AudioData AudioData => audioData;
	}
}



[Serializable]
public class GamelayData
{
	public float minBatElevationValue;
	public float maxBatElevationValue;
	public float minInGameBallSpeed;
	public float maxInGameBallSpeed;
	public float minRealWorldBallSpeed;
	public float maxRealWorldBallSpeed;
	public float minInGameBatSpeed;
	public float maxInGameBatSpeed;
	public float minRealWorldBatSpeed;
	public float maxRealWorldBatSpeed;
	public float defaultBallSpeedValue;
	public float defaultBallType;
	public float defaultBatSpeed;
}

[Serializable]
public class BowlingData
{
	public float bounceScalar; //ball bounce factor
	public float spinScalar; //ball spin factor
}

[Serializable]
public class BattingData
{
	public float boundaryPointX; // max x value the bat can cover
	public float batsmanReachLimitMin; // the ball can be hit once it is inside this limit
	public float batsmanReachLimitMax; // the ball cannot be hit once it gets outside this limit
}

[Serializable]
public class CameraData
{
	public Vector3 defaultPosition; // default position of camera
	public Vector3 afterBallHitPosition; // the position camera will shift to once the ball is hit
	public Vector3 zoomedViewPosition; // camera's position for a zoomed view
	public float interpolationInterval; // camera's interpolation interval value for lerp
	public float afterHitInterpolationInterval; // camera's interpolation interval value for lerp
	public float xBoundaryValue; // make camera follow the ball on its x axis after it is hit if the ball's x position gets out of the range [-x, x]
	public Vector3 zoomPos; //move camera to this position on zoom
	public int ballZThreshold; //z position of ball at which the camera starts to zoom 
	public int minInterpolationValue; //minimum interpolation value
}

[Serializable]
public class MarkerDragData
{
	public float boundaryPointX; // max x value the marker can cover
	public float minBoundaryPointZ; // min z value the marker can cover
	public float maxBoundaryPointZ; // max z value the marker can cover
	public float scaleDownDragBy; // value for scaling down the touch drag value to an appropriate the world value
}

[Serializable]
public class SwipeData
{
	public float minDrag; // the minimum length after which a drag i.e. swipe is considered valid
}

[Serializable]
public class AudioData
{
	public AudioClip bounceClip;
	public AudioClip hitClip;
}