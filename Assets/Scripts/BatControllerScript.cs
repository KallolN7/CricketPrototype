using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Games2Win
{
	/// <summary>
	/// Script resposible for bat swing and calculating the bat swing direction
	/// </summary>
	public class BatControllerScript : MonoBehaviour
	{
        [SerializeField] private GameData gameData;  //GameData scriptable object
		private Vector3 ballsPositionAtHit; // the balls position when it gets hit by the bat	
        private float BatElevation
		{
			set
			{
				batElevation = value;
				transform.rotation = Quaternion.Euler(batElevation, transform.rotation.y, transform.rotation.z); // update the bats rotation to match the elevation
			}
		}

		private float batElevation; // the bat's elevation angle i.e. the bat's x rotation axis 
		private float batSpeed; // the bat's speed
		private GameObject ball; // the ball gameObject
		private bool isBatSwinged; // has the bat swinged
		private Vector3 defaultPosition; // bat's default beginning position
		private bool IsBallThrown; //to check if ball is bowled or not
		private bool IsBallHit; //to check if ball is hit or not
		private bool isBallInHittingRange; //to check if ball is in hitting zone or not
		



		#region Mono Methods

		void Awake()
		{
			defaultPosition = transform.position; // set defaultPosition to the bats beginning position
			ball = FindObjectOfType<BallControllerScript>().gameObject; //Using FindObjectOfType instead of direct referencing in scene so that this bat gameobject can be used as a prefab and instantiated when necessary or downloaded as Addressables.		
		}

		/// <summary>
		/// Subscribing methods to events
		/// </summary>
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

		/// <summary>
		/// Un-Subscribing methods from events
		/// </summary>
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
			if (!isBatSwinged && IsBallThrown && ball.transform.position.z <= gameData.BattingData.batsmanReachLimitMax)
			{
				transform.transform.position = new Vector3(ball.transform.position.x,
					transform.transform.position.y,
					transform.transform.position.z);
			}

			// Clamp the bats position withing the pitch width
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, -gameData.BattingData.boundaryPointX, gameData.BattingData.boundaryPointX), transform.position.y, transform.position.z);

			// if the bat has swinged once and the ball is hitted by the bat then update its position to the balls position at the time of hit
			// just to make it look as if the bat hit the ball
			if (isBatSwinged && IsBallHit)
			{
				transform.position = Vector3.MoveTowards(transform.position, ballsPositionAtHit, Time.deltaTime * 20);
			}
		}

		#endregion



		#region Private Regions

		/// <summary>
		/// Method called for BallHit event
		/// </summary>
		/// <param name="obj"></param>
		private void HitTheBall(System.Object obj)
		{
			float dragAngle = (float)obj;
			isBatSwinged = true;
			// if the ball is inside the bats hit range then hit the ball
			if (isBallInHittingRange)
			{
				ballsPositionAtHit = ball.transform.position; // set the ballsHitPositon to the balls position at the time of hit
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, dragAngle, transform.rotation.eulerAngles.z); // change rotation of the bat on the y axis to the swipe dragAngle

				// Trigger the BatSwing event and pass it the forward direction of 
				//the bat's transform and the bat's speed
				EventManager.TriggerEvent(EventID.BatSwing, new BatSwingData { hitDirection = transform.forward, batSpeed = batSpeed });
			}
		}

		/// <summary>
		/// Method called on update of bat speed
		/// </summary>
		/// <param name="obj"></param>
		private void OnUpdateBatSpeed(System.Object obj)
		{
			batSpeed = (float)obj;
		}

		/// <summary>
		/// Method called on update of bat elevation
		/// </summary>
		/// <param name="obj"></param>
		private void OnUpdateBatElevation(System.Object obj)
		{
			BatElevation = (float)obj;	
		}

		/// <summary>
		/// Method called onball entering hitting zone
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallEnterHittingZone(System.Object arg)
		{
			isBallInHittingRange = true;
		}

		/// <summary>
		/// Method called on exiting hitting zone
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallExitHittingZone(System.Object arg)
		{
			isBallInHittingRange = false;
		}

		/// <summary>
		/// Method called on BallBowled event
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallBowled(System.Object arg)
		{
			IsBallThrown = true;
		}

		/// <summary>
		/// Method called on Reset event
		/// Setting all values to default values 
		/// </summary>
		/// <param name="obj"></param>
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


