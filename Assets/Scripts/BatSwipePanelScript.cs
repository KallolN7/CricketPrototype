using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Games2Win
{
	public class BatSwipePanelScript : MonoBehaviour, IBeginDragHandler, IDragHandler
	{
    	[SerializeField]private float minDrag; // the minimum length after which a drag i.e. swipe is considered valid
		private Vector2 startTouchPosition; // the touch's start position
		private Vector2 newTouchPosition; // the current touch's position 
		private bool IsBatSwinged;


		#region Mono Methods

		void Awake()
		{
			Input.multiTouchEnabled = false; // switch multitouch off
		}

		private void OnEnable()
		{
			EventManager.AddListener(EventID.Reset, OnReset);
		}

        private void OnDisable()
		{
			EventManager.RemoveListener(EventID.Reset, OnReset);
		}

		#endregion


		#region Public Regions

		public void OnDrag(PointerEventData eventData)
		{
			newTouchPosition = eventData.position; // set newTouchPosition to current drag position

			// if the bat has not been swinged i.e the player has not tried hitting the ball before 
			// and the drag length is greated than the minimum drag length required then call the
			// BatControllerScript's HitTheBall function with dragAngle passed as the parameter
			if (!IsBatSwinged && Vector2.Distance(newTouchPosition, startTouchPosition) >= minDrag)
			{
				IsBatSwinged = true;
				Vector2 dragDirection = newTouchPosition - startTouchPosition; // direction vector of the drag
				float dragAngle = Mathf.Atan2(dragDirection.y, dragDirection.x) * Mathf.Rad2Deg; // angle of the direction vector

				// reset the dragAngle to match that of the world's angle
				dragAngle += 90;
				dragAngle *= -1;

				// trigger BallHit event with dragAngle passed as the parameter
				EventManager.TriggerEvent(EventID.BallHit, dragAngle);
			}
		}

		#endregion

		#region Private Regions

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			startTouchPosition = eventData.position; // set startTouchPosition to the touch's start position
		}


		private void OnReset(object arg)
		{
			IsBatSwinged = false;
		}

		#endregion
	}
}


