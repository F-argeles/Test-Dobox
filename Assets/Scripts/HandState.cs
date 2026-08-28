using Mediapipe;
using System;

namespace Assets.Scripts
{
    public class HandState
    {
        [Flags]
        public enum FingerState
        {
            Closed = 0,
            ThumbOpen = 1,
            IndexOpen = 2,
            MiddleOpen = 4,
            RingOpen = 8,
            PinkyOpen = 16,
        }
        public delegate void HandStateEvent(FingerState previousState, FingerState currentState);
        public event HandStateEvent OnStateChanged = (p, c) => { };
        FingerState m_FingerState;

        // Index des landmarks MediaPipe : {MCP (base), PIP, DIP, TIP}
        static readonly int[] ThumbIndex = { 1, 2, 3, 4 };
        static readonly int[] IndexIndex = { 5, 6, 7, 8 };
        static readonly int[] MiddleIndex = { 9, 10, 11, 12 };
        static readonly int[] RingIndex = { 13, 14, 15, 16 };
        static readonly int[] PinkyIndex = { 17, 18, 19, 20 };
        const int WristIndex = 0;
        const double distFingerTolerance = 1.1f;
        const double distThumbTolerance = 0.6f;

        public void Process(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null)
                return;

            /* Analyse Fingers */
            FingerState fingerState = FingerState.Closed;

            if (IsThumbOpen(landmarkList)) fingerState |= FingerState.ThumbOpen;
            if (IsFingerOpen(landmarkList, IndexIndex)) fingerState |= FingerState.IndexOpen;
            if (IsFingerOpen(landmarkList, MiddleIndex)) fingerState |= FingerState.MiddleOpen;
            if (IsFingerOpen(landmarkList, RingIndex)) fingerState |= FingerState.RingOpen;
            if (IsFingerOpen(landmarkList, PinkyIndex)) fingerState |= FingerState.PinkyOpen;

            if (m_FingerState != fingerState)
            {
                OnStateChanged(m_FingerState, fingerState);
                m_FingerState = fingerState;
            }
        }

        // Un doigt est "ouvert" si sa pointe (TIP) est plus loin du poignet
        // que son articulation intermédiaire (PIP). Fonctionne pour Index/Middle/Ring/Pinky.
        static bool IsFingerOpen(NormalizedLandmarkList landmarkList, int[] fingerIdx)
        {
            if (landmarkList == null)
                return false;
            if (landmarkList.Landmark == null || landmarkList.Landmark.Count == 0)
                return false;

            var wrist = landmarkList.Landmark[WristIndex];
            var pip = landmarkList.Landmark[fingerIdx[1]];
            var tip = landmarkList.Landmark[fingerIdx[3]];

            float distPip = Distance(wrist, pip);
            float distTip = Distance(wrist, tip);

            return distTip > distPip * distFingerTolerance; // marge de tolérance
        }

        // Le pouce a une géométrie différente (mouvement plutôt latéral que radial),
        // donc on compare sa pointe à la base de l'index plutôt qu'au poignet.
        static bool IsThumbOpen(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null)
                return false;
            if(landmarkList.Landmark == null || landmarkList.Landmark.Count == 0)
                return false;

            var wrist = landmarkList.Landmark[WristIndex];
            var indexMcp = landmarkList.Landmark[IndexIndex[0]];
            var thumbTip = landmarkList.Landmark[ThumbIndex[3]];

            float distTipToIndex = Distance(thumbTip, indexMcp);
            float distWristToIndex = Distance(wrist, indexMcp);

            return distTipToIndex > distWristToIndex * distThumbTolerance;
        }

        static float Distance(NormalizedLandmark a, NormalizedLandmark b)
        {
            if (a == null || b == null)
                return 0;

            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}