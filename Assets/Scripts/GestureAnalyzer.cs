namespace Assets.Scripts
{
    public class GestureAnalyzer
    {
        public enum MeaningfulGesture
        { None, Open, Closed, ThumbUp, Pointing }

        public MeaningfulGesture Classify(HandState.FingerState state)
        {
            switch (state)
            {
                case HandState.FingerState.ThumbOpen | HandState.FingerState.IndexOpen |
                     HandState.FingerState.MiddleOpen | HandState.FingerState.RingOpen |
                     HandState.FingerState.PinkyOpen:
                    return MeaningfulGesture.Open;

                case HandState.FingerState.Closed:
                    return MeaningfulGesture.Closed;

                case HandState.FingerState.ThumbOpen:
                    return MeaningfulGesture.ThumbUp;

                case HandState.FingerState.IndexOpen:
                    return MeaningfulGesture.Pointing;

                default:
                    return MeaningfulGesture.None;
            }
        }
    }
}
