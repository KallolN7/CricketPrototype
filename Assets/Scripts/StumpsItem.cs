using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Games2Win
{
    /// <summary>
    /// Script attached to individual stumps. Stumps take action independently based on events they are subscribed to.
    /// </summary>
    public class StumpsItem : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Vector3 defaultPos;

        #region Mono Methods

        private void Awake()
        {
            defaultPos = this.transform.position;
        }

        /// <summary>
        /// Subscribing methods to events
        /// </summary>
        private void OnEnable()
        {
            EventManager.AddListener(EventID.Reset, OnReset);
            EventManager.AddListener(EventID.HitStumps, OnHitStumps);
        }


        /// <summary>
        /// Un-Subscribing methods from events
        /// </summary>
        private void OnDisable()
        {
            EventManager.RemoveListener(EventID.Reset, OnReset);
            EventManager.AddListener(EventID.HitStumps, OnHitStumps);
        }

        #endregion


        #region Private Regions


        /// <summary>
        /// Resetting stump properties
        /// </summary>
        /// <param name="arg"></param>
        private void OnReset(System.Object arg)
        {
            rigidbody.velocity = Vector3.zero; // reset the stump's velocity to zero
            rigidbody.angularVelocity = Vector3.zero; // reset the stump's angular velocity to zero
            rigidbody.useGravity = false; // reset stump's to not get affected by gravity
            transform.position = defaultPos; // reset the stump's position
            transform.rotation = Quaternion.identity; // reset the stump's rotation
        }

        /// <summary>
        /// Enabling gravity on stmps being hit
        /// </summary>
        /// <param name="arg"></param>
        private void OnHitStumps(System.Object arg)
        {
            if(this.gameObject == (GameObject)arg)
            rigidbody.useGravity = true;
        }

        #endregion
    }
}


