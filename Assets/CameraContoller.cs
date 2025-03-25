using JetBrains.Annotations;
using UnityEngine;

public class CameraContoller : MonoBehaviour
{
    [SerializeField] Transform Target;
    [SerializeField] float HorizontalFollowSpeed;
    [SerializeField] Vector3 FollowOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
            return;

        transform.position = Target.position + FollowOffset;

    }
}
