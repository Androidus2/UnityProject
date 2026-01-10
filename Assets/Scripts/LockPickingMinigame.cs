using System;
using System.Collections;
using UnityEngine;

public class LockPickingMinigame : MonoBehaviour
{
    public static LockPickingMinigame Instance;

    [Header("Referinte Vizuale")]
    [SerializeField] GameObject minigameRootObject;
    [SerializeField] Animator animator;

    // camera
    [Header("Camera Setup")]
    [SerializeField] Camera lockpickCamera;
    private Camera mainCameraRef; // Tinem minte camera player-ului

    [Header("Audio")]
    [SerializeField] private AudioClip unlockClip;
    [SerializeField] private AudioClip breakClip;
    [SerializeField] private AudioClip rattleClip;

    private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] float pickSpeed = 3f;
    [SerializeField] float rotationTolerance = 0.05f;
    [SerializeField] float maxOpenCap = 0.95f;
    [SerializeField] KeyCode cancelKey = KeyCode.Escape;

    // State
    private float pickPosition;
    private float cyllinderPosition;
    private float sweetSpot;
    private bool isGameActive = false;
    private int currentPickUses;

    //Memoram functia pe care trebuie sa o apelam la final
    private Action<bool> currentCallback;

    private float shakeTimeRemaining;
    private float visualPickOffset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (animator == null) animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Auto-Find Camera
        if (lockpickCamera == null)
        {
            Camera[] allCameras = GetComponentsInChildren<Camera>(true);
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.name == "camera_lockpick") { lockpickCamera = cam; break; }
            }
        }
        if (lockpickCamera != null) lockpickCamera.enabled = false;
    }

    private void Start()
    {
        if (minigameRootObject != null) minigameRootObject.SetActive(false);
        isGameActive = false;
    }

    public void StartMinigame(Action<bool> onFinishedCallback)
    {
        // 1. Memoram callback-ul (Cine m-a apelat? Chestul sau Usa?)
        currentCallback = onFinishedCallback;

        // 2. Schimbam Camerele
        mainCameraRef = Camera.main;
        if (mainCameraRef != null) mainCameraRef.enabled = false;
        if (lockpickCamera != null) lockpickCamera.enabled = true;

        // 3. Afisam Minigame-ul
        if (minigameRootObject != null) minigameRootObject.SetActive(true);

        isGameActive = true;
        ResetMechanics();
    }

    void ResetMechanics()
    {
        cyllinderPosition = 0f;
        pickPosition = 0.5f;
        sweetSpot = UnityEngine.Random.Range(0.1f, 0.9f);
        currentPickUses = UnityEngine.Random.Range(3, 8);
        UpdateAnimator();
    }

    private void Update()
    {
        if (!isGameActive) return;

        if (Input.GetKeyDown(cancelKey)) { FinishGame(false); return; }

        float dt = Time.unscaledDeltaTime;
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        if (v <= 0.1f)
        {
            pickPosition += h * dt * pickSpeed;
            pickPosition = Mathf.Clamp01(pickPosition);
        }
        else ProcessRotation(v, dt);

        if (v <= 0) cyllinderPosition = Mathf.MoveTowards(cyllinderPosition, 0f, dt * 2f);

        UpdateShake(dt);
        UpdateAnimator();
    }

    void ProcessRotation(float force, float dt)
    {
        float distance = Mathf.Abs(pickPosition - sweetSpot);
        float allowance = (distance <= rotationTolerance) ? 1f : (1f - distance);
        if (allowance < 1f) allowance = Mathf.Clamp(allowance, 0f, 0.2f);

        cyllinderPosition += force * dt;

        if (cyllinderPosition >= allowance)
        {
            cyllinderPosition = allowance;
            if (allowance < 0.9f) { TriggerShake(); DamagePick(); }
        }

        if (cyllinderPosition >= maxOpenCap && distance <= rotationTolerance)
        {
            FinishGame(true);
        }
    }

    // (Functiile DamagePick, UpdateShake, TriggerShake raman la fel) ...
    void DamagePick()
    {
        if (UnityEngine.Random.Range(0, 50) == 0)
        {
            currentPickUses--;
            if (currentPickUses <= 0) StartCoroutine(BreakPickRoutine());
        }
    }
    IEnumerator BreakPickRoutine()
    {
        bool wasActive = isGameActive; isGameActive = false;
        if (breakClip) audioSource.PlayOneShot(breakClip);
        yield return new WaitForSecondsRealtime(1f);
        ResetMechanics(); isGameActive = wasActive;
    }
    void TriggerShake() { shakeTimeRemaining = 0.2f; if (rattleClip) audioSource.PlayOneShot(rattleClip); }
    void UpdateShake(float dt) { if (shakeTimeRemaining > 0) { shakeTimeRemaining -= dt; visualPickOffset = UnityEngine.Random.Range(-0.02f, 0.02f); } else visualPickOffset = 0f; }
    void UpdateAnimator() { if (animator) { animator.SetFloat("PickPosition", Mathf.Clamp01(pickPosition + visualPickOffset)); animator.SetFloat("LockOpen", cyllinderPosition); } }


    //iesire din minigame / functia reverse
    void FinishGame(bool success)
    {
        isGameActive = false;
        if (success && unlockClip) audioSource.PlayOneShot(unlockClip);

        // revenire camera principala
        if (lockpickCamera != null) lockpickCamera.enabled = false;
        if (mainCameraRef != null) mainCameraRef.enabled = true;

        // ascundem minigame-ul
        if (minigameRootObject != null) minigameRootObject.SetActive(false);

        // anuntam rezultatul inapoi celui care a apelat minigame-ul
        currentCallback?.Invoke(success);
        currentCallback = null;
    }
}