using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Assets.Scripts;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject canvas;

    private float horizontalInput;
    private float verticalInput;
    private bool shiftPressed;
    private int speedHash;
    private int jumpHash;
    float topVelocity;
    Vector3 inputDir;


    private NetworkVariable<bool> jumpNetworkValue = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> inFoodArea = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

    public Raptor basedino2 = new Raptor();
    private NetworkVariable<Raptor> basedino;


    public PlayerUI playerUI;

    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private AudioListener audioListener;

    float m_MaxDistance = 2f;
    bool m_HitDetect;
    RaycastHit m_Hit;

    private void Awake()
    {
        basedino = new NetworkVariable<Raptor>(basedino2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speedHash = Animator.StringToHash("speed");
        jumpHash = Animator.StringToHash("jumping");
        rb.mass = basedino.Value.Weight;
        rb.drag = basedino.Value.GroundDrag;

    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner) return;

        UpdateBasicStatusServerRPC();

        if (!Application.isFocused) return;

        PlayerInput();
        AnimationsServerRPC();
    }

    [ServerRpc]
    private void UpdateBasicStatusServerRPC()
    {
        basedino.Value = basedino.Value.HealthStatusUpdate();

        playerUI.FoodSliderUpdate(basedino.Value.Hunger);
        playerUI.WaterSliderUpdate(basedino.Value.Thirst);
    }

    private void FixedUpdate()
    {
        if (!Application.isFocused || !IsOwner) return;
        MovePlayer();
    }


    private void PlayerInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpServerRPC();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            m_HitDetect = Physics.BoxCast(playerCollider.bounds.center,
                new Vector3(1,1,1), playerObj.transform.forward, out m_Hit, playerObj.transform.rotation, m_MaxDistance);


            if (m_HitDetect)
            {

                if(m_Hit.collider.tag == "Food")
                {
                    Debug.Log("Comi");
                    //    EatServerRPC();
                }

                if (m_Hit.collider.tag == "Water")
                {
                    Debug.Log("Bebi");
                    //    DrinkServerRPC();
                }
            }
        }

    }

    //-----------------------Interactions-------------
    [ServerRpc]
    private void EatServerRPC()
    {
        basedino.Value = basedino.Value.Eat();
    }

    [ServerRpc]
    private void DrinkServerRPC()
    {
        basedino.Value = basedino.Value.Drink();
    }

    //----------------------JUMP-----------------
    [ServerRpc]
    private void JumpServerRPC()
    {
        if (!jumpNetworkValue.Value && basedino.Value.GetCanJump())
        {
            var x = inputDir;
            x.y = basedino.Value.JumpHeight;

            rb.AddForce(x, ForceMode.Impulse);
            jumpNetworkValue.Value = true;
        }
    }


    //-----------------------ANIMATIONS--------------------
    [ServerRpc]
    private void AnimationsServerRPC()
    {
        if (!jumpNetworkValue.Value)
        {
            animator.SetFloat(speedHash, rb.velocity.magnitude);
        }
        else
        {
            animator.SetBool(jumpHash, jumpNetworkValue.Value);
        }
    }


    //-----------------------MOVE PLAYER--------------------
    private void MovePlayer()
    {
        if (!jumpNetworkValue.Value)
        {

            shiftPressed = Input.GetKey(KeyCode.LeftShift);

            //Direction
            orientation.forward = (playerObj.position - new Vector3(cameraTransform.position.x, playerObj.position.y, cameraTransform.position.z)).normalized;
            inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            //Speed to add
            topVelocity = shiftPressed ? basedino.Value.TopSprintSpeed : basedino.Value.TopWalkingSpeed;
        }

        //Server call
        MovePlayerServerRPC(topVelocity, inputDir);

    }

    [ServerRpc]
    public void MovePlayerServerRPC(float topVelocity, Vector3 inputDir)
    {

        if (inputDir != Vector3.zero)
        {
            //playerObj.forward = inputDir.normalized;
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, 0.2f);
        }

        rb.AddForce(inputDir.normalized * basedino.Value.Accelaration);

        if (rb.velocity.magnitude > topVelocity)
        {
            rb.AddForce(-inputDir.normalized * basedino.Value.Accelaration);
        }


    }


    //private void UpdateDrag()
    //{
    //    if (grounded) {
    //        rb.drag = basedino.GroundDrag;
    //    }
    //    else
    //    {
    //        rb.drag = 0;
    //    }
    //}



    //-----------------------COLLISIONS--------------------
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpNetworkValue.Value = false;
            animator.SetBool(jumpHash, jumpNetworkValue.Value);
        }
    }

    //-----------------------ON SPAWN ADJUSTS--------------------
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            //enable the listener
            audioListener.enabled = true;
            //set camera priority
            freeLookCamera.Priority = 1;
        }
        else
        {
            //Set camera priority lower
            freeLookCamera.Priority = 0;
            Destroy(canvas);
        }
    }

}