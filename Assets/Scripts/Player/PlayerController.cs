using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class PlayerController : NetworkBehaviour {

    #region Serialized Variables

    [Header("Enemy")]
    [SerializeField]
    private GameObject enemyPrefab;

    [Header("Components")]
    [SerializeField]
    private Animator animationController;
    [SerializeField]
    private Rigidbody rigidBody;
    [SerializeField]
    private CapsuleCollider capsuleCollider;
    [SerializeField]
    private GameObject topdownCameraPrefab;
    [SerializeField]
    private LayerMask projectileLayermask;
    [SerializeField]
    private Transform bulletSpawn;

    [Header("Movement")]
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float maximumSpeed;
    [SerializeField]
    private float _rotationSpeed;

    [Header("Weapons")]
    [SerializeField]
    private GameObject bulletPrefab;
    //Input weapons here

    #endregion

    #region Private Variables

    private Camera _topDownCamera;
    private GameObject topDownCameraPivot;

    private float _horizontal;
    private float _vertical;
    private Vector3 _forward;

    private Vector3 _motion;
    private bool _bMoving;

    private float _rayLength;
    private Vector3 _mouseTargetPosition;
    private Vector3 _lookRotation;

    private Plane _groundPlane ;
    private Vector3 _mousePos;

    #endregion

    #region Network Variables

    [SyncVar]
    private float _currentHealth;
    [SyncVar]
    private float _curentShield;

    #endregion

    public override void OnStartLocalPlayer()
    {
        _groundPlane = new Plane(Vector3.up, Vector3.zero);

        topDownCameraPivot = Instantiate(topdownCameraPrefab);
        _topDownCamera = topDownCameraPivot.GetComponentInChildren<Camera>();

    }

    private void OnEnable()
    {
        GameEventManager.TriggerEvent(new LocalNetwork_EventType(LocalNetworkEventType.PlayerConnected, this.gameObject));
    }

    private void OnDisable()
    {
        GameEventManager.TriggerEvent(new LocalNetwork_EventType(LocalNetworkEventType.PlayerDisconnected, this.gameObject));
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            Fire();
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Move();
            Rotate();

            UpdateCamera();
        }
    }

    private void Move()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _forward = this.transform.forward;

        _motion = Vector3.zero;
        _bMoving = false;

        if (Mathf.Abs(_horizontal) > 0.0f)
        {
            _motion += Vector3.right * _horizontal * movementSpeed;
            _bMoving = true;
        }

        if (Mathf.Abs(_vertical) > 0.0f)
        {
            _forward.y = 0.0f;
            _forward.Normalize();

            _motion += Vector3.forward * _vertical * movementSpeed;
            _bMoving = true;
        }

        if (Vector3.Magnitude(_motion) > 0)
        {
            animationController.SetBool("isMoving", true);
        }
        else
        {
            animationController.SetBool("isMoving", false);
        }

        _motion += Physics.gravity;

        Vector3 velocity = rigidBody.velocity;
        _motion *= Time.fixedDeltaTime;

        //Apply velocity to the Player
        velocity.x = _motion.x;
        velocity.y = 0.0f;
        velocity.z = _motion.z;
        rigidBody.velocity = velocity;
    }

    private void Rotate()
    {
        Ray cameraRay = _topDownCamera.ScreenPointToRay(Input.mousePosition);

        if (_groundPlane.Raycast(cameraRay, out _rayLength))
        {
            _mouseTargetPosition = cameraRay.GetPoint(_rayLength);

            _mouseTargetPosition.y = this.transform.position.y;

            transform.LookAt(_mouseTargetPosition);
        }
    }

    private void UpdateCamera()
    {
        topDownCameraPivot.transform.position = this.transform.position;
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            CmdRequestProjectile(bulletSpawn.transform.position, _forward, gameObject);
        }
    }

    [Command]
    private void CmdRequestProjectile(Vector3 position, Vector3 targetPosition, GameObject obj)
    {
        GameObject newProjectile = Instantiate(bulletPrefab);

        newProjectile.GetComponent<SimpleProjectile>().InitProjectile(position, targetPosition, this);

        NetworkServer.Spawn(newProjectile);
    }
}
