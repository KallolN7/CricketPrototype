using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games2Win
{
	public class CameraControllerScript : MonoBehaviour
	{
        [SerializeField] private Vector3 defaultPosition; // default position of camera
		[SerializeField] private Vector3 afterBallHitPosition; // the position camera will shift to once the ball is hit
		[SerializeField] private Vector3 zoomedViewPosition; // camera's position for a zoomed view
		[SerializeField] private float interpolationInterval; // camera's interpolation interval value for lerp
		[SerializeField] private float afterHitInterpolationInterval; // camera's interpolation interval value for lerp
		[SerializeField] private float xBoundaryValue; // make camera follow the ball on its x axis after it is hit if the ball's x position gets out of the range [-x, x]

		private GameObject ball; // the ball game object
		private Vector3 startPosition; // start positon for lerp 
		private Vector3 offsettedPosition; // camera's offseted positon from the ball
		private float interpolationValue; // this value will go from 0 to 1
		private float afterHitInterpolationValue; // this value will go from 0 to 1
		private bool isBallHit; // whether to follow the ball after the bat hits the ball
		private Vector3 zoomPos = new Vector3(0, 9.15f, 8.21f); //move camera to this position on zoom
		private const int ballZThreshold = 8;  //z position of ball at which the camera starts to zoom 
		private const int minInterpolationValue = 1; //minimum interpolation value

		public bool IsBallHit
		{
			set
			{
				isBallHit = value;
				if (value)
				{
					startPosition = transform.position; // reset startPositon to the current camera's position
				}
			}
		}


		#region Mono Methods

		void Awake()
		{
			ball = FindObjectOfType<BallControllerScript>().gameObject;
			OnReset(null); // reset camera
		}


		private void OnEnable()
        {
			EventManager.AddListener(EventID.Reset, OnReset);
			EventManager.AddListener(EventID.BallHit, OnBallHit);
			EventManager.AddListener(EventID.BallExitHitZone, OnBallExitHittingZone);
		}

        private void OnDisable()
        {
			EventManager.RemoveListener(EventID.Reset, OnReset);
			EventManager.RemoveListener(EventID.BallHit, OnBallHit);
			EventManager.RemoveListener(EventID.BallExitHitZone, OnBallExitHittingZone);
		}


        void LateUpdate()
		{
			if (isBallHit)
			{ // if the player has hit the ball
			  // lerp the camera's position from startPosition to an offsetted value from the ball
				afterHitInterpolationValue += afterHitInterpolationInterval * Time.deltaTime;
				offsettedPosition = new Vector3(ball.transform.position.x < -xBoundaryValue && ball.transform.position.x > xBoundaryValue ? (ball.transform.position.x + afterBallHitPosition.x)
				: (ball.transform.position.x - afterBallHitPosition.x), afterBallHitPosition.y + ball.transform.position.y, ball.transform.position.z + afterBallHitPosition.z);
				transform.position = Vector3.Lerp(startPosition, offsettedPosition, afterHitInterpolationValue);
			}
			else if (ball.transform.position.z >= ballZThreshold && interpolationValue < minInterpolationValue)
			{
				// lerp the camera's position to get a zoomed view
				interpolationValue += interpolationInterval * Time.deltaTime;
				zoomedViewPosition = zoomPos;
				transform.position = Vector3.Lerp(startPosition, zoomedViewPosition, interpolationValue);
			}
		}

		#endregion

		#region Private Regions
		public void OnReset(System.Object obj)
		{
			isBallHit = false; // reset isBallHit to false
			interpolationValue = 0; // reset interpolationValue to 0
			afterHitInterpolationValue = 0; // reset afterHitInterpolationValue to 0
			transform.position = defaultPosition; // reset camera's position to the default beginning position
			startPosition = defaultPosition; // reset the lerps starting position back to the default position
		}

		private void OnBallHit(System.Object obj)
        {
            IsBallHit = true; // set CameraControllerScript's isBallHit to true
		}

		private void OnBallExitHittingZone(object arg)
		{

		}

		#endregion






	}
}



