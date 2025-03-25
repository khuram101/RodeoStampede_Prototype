using UnityEngine;

public class GrabSensor : MonoBehaviour
{
    [SerializeField] CowboyController controller;
    [SerializeField] LayerMask Ridable;

    private void OnTriggerEnter(Collider other)
    {
        IRidable riddable = other.GetComponent<IRidable>();
        if (riddable != null)
        {
            controller.AddRidable(other.transform.GetComponent<IRidable>());
        }
    }


    private void OnTriggerExit(Collider other)
    {
        IRidable riddable = other.GetComponent<IRidable>();
        if (riddable != null)
        {
            controller.RemoveRidable(other.transform.GetComponent<IRidable>());
        }
       
    }
}

