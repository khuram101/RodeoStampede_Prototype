using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CowboyController : MonoBehaviour
{
    public AnimalController InitialAnimal;
    public AnimalController GrabbedAnimal;
    public LayerMask AnimalLayer;

    [Header("UI")]
    public Canvas PlayGameUI;
    public Canvas GameOverUI;
    public TextMeshProUGUI _distance;


    [Header("Grab Sensor Behaviour")]
    public Transform Sensor;
    public float GrabSensor_ZOffset;
    public float GrabSensorRadius;

    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float horizontalSpeed = 10f;
    public float maxX = 3f;

    [Header("Jump Settings")]
    public float jumpHeight = 2.5f; // How high the animal jumps
    public float jumpDuration = 0.5f; // How fast the jump happens
    private bool isJumping = false;
    public bool InAir;
    private float jumpStartTime;
    private Vector3 startJumpPos;
    public LayerMask groundLayer; // Detect ground
    private bool isGrounded;


    private Vector3 _startPos;
    private bool _isDragging = false;
    private float moveX, deltaX;
    float InertiaSpeed;

    [SerializeField] List<IRidable> animalsInRange;
    public List<IRidable> AnimalsInRange
    {
        get { return animalsInRange; }
    }
    [SerializeField] int ClosestRidable;
    Animator _Anim;
    int inAir_Hash;
    int Death_Hash;
    bool isGameStarted;
    bool isGameOver;
    Rigidbody RB;
    [SerializeField] Transform DummyCircle;
    private SoundController soundController;

    // subtle Movement  Trigger
    public delegate void SubtleMovement(bool status);
    public static event SubtleMovement subtleMovement;

    //stomp shake
    public delegate void StompShake();
    public static event StompShake stompShake;

    [SerializeField] private float maxDistance = 600;
    void Start()
    {
        ClosestRidable = -1;
        animalsInRange = new List<IRidable>();
        RB = GetComponent<Rigidbody>();
        inAir_Hash = Animator.StringToHash("inAir");
        Death_Hash = Animator.StringToHash("Death");
        isGameOver = false;
        Time.timeScale = 0;
        soundController = GetComponent<SoundController>();
        if (Sensor.gameObject.activeInHierarchy)
        {
            Sensor.gameObject.SetActive(false);
        }
    }


    void StartGame()
    {
        InAir = false;
        animalsInRange.Add(InitialAnimal);
        ClosestRidable = 0;
        _Anim = GetComponent<Animator>();
        isGameStarted = true;
        PlayGameUI.gameObject.SetActive(false);
        StartGrab(Input.mousePosition);

        Time.timeScale = 1;
    }


    // Update is called once per frame
    void Update()
    {

        if (isGameOver)
            return;
        ManageInput();

        if (!isGameStarted)
            return;

        if (!InAir)
        {
            if (Sensor.gameObject.activeInHierarchy)
            {
                animalsInRange.Clear();
                Sensor.gameObject.SetActive(false);
            }

            KeepSittingOnAnimal();
            HandleRotation();
            if (GrabbedAnimal == null)
            {
                HandleJump();
            }
            soundController.PlayFootstep();
            Debug.Log("InAir false::::::::::");
            subtleMovement?.Invoke(true);
        }
        else
        {
            Debug.Log("::::::::InAir true::::::::::");
            transform.Translate(transform.forward * (InertiaSpeed * 1.2f) * Time.deltaTime);

            PlaceGrabbableSensor();
            CheckForTheClosestOne();
            HandleJump();
            subtleMovement?.Invoke(false);
        }


        if (transform.position.z > maxDistance)
        {
            GameOver();
        }
    }

    void ManageInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrab(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            DragToMoveHorizontally(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StartJump();
        }
    }


    private void StartGrab(Vector3 position)
    {
        if (!isGameStarted)
        {
            StartGame();
        }
        else
        {
            if (animalsInRange.Count == 0 && ClosestRidable == -1)
                return;

            RB.isKinematic = true;
            RB.useGravity = false;

            GrabAnimal();

            _isDragging = true;
            _startPos = position;
            moveX = deltaX = 0;
            //   InAir = false;//checking issue
        }
    }

    private void DragToMoveHorizontally(Vector3 position)
    {
        if (!_isDragging || GrabbedAnimal == null) return;

        deltaX = (position.x - _startPos.x) / Screen.width;
        moveX = deltaX * horizontalSpeed;

        Vector3 newPosition = GrabbedAnimal.transform.position + new Vector3(moveX, 0, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);

        GrabbedAnimal.transform.position = newPosition;
        _startPos = position;
    }

    public void StartJump()
    {
        if (!GrabbedAnimal)
            return;

        moveX = deltaX = 0;
        _isDragging = false;

        animalsInRange = new List<IRidable>();
        ClosestRidable = -1;
        transform.parent = null;

        Jump();
        InAir = true;
    }

    void Jump()
    {
        isJumping = true;
        jumpStartTime = Time.time;
        startJumpPos = transform.position;

        InertiaSpeed = GrabbedAnimal.GetRidingSpeed();
        GrabbedAnimal.OnRelease();

        _Anim.SetBool(inAir_Hash, true);
        GrabbedAnimal = null;
        RB.isKinematic = false;
    }


    void HandleJump()
    {
        if (!isJumping)
            return;

        float t = (Time.time - jumpStartTime) / jumpDuration;
        if (t >= 1f)
        {
            RB.useGravity = true;
            isJumping = false;

            return;
        }

        float jumpProgress = Mathf.Sin(t * Mathf.PI);
        transform.position = new Vector3(transform.position.x, startJumpPos.y + jumpProgress * jumpHeight, transform.position.z);
    }

    void HandleRotation()
    {
        if (GrabbedAnimal == null)
            return;
        GrabbedAnimal.GetComponent<IRotatable>().Rotate(deltaX);
    }

    public void KeepSittingOnAnimal()
    {
        if (GrabbedAnimal == null)
            return;

        if (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 20);
        }
    }

    void GrabAnimal()
    {
        if (animalsInRange.Count > 0 && ClosestRidable >= 0 && ClosestRidable < animalsInRange.Count)
        {
            if (animalsInRange[ClosestRidable] != null)
            {
                GrabbedAnimal = animalsInRange[ClosestRidable].GetAnimalTransform().GetComponent<AnimalController>();
                GrabbedAnimal.GrabbedByRider(this);
                InAir = false;
                subtleMovement?.Invoke(true);
                stompShake?.Invoke();
                Debug.Log("InAir false");
                transform.parent = GrabbedAnimal.GetSeatTransform();
                _Anim.SetBool(inAir_Hash, false);
            }
        }

    }

    private void CheckForTheClosestOne()
    {
        if (animalsInRange.Count == 0)
            return;


        float FinalDis = 100;
        int indexOfClosest = 0;
        for (int i = 0; i < animalsInRange.Count; i++)
        {
            float _dis = Vector3.Distance(animalsInRange[i].GetAnimalTransform().position, Sensor.position);
            if (_dis < FinalDis)
            {
                FinalDis = _dis;
                indexOfClosest = i;
            }
        }

        if (ClosestRidable == -1)
        {
            ClosestRidable = indexOfClosest;
            animalsInRange[ClosestRidable].Highlight();
        }
        else
        {
            if (ClosestRidable != indexOfClosest)
            {
                for (int i = 0; i < animalsInRange.Count; i++)
                {
                    if (i != indexOfClosest)
                        animalsInRange[i].UnHighlight();
                }

                ClosestRidable = indexOfClosest;
                animalsInRange[ClosestRidable].Highlight();
            }
        }

    }

    void GameOver()
    {
        if (Sensor.gameObject.activeInHierarchy)
            Sensor.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(true);
        _distance.text = transform.position.z.ToString("F0");
        RB.isKinematic = true;
    }
    void Fail()
    {

        GameOver();
        isGameOver = true;



        _Anim.SetTrigger(Death_Hash);



        stompShake?.Invoke();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    #region in Air behaviour

    void PlaceGrabbableSensor()
    {

        if (!Sensor.gameObject.activeInHierarchy)
            Sensor.gameObject.SetActive(true);

        Vector3 origin = transform.position + transform.forward * GrabSensor_ZOffset;
        origin.y = 0;

        DummyCircle.transform.position = origin;
        Sensor.transform.position = origin;

        DummyCircle.transform.localScale = Vector3.one * GrabSensorRadius * 2;
        Sensor.transform.localScale = Vector3.one * GrabSensorRadius * 2;

    }

    public void AddRidable(IRidable Ridable)
    {
        Debug.Log("Added");

        animalsInRange.Add(Ridable);
        Ridable.IndexInList = animalsInRange.Count - 1;
    }

    public void RemoveRidable(IRidable Ridable)
    {
        Debug.Log("Removed");

        int index = Ridable.IndexInList;
        if (index == ClosestRidable)
        {
            animalsInRange[ClosestRidable].UnHighlight();
            animalsInRange[ClosestRidable].IndexInList = -1;
        }

        for (int i = index + 1; i < animalsInRange.Count; i++)
        {
            animalsInRange[i].IndexInList--;
        }

        animalsInRange.RemoveAt(index);
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (InAir)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                Fail();
            }
        }
    }


}
