using Mediapipe;
using Mediapipe.Unity;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    [Serializable]
    public class GestureEvent : UnityEvent<GestureAnalyzer.MeaningfulGesture> { }

    public class HandGestureController : HandLandmarkerResultAnnotationController
    {
        [SerializeField] HandCursor m_HandCursor;

        readonly HandState m_HandState = new HandState();
        readonly GestureAnalyzer m_Analyzer = new GestureAnalyzer();

        public GestureEvent OnGestureDetected = new GestureEvent();

        protected override void SyncNow()
        {
            lock (_currentTargetLock)
            {
                isStale = false;
                annotation.SetHandedness(_currentTarget.handedness);
                annotation.Draw(_currentTarget.handLandmarks, _visualizeZ);

                ProcessLandmarks(LandmarkUtils.ToLandmarkList(_currentTarget.handLandmarks));
            }
        }

        void Awake()
        {
            m_HandState.OnStateChanged += HandleStateChanged;
        }

        // À appeler à chaque frame avec les landmarks fournis par le pipeline MediaPipe
        public void ProcessLandmarks(NormalizedLandmarkList landmarkList)
        {
            m_HandState.Process(landmarkList);
            m_HandCursor.UpdateFromLandmarks(landmarkList);
        }

        void HandleStateChanged(HandState.FingerState previousState, HandState.FingerState currentState)
        {
            GestureAnalyzer.MeaningfulGesture gesture = m_Analyzer.Classify(currentState);
            OnGestureDetected.Invoke(gesture);

            Debug.Log(gesture.ToString());
        }
    }
}