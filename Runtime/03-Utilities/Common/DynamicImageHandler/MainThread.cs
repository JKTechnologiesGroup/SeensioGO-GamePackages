using System;
using System.Collections.Generic;
using UnityEngine;

namespace JKTechnologies.CommonPackage.Utilities
{
    public class MainThread : Singleton<MainThread>
    {

        #region Internal Fields

        private volatile bool idle;
        private List<Action> actionsQueue;
        private List<Action> actionsQueueCopy;

        #endregion


        #region Unity Callbacks

        void Update()
        {
            if (idle)
            {
                return;
            }

            actionsQueueCopy.Clear();

            lock (actionsQueue)
            {
                actionsQueueCopy.AddRange(actionsQueue);
                actionsQueue.Clear();
                idle = true;
            }

            for (int i = 0; i < actionsQueueCopy.Count; i++)
            {
                actionsQueueCopy[i].Invoke();
            }
        }

        #endregion


        #region Public API

        public void Init()
        {
            idle = true;
            actionsQueue = new List<Action>();
            actionsQueueCopy = new List<Action>();
        }

        public void Execute(Action action)
        {
            if (action == null)
            {
                return;
            }

            lock (actionsQueue)
            {
                actionsQueue.Add(action);
                idle = false;
            }
        }

        #endregion

    }
}