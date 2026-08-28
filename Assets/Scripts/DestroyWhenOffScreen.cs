using UnityEngine;

namespace Assets.Scripts
{
    // Détruit automatiquement l'objet dès qu'il n'est plus visible par aucune caméra.
    [RequireComponent(typeof(Renderer))]
    public class DestroyWhenOffscreen : MonoBehaviour
    {
        void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}
