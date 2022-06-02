using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games2Win
{
    // These are callbacks (delegates) that can be used by the messengers defined in EventManager class below
    public delegate void Callback(System.Object arg);

    /// <summary>
    /// A Manager for events that have one parameter of type T.
    /// </summary>
    public static class EventManager
    {

        private static Dictionary<EventID, Delegate> eventTable = new Dictionary<EventID, Delegate>();

        /// <summary>
        /// Adding methods to event of type EventId
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void AddListener(EventID eventType, Callback handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Create an entry for this event type if it doesn't already exist.
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, null);
                }

                // Add the handler to the event.
                eventTable[eventType] = (Callback)eventTable[eventType] + handler;
            }
        }


        /// <summary>
        /// Removing functions from event of type EventId
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public static void RemoveListener(EventID eventType, Callback handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Only take action if this event type exists.
                if (eventTable.ContainsKey(eventType))
                {
                    // Remove the event handler from this event.
                    eventTable[eventType] = (Callback)eventTable[eventType] - handler;

                    // If there's nothing left then remove the event type from the event table.
                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }


        /// <summary>
        /// invoking event of type EventId
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="arg"></param>
        public static void TriggerEvent(EventID eventType, System.Object arg = null)
        {
            Delegate d;
            // Invoke the delegate only if the event type is in the dictionary.
            if (eventTable.TryGetValue(eventType, out d))
            {
                // Take a local copy to prevent a race condition if another thread
                // were to unsubscribe from this event.
                Callback callback = (Callback)d;

                // Invoke the delegate if it's not null.
                if (callback != null)
                {
                    callback(arg);
                }
            }
        }


        /// <summary>
        /// Clearing eventTable dictionary
        /// </summary>
        public static void CleanUpTable()
        {
            eventTable.Clear();
        }
    }


    /// <summary>
    ///  EventId enum
    /// </summary>
    public enum EventID
    {
      Reset,
      DragMarker,
      BallHit,
      BallBowled,
      BallEnterHitZone,
      BatSwing,
      BallExitHitZone,
        UpdateBatSpeed,
      UpdateBatElevation,
      UpdateBallType,
      UpdateBallSpeed,
      SwitchSide
    }

}


