using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace Games2Win
{
	/// <summary>
	/// Script resposible for UI interactions and triggering events based on button clicks
	/// </summary>
	public class CanvasManagerScript : MonoBehaviour
	{
		[SerializeField] private GameObject batSwipePanel;
		[SerializeField] private TextMeshProUGUI ballSpeedText;
		[SerializeField] private TextMeshProUGUI batSpeedText;
		[SerializeField] private TextMeshProUGUI batElevationText;
		[SerializeField] private TextMeshProUGUI ballTypeButtonText;
		[SerializeField] private TextMeshProUGUI batTimerText;
		[SerializeField] private Button BowlButton;
		[SerializeField] private Button ResetButton;
		[SerializeField] private Slider BallSpeedSlider;
		[SerializeField] private Slider BatSpeedSlider;
		[SerializeField] private Button BatHitElevationButton;
		[SerializeField] private Button SwitchSideButton;
		[SerializeField] private Button BallTypeButton;
		[SerializeField] GameData gameData;

		private float ballSpeedSliderValue;
		private float ingameBallSpeed;
		private float realWorldBallSpeed;
		private float batSpeedSliderValue;
		private float ingameBatSpeed;
		private float realWorldBatSpeed;
		private float batElevationValue;
		private int ballType;
		private bool isBallHit;
		private bool canSwitchSides;

		#region Mono Methods

		/// <summary>
		/// Subscribing methods to events and buttons
		/// </summary>
		private void OnEnable()
		{
			EventManager.AddListener(EventID.BallBowled, OnBallBowled);
			EventManager.AddListener(EventID.BatSwing, OnBallHit);
			EventManager.AddListener(EventID.BallEnterHitZone, OnBallEnterHittingZone);
			EventManager.AddListener(EventID.BallExitHitZone, OnBallExitHittingZone);

			BowlButton.onClick.AddListener(OnClick_BowlButton);
			ResetButton.onClick.AddListener(OnClick_ResetButton); 
			BallSpeedSlider.onValueChanged.AddListener(ChangeBallSpeed);
			BatSpeedSlider.onValueChanged.AddListener(ChangeBatSpeed);
			BatHitElevationButton.onClick.AddListener(OnClick_ChangeBatElevationButton);
			SwitchSideButton.onClick.AddListener(OnClick_SwitchSideButton);
			 BallTypeButton.onClick.AddListener(OnClick_ChangeBallTypeButton);
		}

		/// <summary>
		/// Un-Subscribing methods from events and buttons
		/// </summary>
		private void OnDisable()
		{
			EventManager.RemoveListener(EventID.BallBowled, OnBallBowled);
			EventManager.RemoveListener(EventID.BatSwing, OnBallHit);
			EventManager.RemoveListener(EventID.BallEnterHitZone, OnBallEnterHittingZone);
			EventManager.RemoveListener(EventID.BallExitHitZone, OnBallExitHittingZone);

			BowlButton.onClick.RemoveAllListeners();
			ResetButton.onClick.RemoveAllListeners();
			BallSpeedSlider.onValueChanged.RemoveAllListeners();
			BatSpeedSlider.onValueChanged.RemoveAllListeners();
			BatHitElevationButton.onClick.RemoveAllListeners();
			SwitchSideButton.onClick.RemoveAllListeners();
			BallTypeButton.onClick.RemoveAllListeners();
		}

		void Start()
		{
			OnClick_ResetButton();
		}

		#endregion

		#region Private Regions

		/// <summary>
		/// Method to reset the slider values
		/// </summary>
        private void ResetSliders()
        {
			BallSpeedSlider.value = 0;
			BatSpeedSlider.value = 0;
			ChangeBatSpeed(0);
			ChangeBallSpeed(0);
		}

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallBowled(object arg)
		{
			batSwipePanel.SetActive(true); // Enable the bat swipe panel 
		}

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallEnterHittingZone(System.Object arg)
		{
			batTimerText.text = "Hit Now";
		}

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="obj"></param>
		private void OnBallExitHittingZone(System.Object arg)
		{
			if(!isBallHit)
			    batTimerText.text = "Missed!";
		}

		/// <summary>
		/// Method called on Reset event.
		/// </summary>
		/// <param name="arg"></param>
		private void OnBallHit(object arg)
		{
			isBallHit = true;
			batTimerText.text = "Nice Shot!!";
		}

		/// <summary>
		/// Method called on click of ball speed slider.
		/// </summary>
		private void ChangeBallSpeed(float value)
		{
			ballSpeedSliderValue = value;
			ingameBallSpeed = ScaleSpeedToIngame(ballSpeedSliderValue, gameData.GamelayData.minInGameBallSpeed, gameData.GamelayData.maxInGameBallSpeed, 0, 1);
			realWorldBallSpeed = ScaleSpeedToIngame(ballSpeedSliderValue, gameData.GamelayData.minRealWorldBallSpeed, gameData.GamelayData.maxRealWorldBallSpeed, 0, 1);
			ballSpeedText.text = realWorldBallSpeed.ToString("#.##") + "kmph";

			// trigger event UpdateBallSpeed
			EventManager.TriggerEvent(EventID.UpdateBallSpeed, ingameBallSpeed);
		}

		/// <summary>
		/// Method called on click of Bat speed slider.
		/// </summary>
		private void ChangeBatSpeed(float value)
		{
			batSpeedSliderValue = value;
			ingameBatSpeed = ScaleSpeedToIngame(batSpeedSliderValue, gameData.GamelayData.minInGameBatSpeed, gameData.GamelayData.maxInGameBatSpeed, 0, 1);
			realWorldBatSpeed = ScaleSpeedToIngame(batSpeedSliderValue, gameData.GamelayData.minRealWorldBatSpeed, gameData.GamelayData.maxRealWorldBatSpeed, 0, 1);
			batSpeedText.text = realWorldBatSpeed.ToString("#.##") + "kmph";

			// trigger event UpdateBatSpeed
			EventManager.TriggerEvent(EventID.UpdateBatSpeed, ingameBatSpeed);
		}

		/// <summary>
		/// Method called on click of ChangeBatElevationButton.
		/// </summary>
		private void OnClick_ChangeBatElevationButton()
		{
			if (batElevationValue == (int)ShotElevationType.Lofted)
            {
				batElevationValue = (int)ShotElevationType.Grounded;
				batElevationText.text = "<size=70%>Shot Elevation" + "\n" + "<size=100%>Grounded";
			}
			else
            {
				batElevationValue = (int)ShotElevationType.Lofted;
				batElevationText.text = "<size=70%>Shot Elevation" + "\n" + "<size=100%>Lofted";
			}

			batElevationValue = ScaleSpeedToIngame(batElevationValue, gameData.GamelayData.minBatElevationValue, gameData.GamelayData.maxBatElevationValue, 0, 1);

			// triggering UpdateBatElevation event
			EventManager.TriggerEvent(EventID.UpdateBatElevation, batElevationValue);
		}


		/// <summary>
		///Scale the speed value from one range to another 
		/// Example: from a value between range 0 to 1 to a value corresponding to that value, between range 20 to 40.
		/// </summary>
		/// <param name="arg"></param>
		private float ScaleSpeedToIngame(float speed, float scaleMinTo, float scaleMaxTo, float scaleMinFrom, float scaleMaxFrom)
		{
			return (scaleMaxTo - scaleMinTo) * (speed - scaleMinFrom) / (scaleMaxFrom - scaleMinFrom) + scaleMinTo;
		}

		/// <summary>
		///Resetting all values. Called on click of reset button
		/// </summary>
		private void OnClick_ResetButton()
		{
			// triggering Reset event
			EventManager.TriggerEvent(EventID.Reset);

			canSwitchSides = true;
			ResetSliders();
			batTimerText.text = "";
			isBallHit = false;
			batElevationValue = (int)ShotElevationType.Lofted;
			OnClick_ChangeBatElevationButton();
			ballType = (int)BallType.OffSpin;
			OnClick_ChangeBallTypeButton();
			batSwipePanel.SetActive(false);

		}

		/// <summary>
		/// Called on click of Bowl button.
		/// </summary>
		private void OnClick_BowlButton()
		{
			canSwitchSides = false;
			// triggering BallBowled event
			EventManager.TriggerEvent(EventID.BallBowled);
		}

		/// <summary>
		/// Called on click of SwitchSideButton button.
		/// </summary>
		private void OnClick_SwitchSideButton()
		{
			if(canSwitchSides)
				// triggering SwitchSide event
				EventManager.TriggerEvent(EventID.SwitchSide);
		}

		/// <summary>
		/// Called on click of ChangeBallTypeButton button.
		/// </summary>
		private void OnClick_ChangeBallTypeButton()
		{
			ballType++;
			if (ballType > (int)BallType.OffSpin)
			{
				ballType = 0;
			}

			switch (ballType)
			{
				case (int)BallType.Straight:
					ballTypeButtonText.text = "<size=70%>Delivery Type" + "\n" + "<size=100%> Straight";
					break;
				case (int)BallType.LegSpin:
					ballTypeButtonText.text = "<size=70%>Delivery Type" + "\n" + "<size=100%>Leg Spin";
					break;
				case (int)BallType.OffSpin:
					ballTypeButtonText.text = "<size=70%>Delivery Type" + "\n" + "<size=100%>Off Spin";
					break;
			}
			// triggering UpdateBallType event
			EventManager.TriggerEvent(EventID.UpdateBallType, ballType);
		}

		#endregion
	}

	public enum BallType
    {
		Straight = 0,
		LegSpin = 1,
		OffSpin = 2,	
    }

	public enum ShotElevationType
    {
		Grounded,
		Lofted
    }
}


