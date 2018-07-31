using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController_old : NetworkBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidBody;
    [SerializeField]
    private GameObject topdownCameraPrefab;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private CapsuleCollider capsuleCollider;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float jumpForce = 500.0f;

    [Header("Animation")]
    [SerializeField]
    private float moveVelocityThreshold;

    [Header("Stats")]
    [SerializeField]
    private float maxHealth;

    [Header("Shooting")]
    [SerializeField]
    private LayerMask projectileLayermask;
    [SerializeField]
    private SimpleProjectile projectilePrefab;
    [SerializeField]
    private Transform projectileOrigin;

    [SerializeField]
    private GameObject dummyPrefab;

    private GameObject topDownCameraPivot;
    private Camera topDownCamera;

    private int walkAnimationId;
    private bool isGrounded;

    [SyncVar]
    private float currenthealth;

    private void Start()
    {
        if (isLocalPlayer)
        {
            topDownCameraPivot = Instantiate(topdownCameraPrefab);
            topDownCamera = topDownCameraPivot.GetComponentInChildren<Camera>();
        }

        walkAnimationId = Animator.StringToHash("Walk");

        currenthealth = maxHealth;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            CheckForProjectile();

            if (isServer)
            {
                SpawnDummyPrefab();
            }
        }

        UpdateAnimation();
    }

    private void SpawnDummyPrefab()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameObject prefabInstance = Instantiate(dummyPrefab);
            prefabInstance.transform.position = transform.position + transform.forward * 1.5f;

            NetworkServer.Spawn(prefabInstance);
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Move();
        }

        UpdateGroundedState();
    }

    private void CheckForProjectile()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit result;
            if (Physics.Raycast(topDownCamera.ScreenPointToRay(Input.mousePosition), out result, 100.0f, projectileLayermask))
            {
                Vector3 cachedPosition = projectileOrigin.transform.position;
                Vector3 targetPosition = result.point;
                targetPosition.y = cachedPosition.y;

                //This runs on the local authority client
                CmdRequestProjectile(cachedPosition, targetPosition - cachedPosition, gameObject);
            }
        }
    }

    [Command]
    private void CmdRequestProjectile(Vector3 position, Vector3 targetPosition, GameObject obj)
    {
        //This runs on the server
        RpcSpawnProjectile(position, targetPosition);
    }

    [ClientRpc]
    private void RpcSpawnProjectile(Vector3 position, Vector3 targetPosition)
    {
        SimpleProjectile newProjectile = Instantiate(projectilePrefab);
        //newProjectile.InitProjectile(position, targetPosition, this);
    }

    private void UpdateGroundedState()
    {
        isGrounded = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, capsuleCollider.radius - 0.01f);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != capsuleCollider.gameObject)
            {
                isGrounded = true;
            }
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool(walkAnimationId, IsWalking());
    }

    private void Move()
    {
        Vector3 motion = Vector3.zero;
        bool bMoving = false;

        float horizontalMove = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalMove) > 0.0f)
        {
            motion += topDownCamera.transform.right * horizontalMove * moveSpeed;
            bMoving = true;
        }

        float verticalMove = Input.GetAxis("Vertical");
        if (Mathf.Abs(verticalMove) > 0.0f)
        {
            Vector3 forward = topDownCamera.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();

            motion += forward * verticalMove * moveSpeed;
            bMoving = true;
        }

        motion += Physics.gravity;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            motion += Vector3.up * jumpForce;
            isGrounded = false;
        }

        Vector3 velocity = rigidBody.velocity;
        motion *= Time.fixedDeltaTime;

        velocity.x = motion.x;
        velocity.y += motion.y;
        velocity.z = motion.z;
        rigidBody.velocity = velocity;

        if (bMoving)
        {
            Vector3 lookDirection = motion;
            lookDirection.y = 0.0f;
            rigidBody.MoveRotation(Quaternion.LookRotation(lookDirection, Vector3.up));
        }

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        topDownCameraPivot.transform.position = transform.position;
    }

    private bool IsWalking()
    {
        Vector3 velocity = rigidBody.velocity;
        return Mathf.Abs(velocity.x) > moveVelocityThreshold || Mathf.Abs(velocity.z) > moveVelocityThreshold;
    }

    public void Damage(float damageAmount)
    {
        currenthealth -= damageAmount;

        if (currenthealth <= 0.0f)
        {
            enabled = false;
            capsuleCollider.gameObject.SetActive(false);
        }
    }
}
