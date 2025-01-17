using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Character
{
    // Singleton
    private static PlayerController instance;
    public static PlayerController Instance
    {
        get { return instance; }
    }

    private InputManager inputManager;
    private Vector2 movementInput;
    private CharacterController characterController;
    private Rigidbody rb;
    private Vector3 _velocity;
    [HideInInspector] public Animator ratAnimator;

    [Header("Bools & Test Bools")]
    [SerializeField] private bool isSound;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWalking;
    [HideInInspector] private bool isAiming;
    [HideInInspector] private bool isCrouching;
    [HideInInspector] private bool isCrawling;
    [SerializeField] private bool isJumping;
    [HideInInspector] private bool isClimbing;
    [SerializeField] private float isChangingItem;
    [HideInInspector] public bool isInteracting;

    // Public Variables
    [Header("Stadistics")]
    public float originalSpeed;
    public float originalJump;
    public Vector3 _playerCamera;
    public Transform playerCamera;
    public float groundDistance = 5f;
    public LayerMask groundMask;

    private float gravity = -9.81f;
    public float fallSpeed = 5f;
    public float maxDistance = 2f; // Maximum distance to check for ground

    [Header("Items")]
    public Item actualItem;
    public int inventoryIndex;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
        _playerCamera = Camera.main.transform.forward;
        originalSpeed = speed;
        originalJump = jumpForce;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        // _rb = GetComponent<Rigidbody>();
        if(currentHP <= 0 || currentHP > hp) currentHP = hp;
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        ratAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        InputManager.PlayerJump += Jump;
        InputManager.PlayerAim += Aim;
        InputManager.PlayerCrouch += Crouch;
        InputManager.PlayerCrawl += Crawl;
        InputManager.PlayerClimb += Climb;
        InputManager.PlayerAttack += Attack;
        InputManager.PlayerChangeItem += ChangeItem;
        InputManager.PlayerInteract += Interact;
    }

    private void OnDisable()
    {
        InputManager.PlayerJump -= Jump;
        InputManager.PlayerAim -= Aim;
        InputManager.PlayerCrouch -= Crouch;
        InputManager.PlayerCrawl -= Crawl;
        InputManager.PlayerClimb -= Climb;
        InputManager.PlayerAttack -= Attack;
        InputManager.PlayerChangeItem -= ChangeItem;
        InputManager.PlayerInteract -= Interact;
    }

    private void Update()
    {
        DetectJump();
        DetectWall();
        //StartCoroutine(FallToTouchGround());

        // Move();
        CheckHP();

        //if (Input.GetKeyUp(KeyCode.J)) StartCoroutine(Hurt());
    }

    private void FixedUpdate()
    {
        Rotate();
        DetectMovement();
        FallToTouchGround();
    }

    public void GetHurt()
    {
        StartCoroutine(Hurt());
    }

    IEnumerator Hurt()
    {
        GameManager.Instance.FindObjectsByName("Hurt").SetActive(true);
        currentHP -= 1;
        SoundManager.Instance.PlayEffect(Audios.effectPlayerHurt);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.FindObjectsByName("Hurt").SetActive(false);
    }

    private void CheckHP()
    {
        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        SoundManager.Instance.PlayEffect(Audios.effectPlayerDie);
        FadeManager.Instance.FadeOut();
        yield return new WaitForSecondsRealtime(1.5f);
        PlayerPrefs.SetInt("loading", 1);
        DataManager.Instance.RespawnCost(-69);
        if(SceneManager.GetActiveScene().buildIndex == 1) SceneManager.LoadScene(1);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void FallToTouchGround()
    {
        //float currentVerticalSpeed = 0f;

        //while (!isGrounded && !isJumping)
        //{
        //    Debug.Log(isGrounded);
        //    currentVerticalSpeed += gravity * Time.deltaTime * 0.5f;
        //    characterController.Move(new Vector3(0, currentVerticalSpeed, 0) * Time.deltaTime);
        //    yield return null;
        //}

        if (rb.velocity.y < 0) // Character is falling
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallSpeed) * Time.fixedDeltaTime;
            ratAnimator.SetBool("isJumping", false);
        }
    }

    private void DetectMovement()
    {
        movementInput = inputManager.GetPlayerMovement();
        if (ratAnimator.GetBool("openDoor"))
        {
            ratAnimator.SetBool("openDoor", false);
        }
        if (movementInput.magnitude > 0.5f)
        {
            if (isSound) SoundManager.Instance.PlayPlayer(Audios.effectPlayerSteps);
            isSound = false;
            Move();
        }
        else 
        {
            ratAnimator.SetBool("isWalking", false);
            SoundManager.Instance.StopPlayer();
            isSound = true;
        }
    }

    private void Move()
    {
        //Vector3 direction = transform.forward * movementInput.y + transform.right * movementInput.x;
        //characterController.Move(speed * Time.deltaTime * direction);
        if (!isCrawling && !isJumping) ratAnimator.SetBool("isWalking", true);
        else ratAnimator.SetBool("isWalking", false);

        Vector3 direction = transform.forward * movementInput.y + transform.right * movementInput.x;
        Vector3 targetPosition = rb.position + speed * Time.fixedDeltaTime * direction;

        rb.MovePosition(targetPosition);
    }

    private void Rotate()
    {
        Vector3 cameraDirection = playerCamera.forward;
        cameraDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(cameraDirection);
        transform.rotation = rotation;
    }

    private void DetectWall()
    {
        isClimbing = Physics.Raycast(transform.position, Vector3.forward, 3f);
        if (isClimbing)
        {
            ratAnimator.SetBool("isWalking", false);
            ratAnimator.SetBool("isClimbing", true);
        }
        else
        {
            ratAnimator.SetBool("isClimbing", false);
        }
    }

    private void DetectJump()
    {
        //isGrounded = characterController.isGrounded;
        ////Debug.Log(isGrounded);

        //if (isGrounded)
        //{
        //    isJumping = false;
        //}

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 3f);
        Debug.Log(isGrounded);
        if (isGrounded && !isCrawling)
        {
            isJumping = false;
            //ratAnimator.SetBool("isJumping", false);
            speed = 30;
        }
    }

    public void Jump()
    {
        if (!isJumping && isGrounded && !isCrawling)
        {
            //isJumping = true;
            Debug.Log("jumping");
            speed = 5;
            PerformJump();
        }
    }

    void PerformJump()
    {
        //float currentVerticalSpeed = 0f;

        //while (currentVerticalSpeed < jumpForce)
        //{
        //    currentVerticalSpeed += Time.deltaTime * jumpForce * 10;
        //    characterController.Move(new Vector3(0, currentVerticalSpeed, 0) * Time.deltaTime);
        //    yield return null;
        //}

        //while (!isGrounded)
        //{
        //    Debug.Log("falling");
        //    currentVerticalSpeed += gravity * Time.deltaTime * 2;
        //    characterController.Move(new Vector3(0, currentVerticalSpeed, 0) * Time.deltaTime);
        //    yield return null;
        //}


        //isJumping = false;

        if (!isJumping)
        {
            ratAnimator.SetBool("isWalking", false);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            ratAnimator.SetBool("isJumping", true);
        }

        isJumping = true;
    }

    public void Aim()
    {
        isAiming = !isAiming;
    }

    public void Crouch()
    {
        isCrouching = !isCrouching;
    }

    public void Crawl()
    {
        isCrawling = !isCrawling;
        speed = 50;
        ratAnimator.SetBool("isCrawling", !ratAnimator.GetBool("isCrawling"));
        SoundManager.Instance.PlayPlayer(Audios.effectPlayerSteps);
    }

    public void Climb()
    {
        isClimbing = !isClimbing;
    }

    public void Attack()
    {
        isAttacking = !isAttacking;
    }

    public void ChangeItem()
    {
        int itemCount = InventoryManager.Instance.Items.Count;

        // If itemCount is 0, the actualItem on the hud would be null
        if (itemCount == 0)
        {
            actualItem = null;
            GameManager.Instance.UpdateItem(actualItem);
            return;
        }

        // Obtain the mouse scroll
        isChangingItem = inputManager.GetMouseScroll();

        // If the mouse scroll is greater than 0, add 1 to the inventory index
        if (isChangingItem > 0) inventoryIndex = (inventoryIndex + 1) % itemCount;
        // If the mouse scroll is lesser than 0, rest 1 to the inventory index
        else if (isChangingItem < 0) inventoryIndex = (inventoryIndex - 1 + itemCount) % itemCount;

        if (InventoryManager.Instance.Items.Count == 1) inventoryIndex = 0;
        actualItem = InventoryManager.Instance.Items[inventoryIndex];
        GameManager.Instance.UpdateItem(actualItem);
    }

    public void Interact()
    {
        isInteracting = true;
        StartCoroutine(QuitInteract());
    }

    IEnumerator QuitInteract()
    {
        yield return new WaitForSecondsRealtime(1f);
        isInteracting = false;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.CompareTag("Test"))
        {
            CameraManager.instance.ChangeCamera(Cameras.BossCamera);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<ItemPickup>(out ItemPickup ip))
        {
            CanvasManager.Instance.ShowMSG("Press [E] to pickup");
            if (isInteracting)
            {
                ip.Collected();
                CanvasManager.Instance.HideMSG();
                isInteracting = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ItemPickup>(out ItemPickup ip))
        {
            CanvasManager.Instance.HideMSG();
        }
    }
}