using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games2Win
{
	/// <summary>
	/// Script resposible for camera movement and follow of Ball 
	/// </summary>
	public class CameraControllerScript : MonoBehaviour
	{

        [SerializeField] private GameData gameData; //GameData scriptable object
		private GameObject ball; // the ball game object
		private Vector3 startPosition; // start positon for lerp 
		private Vector3 offsettedPosition; // camera's offseted positon from the ball
		private float interpolationValue; // this value will go from 0 to 1
		private float afterHitInterpolationValue; // this value will go from 0 to 1
		private bool isBallHit; // whether to follow the ball after the bat hits the ball

		#region Mono Methods

		void Awake()
		{
			ball = FindObjectOfType<BallControllerScript>().gameObject; //Using FindObjectOfType instead of direct referencing in scene so that this camera gameobject can be used as a prefab and instantiated when necessary or downloaded as Addressables.
			OnReset(null); // reset camera
		}

		/// <summary>
		/// Subscribing methods to events
		/// </summary>
		private void OnEnable()
        {
			EventManager.AddListener(EventID.Reset, OnReset);
			EventManager.AddListener(EventID.BatSwing, OnBallHit);
		}

		/// <summary>
		/// Un-Subscribing methods from events
		/// </summary>
		private void OnDisable()
        {
			EventManager.RemoveListener(EventID.Reset, OnReset);
			EventManager.RemoveListener(EventID.BatSwing, OnBallHit);
		}


        void LateUpdate()
		{
			if (isBallHit)
			{ // if the player has hit the ball
			  // lerp the camera's position from startPosition to an offsetted value from the ball
				afterHitInterpolationValue += gameData.CameraData.afterHitInterpolationInterval * Time.deltaTime;
				offsettedPosition = new Vector3(ball.transform.position.x < -gameData.CameraData.xBoundaryValue && ball.transform.position.x > gameData.CameraData.xBoundaryValue ? (ball.transform.position.x + gameData.CameraData.afterBallHitPosition.x)
				: (ball.transform.position.x - gameData.CameraData.afterBallHitPosition.x), gameData.CameraData.afterBallHitPosition.y + ball.transform.position.y, ball.transform.position.z + gameData.CameraData.afterBallHitPosition.z);
				transform.position = Vector3.Lerp(startPosition, offsettedPosition, afterHitInterpolationValue);
			}
			else if (ball.transform.position.z >= gameData.CameraData.ballZThreshold && interpolationValue < gameData.CameraData.minInterpolationValue)
			{
				// lerp the camera's position to get a zoomed view
				interpolationValue += gameData.CameraData.interpolationInterval * Time.deltaTime;
				gameData.CameraData.zoomedViewPosition = gameData.CameraData.zoomPos;
				transform.position = Vector3.Lerp(startPosition, gameData.CameraData.zoomedViewPosition, interpolationValue);
			}
		}

		#endregion

		#region Private Regions

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		public void OnReset(System.Object obj)
		{
			isBallHit = false; // reset isBallHit to false
			interpolationValue = 0; // reset interpolationValue to 0
			afterHitInterpolationValue = 0; // reset afterHitInterpolationValue to 0
			transform.position = gameData.CameraData.defaultPosition; // reset camera's position to the default beginning position
			startPosition = gameData.CameraData.defaultPosition; // reset the lerps starting position back to the default position
		}

		/// <summary>
		/// Method called on BatSwing event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallHit(System.Object obj)
        {
            isBallHit = true; // set CameraControllerScript's isBallHit to true
		}
		#endregion

	}
}



