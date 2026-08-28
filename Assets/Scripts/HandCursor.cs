using Mediapipe;
using UnityEngine;

namespace Assets.Scripts
{
    // Déplace cet objet dans la scène 3D pour qu'il suive la position
    // de la main détectée par MediaPipe (sert de "main virtuelle").
    public class HandCursor : MonoBehaviour
    {
        [SerializeField] Camera m_Camera;
        [SerializeField] float m_DistanceFromCamera = 10f;

        // La paume n'a pas de landmark dédié dans le modèle MediaPipe Hands : on l'estime
        // en faisant la moyenne du poignet et des 4 bases de doigts (MCP), qui forment
        // un cercle assez stable autour du centre réel de la paume.
        static readonly int[] PalmLandmarkIndices = { 0, 5, 9, 13, 17 };

        void Awake()
        {
            // Filet de sécurité : si le champ Camera n'a pas été rempli dans l'Inspector,
            // on utilise la caméra principale de la scène plutôt que de planter en silence.
            if (m_Camera == null)
                m_Camera = Camera.main;

            if (m_Camera == null)
                Debug.LogError("[HandCursor] Aucune caméra assignée ni caméra 'MainCamera' trouvée dans la scène.");
        }

        // Moyenne simple des positions (X, Y) du poignet et des 4 bases de doigts.
        static Vector2 GetPalmCenter(NormalizedLandmarkList landmarkList)
        {
            float sumX = 0f, sumY = 0f;

            foreach (int index in PalmLandmarkIndices)
            {
                var lm = landmarkList.Landmark[index];
                sumX += lm.X;
                sumY += lm.Y;
            }

            int count = PalmLandmarkIndices.Length;
            return new Vector2(sumX / count, sumY / count);
        }

        public void UpdateFromLandmarks(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count == 0) return;

            var palm = GetPalmCenter(landmarkList);

            // MediaPipe donne des coordonnées normalisées [0,1] avec l'origine en haut à gauche.
            // Le Viewport Unity attend une origine en bas à gauche : on inverse donc Y.
            Vector3 viewportPoint = new Vector3(palm.x, 1f - palm.y, m_DistanceFromCamera);
            
            transform.position = m_Camera.ViewportToWorldPoint(viewportPoint);
        }
    }
}