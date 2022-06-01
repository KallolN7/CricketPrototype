using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Games2Win
{
	public class BatControllerScript : MonoBehaviour
	{
		[SerializeField] private float batSpeed; // the bat's speed
		[SerializeField] private float batElevation; // the bat's elevation angle i.e. the bat's x rotation axis 
		[SerializeField] private float boundaryPointX; // max x value the bat can cover

		[SerializeField] private float batsmanReachLimitMin; // the ball can be hit once it is inside this limit
		[SerializeField] private float batsmanReachLimitMax; // the ball cannot be hit once it gets outside this limit
		[SerializeField] private Vector3 ballsPositionAtHit; // the balls position when it gets hit by the bat	
		[SerializeField]
		private float BatElevation
		{
			set
			{
				batElevation = value;
				transform.rotation = Quaternion.Euler(batElevation, transform.rotation.y, transform.rotation.z); // update the bats rotation to match the elevation
			}
		}

		private GameObject ball; // the ball gameObject
		private bool isBatSwinged; // has the bat swinged
		private Vector3 defaultPosition; // bat's default beginning position
		private bool IsBallThrown;
		private bool IsBallHit;
		private bool isBallInHittingRange;
		



		#region Mono Methods

		void Awake()
		{
			ball = FindObjectOfType<BallControllerScript>().gameObject;		
		}

		void Start()
		{
			defaultPosition = transform.position; // set defaultPosition to the bats beginning position
		}

		private void OnEnable()
		{
			EventManager.AddListener(EventID.Reset, OnReset);
			EventManager.AddListener(EventID.UpdateBatSpeed, OnUpdateBatSpeed);
			EventManager.AddListener(EventID.UpdateBatElevation, OnUpdateBatElevation);
			EventManager.AddListener(EventID.BallBowled, OnBallBowled);
			EventManager.AddListener(EventID.BallHit, HitTheBall);
			EventManager.AddListener(EventID.BallEnterHitZone, OnBallEnterHittingZone);
			EventManager.AddListener(EventID.BallExitHitZone, OnBallExitHittingZone);
		}

		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.Reset, OnReset);
			EventManager.RemoveListener(EventID.UpdateBatSpeed, OnUpdateBatSpeed);
			EventManager.RemoveListener(EventID.UpdateBatElevation, OnUpdateBatElevation);
			EventManager.RemoveListener(EventID.BallBowled, OnBallBowled);
			EventManager.RemoveListener(EventID.BallHit, HitTheBall);
			EventManager.RemoveListener(EventID.BallEnterHitZone, OnBallEnterHittingZone);
			EventManager.RemoveListener(EventID.BallExitHitZone, OnBallExitHittingZone);
		}

		void FixedUpdate()
		{
			// if the bat has not swinged once and the ball is thrown and inside the bats hit range then 
			if (!isBatSwinged && IsBallThrown && ball.transform.position.z <= batsmanReachLimitMax)
			{
				transform.transform.position = new Vector3(ball.transform.position.x,
					transform.transform.position.y,
					transform.transform.position.z);
			}

			// Clamp the bats position withing the pitch width
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, -boundaryPointX, boundaryPointX), transform.position.y, transform.position.z);

			// if the bat has swinged once and the ball is hitted by the bat then update its position to the balls position at the time of hit
			// just to make it look as if the bat hit the ball
			if (isBatSwinged && IsBallHit)
			{
				transform.position = Vector3.MoveTowards(transform.position, ballsPositionAtHit, Time.deltaTime * 20);
			}
		}

		#endregion



		#region Private Regions

		private void HitTheBall(System.Object obj)
		{
			float dragAngle = (float)obj;
			isBatSwinged = true;	

			// if the ball is inside the bats hit range then hit the ball
			if (isBallInHittingRange)
			{
				Debug.Log("Hit Ball");
				AudioManagerScript.instance.PlayBatHitAudio(); // play the bat hit sound
				ballsPositionAtHit = ball.transform.position; // set the ballsHitPositon to the balls position at the time of hit
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, dragAngle, transform.rotation.eulerAngles.z); // change rotation of the bat on the y axis to the swipe dragAngle

				// Call the HitBall function of the BallControllerScript and pass it the forward direction of 
				//the bat's transform and the bat's speed

				EventManager.TriggerEvent(EventID.BatSwing, new BatSwingData { hitDirection = transform.forward, batSpeed = batSpeed });
			}
		}

		private void OnUpdateBatSpeed(System.Object obj)
		{
			batSpeed = (float)obj;
		}

		private void OnUpdateBatElevation(System.Object obj)
		{
			batElevation = (float)obj;	
		}

		private void OnBallEnterHittingZone(System.Object arg)
		{
			isBallInHittingRange = true;
		}

		private void OnBallExitHittingZone(System.Object arg)
		{
			isBallInHittingRange = false;
		}

		private void OnBallBowled(System.Object arg)
		{
			IsBallThrown = true;
		}

		private void OnReset(System.Object arg)
		{
			isBallInHittingRange = false;
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0);
			transform.position = defaultPosition;
			isBatSwinged = false;
			IsBallThrown = false;
			IsBallHit = false;
			isBatSwinged = false;
		}
	}

	#endregion

	public class BatSwingData
    {
		public Vector3 hitDirection;
		public float batSpeed;
	}
}


