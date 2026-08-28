using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace Assets.Scripts
{
    // Fait apparaître des objets 3D (avec Rigidbody, donc soumis à la gravité)
    // à intervalle régulier, au-dessus de la scène.
    public class ObjectSpawner : MonoBehaviour
    {
        private const string objectTag = "Grabbable";
        [SerializeField] float m_SpawnInterval = 5f;
        [SerializeField] float m_AccelerationRatePerCatch = 0.2f;
        [SerializeField] Vector2 m_SpawnAreaWidth = new Vector2(-3f, 3f);
        [SerializeField] float m_SpawnHeight = 5f;

        [SerializeField] HandGrabber Hand;
        [SerializeField] TextHandler TextTime;
        [SerializeField] TextHandler TextSpawnRate;
        [SerializeField] Button ResetButton;

        [SerializeField] List<GameObject> m_Prefabs;

        public UnityEvent OnTimerFinished = new UnityEvent();

        public float TimeLeft = 30;
        public bool IsSpawning = true;

        float m_TimerBeforeSpawn;

        private List<GameObject> SpawnedObjects = new List<GameObject>();

        private void Awake()
        {
            TextTime?.SetText("Time left : " + TimeLeft.ToString("#0.0") + "s");
            TextSpawnRate?.SetText("Spawn Rate : 1 / " + m_SpawnInterval.ToString("#0.0") + "s");
        }

        void Update()
        {
            if (TimeLeft > 0 && IsSpawning)
            {
                TimeLeft = Math.Clamp(TimeLeft - Time.deltaTime, 0, TimeLeft);
                Debug.Log(TimeLeft);

                TextTime?.SetText("Time left : " + TimeLeft.ToString("#0.0") + "s");
            }
            else
            {
                IsSpawning = false;
                OnTimerFinished.Invoke();

                ResetButton.gameObject.SetActive(true);
            }

            m_TimerBeforeSpawn += Time.deltaTime;
            if (m_TimerBeforeSpawn >= m_SpawnInterval)
            {
                m_TimerBeforeSpawn = 0f;
                Spawn();
            }


        }

        GameObject GetRandomizedPrefab()
        {
            GameObject objectPath = m_Prefabs?[UnityEngine.Random.Range(0, m_Prefabs.Count)]!;
            return objectPath;
        }

        void Spawn()
        {
            if (!IsSpawning) return;

            float x = UnityEngine.Random.Range(m_SpawnAreaWidth.x, m_SpawnAreaWidth.y);
            Vector3 pos = new Vector3(x, m_SpawnHeight, 0f);

            var m_Prefab = GetRandomizedPrefab();

            GameObject obj = Instantiate(m_Prefab, pos, Quaternion.identity);
            obj.tag = objectTag; // pour que HandGrabber sache quels objets attraper

            // S'assurer qu'un Rigidbody est présent pour que la gravité s'applique.
            if (obj.GetComponent<Rigidbody>() == null)
                obj.AddComponent<Rigidbody>();

            if (obj.GetComponent<SphereCollider>() == null)
                obj.AddComponent<SphereCollider>();

            // Nettoyage : l'objet se détruit tout seul dès qu'il sort du champ de la caméra.
            if (obj.GetComponent<DestroyWhenOffscreen>() == null)
                obj.AddComponent<DestroyWhenOffscreen>();

            SpawnedObjects.Add(obj);
        }

        public void DestroyObject(GameObject obj)
        {
            SpawnedObjects.Remove(obj);
            Destroy(obj.gameObject);
        }

        public void ReduceInterval()
        {
            m_SpawnInterval = Math.Clamp(m_SpawnInterval - m_AccelerationRatePerCatch, 0.5f, m_SpawnInterval);

            TextSpawnRate?.SetText("Spawn Rate : 1 / " + m_SpawnInterval.ToString("#0.0") + "s");
        }

        public void Reset()
        {
            m_SpawnInterval = 5f;
            TimeLeft = 45;
            Hand.score = 0;

            TextTime?.SetText("Time left : " + TimeLeft.ToString("#0.0") + "s");
            TextSpawnRate?.SetText("Spawn Rate : 1 / " + m_SpawnInterval.ToString("#0.0") + "s");
            Hand.TextScore?.SetText("Score : " + Hand.score.ToString());

            IsSpawning = true;

            ResetButton.gameObject.SetActive(false);
        }
    }
}