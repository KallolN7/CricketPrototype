using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Games2Win
{
    public class StumpsItem : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Vector3 defaultPos;

        #region Mono Methods

        private void Awake()
        {
            defaultPos = this.transform.position;
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


        #region Private Regions

        private void OnReset(System.Object arg)
        {
            rigidbody.velocity = Vector3.zero; // reset the stump's velocity to zero
            rigidbody.angularVelocity = Vector3.zero; // reset the stump's angular velocity to zero
            rigidbody.useGravity = false; // reset stump's to not get affected by gravity
            transform.position = defaultPos; // reset the stump's position
            transform.rotation = Quaternion.identity; // reset the stump's rotation
        }

        #endregion
    }
}


