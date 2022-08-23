using UnityEngine;

public class MovableCamera : MonoBehaviour
{
    private const float MAX_CAM_ANGLE = 80f;

    [SerializeField]
    private float camSpeed = 5f;

    [SerializeField]
    private float mouseSensitivity = 5f;

    private float xRot = 0f;

    private bool camInverted = false;

    public bool canMove = true;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (canMove)
        {
            Movement();
            Rotation();
        }
    }

    private void Movement()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 translation = new(horizontal * camSpeed * Time.deltaTime, 0f, vertical * camSpeed * Time.deltaTime);
        transform.Translate(translation, Space.Self);
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (!camInverted)
        {
            xRot -= mouseY;
        }
        else
        {
            xRot += mouseY;
        }

        xRot = Mathf.Clamp(xRot, -MAX_CAM_ANGLE, MAX_CAM_ANGLE);

        transform.localRotation = Quaternion.Euler(xRot, transform.localRotation.eulerAngles.y, 0f);
        transform.Rotate(Vector3.up * mouseX, Space.Self);
    }
}
