using UnityEngine;

public class BillboardY : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 direction = cam.transform.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
