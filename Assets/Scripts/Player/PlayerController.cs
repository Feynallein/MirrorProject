using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [Header("Movement Setup")]
    [Tooltip("Translation speed in m/s")]
    [SerializeField] float _TranslationSpeed;
    [Tooltip("Rotation speed in °/s")]
    [SerializeField] float _RotationSpeed;
    [Tooltip("Interpolation speed [0;1]")]
    [SerializeField] float _InterpolationSpeed;
    [Tooltip("Strength for jumps")]
    [SerializeField] float _JumpingStrength;

    [Header("Other Settings")]
    [Tooltip("Which layers should be considered as ground")]
    [SerializeField] LayerMask _GroundLayer;

    // The player's rigid body
    Rigidbody _Rigidbody;

    // Is the player on the ground
    bool _IsGrounded;

    // Rigidbody of what's the player is helding. Null if the player isn't holding anything
    Rigidbody _HeldedItemRigidbody;

    // Getter to _IsGrounded, return true if the player is grounded
    public bool IsGrounded { get { return _IsGrounded; } }

    private void Start() {
        _Rigidbody = GetComponent<Rigidbody>(); // The player's rigidbody could be serialized
    }

    void FixedUpdate() {
        float horizontalInput, verticalInput;

        // Getting axis
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Moving toward the x
        Vector3 moveVect = transform.forward * _TranslationSpeed * Time.fixedDeltaTime * verticalInput;
        _Rigidbody.MovePosition(_Rigidbody.position + moveVect);

        // Rotating the player based on the horizontal input
        float rotAngle = horizontalInput * _RotationSpeed * Time.fixedDeltaTime;
        Quaternion qRot = Quaternion.AngleAxis(rotAngle, transform.up);
        Quaternion qRotUpRight = Quaternion.FromToRotation(transform.up, Vector3.up);
        Quaternion qOrientationUpRightTarget = qRotUpRight * _Rigidbody.rotation;
        Quaternion qNewUpRightOrientation = Quaternion.Slerp(_Rigidbody.rotation, qOrientationUpRightTarget, Time.fixedDeltaTime * _InterpolationSpeed); // We're using a slerp for a smooth movement
        _Rigidbody.MoveRotation(qRot * qNewUpRightOrientation);

        // This prevents the player from falling/keep moving if there is no input
        if (_IsGrounded) _Rigidbody.velocity = Vector3.zero;
        _Rigidbody.angularVelocity = Vector3.zero;

        if (!_IsGrounded) return; // What's below this won't be executed while jumping
        if (Input.GetKey(KeyCode.Space)) Jump();
    }

    void Jump() {
        _IsGrounded = false;
        _Rigidbody.AddForce(_JumpingStrength * transform.up, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision) {
        // Checking if the player encountered a ground object
        if ((_GroundLayer & (1 << collision.gameObject.layer)) > 0) _IsGrounded = true;
    }
}