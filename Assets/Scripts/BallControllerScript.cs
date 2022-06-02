using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Games2Win
{
	/// <summary>
	/// Script resposible for ball movement and bounce
	/// </summary>
	public class BallControllerScript : MonoBehaviour
	{

		[SerializeField] private GameData gameData;  //GameData scriptable object
		[SerializeField] private Rigidbody rb;
		private Vector3 defaultPosition;
		private float angle; // the bounce angle of the ball after the ball hits the ground for the first time
		private Vector3 startPosition; // ball's startPosition for the lerp function
		private Vector3 targetPosition; // ball's targetPosition for the lerp function
		private Vector3 direction; // the direction vector the ball is going in
		
		private float spinBy; // value to spin the ball by

		private bool firstBounce; // whether ball's hit the ground once or not
		private bool isBallThrown; // whether the ball is thrown or not
		private bool isBallHit; // whether the bat hitted the ball
		private bool isTrajectoryEnabled; // whether the trajectory is enabled or disabled
		private int ballType;
		private float ballSpeed; // speed of the ball
		private float realWorldBallSpeed;
		private BatSwingData batSwingData;


		#region Mono Methods

		void Awake()
		{
			defaultPosition = transform.position; // set defaultPosition to the balls beginning position
			startPosition = transform.position;  // set the startPosition to the balls beginning position
			OnReset(null);
		}

		/// <summary>
		/// Subscribing methods to events
		/// </summary>
		private void OnEnable()
		{
			EventManager.AddListener(EventID.Reset, OnReset);
			EventManager.AddListener(EventID.DragMarker, OnDragMarker);
			EventManager.AddListener(EventID.BallBowled, BowlBall);
			EventManager.AddListener(EventID.BatSwing, OnHitTheBall);
			EventManager.AddListener(EventID.UpdateBallSpeed, OnUpdateBallSpeed);
			EventManager.AddListener(EventID.UpdateBallType, OnUpdateBallType);
			EventManager.AddListener(EventID.SwitchSide, SwitchSide);
		}


		/// <summary>
		/// Un-Subscribing methods from events
		/// </summary>
		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.Reset, OnReset);
			EventManager.RemoveListener(EventID.DragMarker, OnDragMarker);
			EventManager.RemoveListener(EventID.BallBowled, BowlBall);
			EventManager.RemoveListener(EventID.BatSwing, OnHitTheBall);
			EventManager.RemoveListener(EventID.UpdateBallSpeed, OnUpdateBallSpeed);
			EventManager.RemoveListener(EventID.UpdateBallType, OnUpdateBallType);
			EventManager.RemoveListener(EventID.SwitchSide, SwitchSide);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == "HitZone") // checking if ball entered hitting zone
			{
				//trigger BallEnterHitZone event on as soon as ball enters hit zone
				EventManager.TriggerEvent(EventID.BallEnterHitZone);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == "HitZone") // checking if ball exited hitting zone
			{
				//trigger BallExitHitZone event on as soon as ball leaves hit zone
				EventManager.TriggerEvent(EventID.BallExitHitZone);
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			if (!isBallHit && collision.gameObject.CompareTag("Ground"))
			{ // if the ball is not hit by the bat and it collides with the ground then the expression returns true
				switch (ballType)
				{ // check the ballType and set the spinBy value depending on the ball's speed
					case (int)BallType.Straight:
						spinBy = direction.x; // don't change spinBy 
						break;
					case (int)BallType.LegSpin:
						spinBy = -gameData.BowlingData.spinScalar / ballSpeed; // change spinBy to a positive value based on the spinScalar value and the ball's speed
						break;
					case (int)BallType.OffSpin:
						spinBy = gameData.BowlingData.spinScalar / ballSpeed; // change spinBy to a negative value based on the spinScalar value and the ball's speed
						break;
				}

				if (!firstBounce)
				{ // if firstBounce is false i.e. when the ball hits the ground for the first time then the expression returns true 
					firstBounce = true; // set the firstBounce bool to true
					rb.useGravity = true; // allow the gravity to affect the ball

					// change the y value of the direction to the negative of it's present value multiplied by the bounceScalar and ball's speed
					// of the ball i.e. the bounce will be more if the ball's speed is more compared to a slower one
					direction = new Vector3(spinBy, -direction.y * (gameData.BowlingData.bounceScalar * ballSpeed), direction.z);
					direction = Vector3.Normalize(direction); // normalize the direction value

					angle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg; // calculte the bounce angle from the direction vector

					// Add an instant force impulse in the direction vector multiplied by ballSpeed to the ball considering its mass
					rb.AddForce(direction * ballSpeed, ForceMode.Impulse);

					//triggerBallBounce event on as soon as ball touches Ground
					EventManager.TriggerEvent(EventID.BallBounce);
				}

			}

			if (collision.gameObject.CompareTag("Stump"))
			{
				//trigger HitStumps event on as soon as ball touches stumps
				EventManager.TriggerEvent(EventID.HitStumps, collision.gameObject);
			}
		}

		#endregion


		#region Private Regions

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnReset(System.Object arg)
		{
			firstBounce = false;
			isBallHit = false;
			isBallThrown = false;
			rb.velocity = Vector3.zero;
			rb.useGravity = false;
			transform.position = defaultPosition;
		}

		/// <summary>
		/// Method called on whenever the ball pitch marker is moved.
		/// </summary>
		/// <param name="obj"></param>
		private void OnDragMarker(object arg)
		{
			targetPosition = (Vector3)arg;
		}

		/// <summary>
		/// Method called on BowlBall event
		/// </summary>
		/// <param name="obj"></param>
		private void BowlBall(System.Object arg)
		{
			if (!isBallThrown)
			{ // if the ball is not thrown, throw the ball
				isBallThrown = true;
				direction = Vector3.Normalize(targetPosition - startPosition); // calculate the direction vector
				rb.AddForce(direction * ballSpeed, ForceMode.Impulse); // Add an instant force impulse in the direction vector multiplied by ballSpeed to the ball considering its mass
			}
		}

		/// <summary>
		/// Method called on BallHit event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnHitTheBall(System.Object obj)
		{
			batSwingData = (BatSwingData)obj;
			//Debug.Log("OnHitTheBall, hitdirection = " + batSwingData.hitDirection + " | batspeed = " + batSwingData.batSpeed);
			isBallHit = true; // set the is ball hit to true
			rb.velocity = Vector3.zero; // set the ball's velocity to zero to stop the ball
			direction = Vector3.Normalize(batSwingData.hitDirection); // normalize the hit direction of the bat
			float hitSpeed = (ballSpeed / 2) + batSwingData.batSpeed; // calculate the balls return speed based on the bats speed and the balls speed
			rb.AddForce(-direction * hitSpeed, ForceMode.Impulse); // Add an instant force impulse in the negative direction vector multiplied by ballSpeed to the ball considering its mass
			if (!firstBounce)
			{ // if the ball has never hit the ground then set the ball's rigidbody to be affected by gravity
				rb.useGravity = true;
			}
		}

		/// <summary>
		/// Method called on SwitchSide event.
		/// </summary>
		/// <param name="obj"></param>
		private void SwitchSide(System.Object obj)
		{
			transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z); // negate the x value of balls position to change the side
			defaultPosition = transform.position; // reset the default position to new balls position
			startPosition = transform.position; // reset the default position to new balls position
		}

		/// <summary>
		/// Method called on Update of  ball speed.
		/// </summary>
		/// <param name="obj"></param>
		private void OnUpdateBallSpeed(System.Object obj)
        {
			ballSpeed = (float)obj;	
        }

		/// <summary>
		/// Method called on  Update of  ball type.
		/// </summary>
		/// <param name="obj"></param>
		private void OnUpdateBallType(System.Object obj)
		{
			ballType = (int)obj;
		}

		#endregion
	}
}

