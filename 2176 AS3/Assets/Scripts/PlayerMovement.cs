using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravityValue = -9.81f;

    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;     
    public float groundDistance = 0.4f;    
    public LayerMask groundMask;
    private bool wasGroundedLastFrame;

    [Header("References")]
    public Animator playerAnim;           
    public Transform cameraTransform;      

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        if (playerAnim == null)
        {
            playerAnim = GetComponent<Animator>();
            if (playerAnim == null)
                Debug.LogWarning("⚠️ Animator not found on player!");
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Start()
    {
        groundedPlayer = true;
        wasGroundedLastFrame = true;
    }

    void Update()
    {
        HandleGroundCheck();

        // check if in air last frame & on ground this frame
        if (!wasGroundedLastFrame && groundedPlayer)
        {
            AudioManager.instance.PlaySound("Land");
        }

        // update 'wasGroundedLastFrame' variable for next frame
        wasGroundedLastFrame = groundedPlayer;

        HandleMovement();
        HandleAnimation();
    }

    void HandleGroundCheck()
    {
        groundedPlayer = Physics.Raycast(
            groundCheckPoint.position,
            Vector3.down,
            groundDistance,
            groundMask
        );

        Debug.DrawRay(groundCheckPoint.position, Vector3.down * groundDistance,
        groundedPlayer ? Color.green : Color.red);


        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f * Time.deltaTime;
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move;
        if (CameraControl.Instance != null && CameraControl.Instance.IsInFixedCamera)
        {
            Vector3 camForward = Vector3.Scale(CameraControl.Instance.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = CameraControl.Instance.transform.right;
            move = (camForward * vertical + camRight * horizontal).normalized;
        }
        else
        {
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = cameraTransform.right;
            move = (camForward * vertical + camRight * horizontal).normalized;
        }

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            // AUDIO
            AudioManager.instance.PlaySound("Jump");

            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            groundedPlayer = false;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        Vector3 finalMove = (move * playerSpeed);
        finalMove.y = playerVelocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }
    void HandleAnimation()
    {
        if (playerAnim == null) return;

        float mag = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

        bool isCurrentlyWalking = mag > 0.1f && groundedPlayer;

        // tell audio manager about player walking state
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetWalkingState(isCurrentlyWalking);
        }

        playerAnim.SetFloat("Mag", mag, 0.1f, Time.deltaTime); // smooth damp
        playerAnim.SetBool("isGrounded", groundedPlayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = groundedPlayer ? Color.green : Color.red;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundDistance);
        }
    }
}
