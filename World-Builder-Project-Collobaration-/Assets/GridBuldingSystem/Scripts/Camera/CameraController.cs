using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float moveSpeed = 10f;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 scrollInput;

    #region Input
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void OnScrollWheel(InputValue value)
    {
        scrollInput = value.Get<Vector2>();
    }
    #endregion

    #region Unity Methods
    void Update()
    {
       float deltaTime = Time.unscaledDeltaTime;
      UpdateMovement(deltaTime);
    }
    #endregion

    #region Control Methods

    void UpdateMovement(float deltaTime)
    {
        // Forward movement
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        Vector3 motion =  targetVelocity * deltaTime;
        cameraTarget.position += forward * motion.z + right * motion.x;
    }
    #endregion
}
