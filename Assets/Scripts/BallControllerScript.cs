using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Games2Win
{
	public class BallControllerScript : MonoBehaviour
	{
		[SerializeField] private Vector3 defaultPosition; // ball's default beginning position
		[SerializeField] private GameObject ball; // stores the ball game object
		[SerializeField] private float bounceScalar; // the bounce scalar value to scale the bounce angle after the ball hits the ground
		[SerializeField] private float spinScalar; // the ball's spin scalar value
		//[SerializeField] private float realWorldBallSpeed; // the ball's speed to display on the UI which corresponds to the real world units(kmph)

		private float angle; // the bounce angle of the ball after the ball hits the ground for the first time
		private Vector3 startPosition; // ball's startPosition for the lerp function
		private Vector3 targetPosition; // ball's targetPosition for the lerp function
		private Vector3 direction; // the direction vector the ball is going in
		private Rigidbody rb; // rigidbody of the ball
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
			rb = gameObject.GetComponent<Rigidbody>();
			startPosition = transform.position;  // set the startPosition to the balls beginning position
			OnReset(null);
		}

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
			if (other.tag == "HitZone")
			{
				EventManager.TriggerEvent(EventID.BallEnterHitZone);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == "HitZone")
			{
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
						spinBy = spinScalar / ballSpeed; // change spinBy to a positive value based on the spinScalar value and the ball's speed
						break;
					case (int)BallType.OffSpin:
						spinBy = -spinScalar / ballSpeed; // change spinBy to a negative value based on the spinScalar value and the ball's speed
						break;
				}

				if (!firstBounce)
				{ // if firstBounce is false i.e. when the ball hits the ground for the first time then the expression returns true 
					firstBounce = true; // set the firstBounce bool to true
					rb.useGravity = true; // allow the gravity to affect the ball

					// change the y value of the direction to the negative of it's present value multiplied by the bounceScalar and ball's speed
					// of the ball i.e. the bounce will be more if the ball's speed is more compared to a slower one
					direction = new Vector3(spinBy, -direction.y * (bounceScalar * ballSpeed), direction.z);
					direction = Vector3.Normalize(direction); // normalize the direction value

					angle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg; // calculte the bounce angle from the direction vector

					// Add an instant force impulse in the direction vector multiplied by ballSpeed to the ball considering its mass
					rb.AddForce(direction * ballSpeed, ForceMode.Impulse);
					//rb.velocity = direction * ballSpeed; // update the balls velocity
					//Debug.Log("Ball angle after bounce = " + angle);
				}
				AudioManagerScript.instance.PlayBounceAudio(); // play the ball bounce sound
			}

			if (collision.gameObject.CompareTag("Stump"))
			{ // if the ball has hit the stump then the expression returns true
				AudioManagerScript.instance.PlayBatHitAudio(); // play the same sound as the bat hit sound.
				collision.gameObject.GetComponent<Rigidbody>().useGravity = true; // set the stump's rigidbody to be affected by gravity
			}
		}

		#endregion


		#region Private Regions

		private void OnReset(System.Object arg)
		{
			firstBounce = false;
			isBallHit = false;
			isBallThrown = false;
			rb.velocity = Vector3.zero;
			rb.useGravity = false;
			transform.position = defaultPosition;
		}

		private void OnDragMarker(object arg)
		{
			targetPosition = (Vector3)arg;
		}

		private  void BowlBall(System.Object arg)
		{
			if (!isBallThrown)
			{ // if the ball is not thrown, throw the ball
				isBallThrown = true;
				direction = Vector3.Normalize(targetPosition - startPosition); // calculate the direction vector
				rb.AddForce(direction * ballSpeed, ForceMode.Impulse); // Add an instant force impulse in the direction vector multiplied by ballSpeed to the ball considering its mass
			}
		}

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

		private void SwitchSide(System.Object obj)
		{
			transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z); // negate the x value of balls position to change the side
			defaultPosition = transform.position; // reset the default position to new balls position
			startPosition = transform.position; // reset the default position to new balls position
		}

		private void OnUpdateBallSpeed(System.Object obj)
        {
			ballSpeed = (float)obj;	
        }

		private void OnUpdateBallType(System.Object obj)
		{
			ballType = (int)obj;
		}

		#endregion
	}
}

