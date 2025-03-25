using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalController : MonoBehaviour, IRidable, IRotatable
{

    [Header("Rider Settings")]
    [HideInInspector] public CowboyController Rider;
    public Transform Seat;
    public float timeToThrowCowboy;

    [Header("Self Movement Settings")]
    public float forwardSpeed = 5f;
    public float RidingSpeed = 5f;

    [Header("Aggression Behaviuor")]
    public Transform AggressPanel;
    public TextMeshProUGUI AggressTimer;
    public Image AggressFiller;

    [Header("Grab Sensor Settings")]
    public MeshRenderer[] MeshesToChangeColor;
    public Material HighlighedMaterials;
    bool isHighlighted;
    private Material defaultMaterial;

    [SerializeField] int _indexInList;
    float timer;
    private float moveX, deltaX;
    private PlayerParticles playerParticles;

    [Header("Rotation Speed")]
    public float maxRotation;
    public float RotationSpeed;

    public int IndexInList
    {
        get => _indexInList;
        set => _indexInList = value;
    }

    // // random rotation
    private float rotationTimer;
    private float angryrotationDuration = 1f; // Adjust for how often the animal should change direction
    private float maxRotationAngle = 4f; // Adjust for how much the animal should turn
    private int rotationDirection = 1; // 1 for right, -1 for left

    private bool isStopMovement = false;

    private Transform cameraTransform;
    void Start()
    {
        timer = 0f;
        angryrotationDuration = timeToThrowCowboy * 0.7f;
        defaultMaterial = MeshesToChangeColor[0].sharedMaterial;
        playerParticles = GetComponent<PlayerParticles>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!isStopMovement && Vector3.Distance(cameraTransform.position, transform.position) < 120)
            Movement();
    }

    void Movement()
    {
        if (Rider)
        {
            transform.Translate(Vector3.forward * RidingSpeed * Time.deltaTime);
            AggressionBehaviour();
        }
        else
        {
            // Move forward
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        }
        playerParticles.HandleWalkParticle();
    }

    public void Rotate(float steer)
    {
        float targetRotation = steer * 40 * maxRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                                            Quaternion.Euler(0, targetRotation, 0),
                                                            Time.deltaTime * RotationSpeed);
    }
    void AggressionBehaviour()
    {
        if (timer <= timeToThrowCowboy)
        {
            timer += Time.deltaTime;

            AggressFiller.fillAmount = timer / timeToThrowCowboy;
            AggressTimer.text = (timeToThrowCowboy - timer).ToString("F1");
        }
        else
        {
            AggressTimer.text = "0";
            AggressFiller.fillAmount = 0;

            AggressPanel.gameObject.SetActive(false);
            Rider.StartJump();
            StartCoroutine(nameof(StopMovement));
        }
    }

    IEnumerator StopMovement()
    {
        isStopMovement = true;
        yield return new WaitForSeconds(2);
    }
    public float GetRidingSpeed()
    {
        return RidingSpeed;
    }


    public void GrabbedByRider(CowboyController cowboy)
    {
        Rider = cowboy;

        AggressPanel.gameObject.SetActive(true);

        UnHighlight();
        playerParticles.HandleLandParticle();
    }

    public void Highlight()
    {
        if (!isHighlighted)
        {
            foreach (MeshRenderer t in MeshesToChangeColor)
            {
                t.material = HighlighedMaterials;
            }
            isHighlighted = true;
        }
    }

    public void UnHighlight()
    {
        if (isHighlighted)
        {
            foreach (MeshRenderer t in MeshesToChangeColor)
            {
                t.material = defaultMaterial;
            }
            isHighlighted = false;
        }
    }

    public bool IsHighlighted() => isHighlighted;

    public void OnRide() { }

    public void OnRelease()
    {
        Rider = null;
        UnHighlight();
    }

    public Transform GetAnimalTransform()
    {
        return transform;
    }
    public Transform GetSeatTransform()
    {
        return Seat;
    }

}
public interface IRotatable
{
    void Rotate(float steer);
}