using UnityEngine;

public class EnemyHeadLook : MonoBehaviour
{
    [SerializeField] private float leftAngle = -60f;   
    [SerializeField] private float rightAngle = 60f;   
    [SerializeField] private float sweepSpeed = 1.2f;
    [SerializeField] private float returnSpeed = 160f;

    private bool isLooking = false;
    private bool isReturning = false;

    private float sweepTimer = 0f;
    private Quaternion initialLocalRot;

    private void Awake()
    {
        initialLocalRot = transform.localRotation;
    }

    private void Update()
    {
        if (isLooking)
        {
            sweepTimer += Time.deltaTime * sweepSpeed;

            float t = Mathf.PingPong(sweepTimer, 1f);
            float angle = Mathf.Lerp(leftAngle, rightAngle, t);

            Quaternion yaw = Quaternion.Euler(0f, angle, 0f);
            transform.localRotation = initialLocalRot * yaw;
            return;
        }

        if (isReturning)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                initialLocalRot,
                returnSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.localRotation, initialLocalRot) < 0.1f)
            {
                transform.localRotation = initialLocalRot;
                isReturning = false;
            }
        }
    }

    public void StartLooking()
    {
        isReturning = false;
        float currentY = transform.localRotation.eulerAngles.y;
        float localY = Mathf.DeltaAngle(initialLocalRot.eulerAngles.y, currentY);

        float t = Mathf.InverseLerp(leftAngle, rightAngle, localY);

        sweepTimer = t;

        isLooking = true;
    }

    public void StopLooking()
    {
        isLooking = false;
        isReturning = true;
    }
}
