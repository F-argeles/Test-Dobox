using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    // Attrape ou lâche les objets "Grabbable" en fonction des gestes
    // détectés par HandGestureController (poing = attraper, sinon = lâcher).
    [RequireComponent(typeof(HandCursor))]
    public class HandGrabber : MonoBehaviour
    {
        [SerializeField] ObjectSpawner Spawner;
        [SerializeField] HandGestureController m_GestureController;
        [SerializeField] float m_GrabRadius = 1.5f;
        [SerializeField] public TextHandler TextScore;

        public UnityEvent OnObjectGrabbed = new UnityEvent();
        public UnityEvent OnObjectOverlapped = new UnityEvent();

        Rigidbody m_HeldObject;

        public int score = 0;

        private void Awake()
        {
            TextScore?.SetText("Score : " + score.ToString());
        }

        void OnEnable()
        {
            m_GestureController.OnGestureDetected.AddListener(HandleGesture);
            Spawner.OnTimerFinished.AddListener(OnTimerFinished);
            OnObjectOverlapped.AddListener(OnDrawGizmosSelected);
        }

        void OnDisable()
        {
            m_GestureController.OnGestureDetected.RemoveListener(HandleGesture);
        }

        void OnTimerFinished()
        {
            TextScore?.SetText("Final Score : " + score.ToString());
        }


        void HandleGesture(GestureAnalyzer.MeaningfulGesture gesture)
        {
            if (gesture == GestureAnalyzer.MeaningfulGesture.Closed)
                TryGrab();
        }

        void TryGrab()
        {
            bool success = false;

            if (m_HeldObject != null) return; // déjà un objet en main

            Collider[] nearby = Physics.OverlapSphere(transform.position, m_GrabRadius);

            foreach (var col in nearby)
            {
                if (!col.CompareTag("Grabbable")) continue;
                if (col.attachedRigidbody == null) continue;

                //OnObjectOverlapped.Invoke();

                m_HeldObject = col.attachedRigidbody;
                m_HeldObject.isKinematic = true;              // on suspend la physique
                m_HeldObject.transform.SetParent(transform);  // l'objet suit la main

                success = true;

                break;
            }

            if (success)
            {
                //On détruit l'objet attrapé
                Destroy(m_HeldObject.gameObject);

                Spawner.ReduceInterval();

                if (Spawner.IsSpawning)
                {
                    score++;
                    TextScore?.SetText("Score : " + score.ToString());
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_GrabRadius);
        }
    }
}