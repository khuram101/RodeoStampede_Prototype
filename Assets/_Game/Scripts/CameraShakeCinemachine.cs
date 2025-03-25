using UnityEngine;
using Unity.Cinemachine;
using System;

public class CameraShakeCinemachine : MonoBehaviour
{
    private CinemachineCamera cinemachineCam;
    [Header("Perlin Shake")]
    private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField]
    private float shakeIntensity = 2f;
    [SerializeField]
    private float shakeFrequency = 2f;
    private bool shakeStatus;

    [Header("Stomp shake"), SerializeField]
    private float intensity;
    private CinemachineImpulseSource impulseSource;
    private void OnEnable()
    {
        CowboyController.subtleMovement += SubtleMovement;
        CowboyController.stompShake += StompShake;
    }
    private void OnDisable()
    {
        CowboyController.subtleMovement -= SubtleMovement;
        CowboyController.stompShake -= StompShake;
    }
    void Start()
    {
        cinemachineCam = GetComponent<CinemachineCamera>();
        noise = cinemachineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (noise == null)
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin component not found! Add it to the camera.");
        }

    }
    private void Update()
    {
        if (shakeStatus)
            StartShake();
        else
            StopShake();
    }

    void SubtleMovement(bool _status)
    {
        shakeStatus = _status;
    }
    void StartShake()
    {
        if (noise != null)
        {
            noise.AmplitudeGain = shakeIntensity;  // Set shake intensity
            noise.FrequencyGain = shakeFrequency;  // Set shake speed
        }
    }
    void StopShake()
    {
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;  // Stop shake
            noise.FrequencyGain = 0f;
        }
    }

    void StompShake()
    {
        impulseSource.GenerateImpulse();
    }
}
