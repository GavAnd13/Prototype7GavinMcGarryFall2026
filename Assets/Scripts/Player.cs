using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public GameObject cam;
    public GameObject carryingContainer;
    public LayerMask pickupMask;
    public float moveSpeed;
    public float rotateSpeed;
    public float pickupDistance;

    private Vector2 m_Rotation;
    private Vector2 m_Look;
    private Vector2 m_Move;

    bool carrying;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        m_Look = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (carrying)
            {
                Transform carried = carryingContainer.transform.GetChild(0);
                carrying = false;

                carried.transform.SetParent(null);

                carried.GetComponent<Rigidbody>().isKinematic = false;
                carried.GetComponent<Collider>().isTrigger = false;
            }
            else
            {
                if (Physics.Raycast(transform.position, cam.transform.forward, out RaycastHit hit, pickupDistance, pickupMask))
                {
                    carrying = true;

                    //becomes part of player
                    hit.transform.SetParent(carryingContainer.transform);
                    hit.transform.localPosition = Vector3.zero;
                    hit.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    hit.rigidbody.isKinematic = true;
                    hit.collider.isTrigger = true;
                }
            }
        }
    }

    public void Update()
    {
        // Update orientation first, then move. Otherwise move orientation will lag
        // behind by one frame.
        Look(m_Look);
        Move(m_Move);
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01)
            return;
        var scaledMoveSpeed = moveSpeed * Time.deltaTime;
        // For simplicity's sake, we just keep movement in a single plane here. Rotate
        // direction according to world Y rotation of player.
        var move = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        transform.position += move * scaledMoveSpeed;
    }

    private void Look(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
        m_Rotation.y += rotate.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
        cam.transform.localEulerAngles = m_Rotation;
    }
}
