using UnityEngine;

namespace Pooler.Scripts
{
    public class EnvironmentSection : MonoBehaviour , IEnvironmentSection
    {
        public MeshRenderer ground;
        [SerializeField] private float environmentLength = 50;
        public float GetLength()
        {
            return environmentLength;
        }
    }


    public interface IEnvironmentSection
    {
        float GetLength();
    }

}