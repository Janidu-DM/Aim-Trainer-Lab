using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementPhysics : MonoBehaviour
{
    [Header("MovementSettings")]
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _gravity = -19.75f;
    [SerializeField] private float _jumpHeight = 2f;

    [Header("GroundCheckSettings")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;

    public float CurrentSpeed //this property responsible for calculate realtime Speed of the character so other scripts can look
    {
        get
        {
            if (_characterController == null)
            {
                Debug.Log("Character Controller in physics engine is null");
                return 0f;
            }

            Vector3 flatVelocity = new Vector3(_characterController.velocity.x,0f,_characterController.velocity.z);
            return flatVelocity.magnitude; 
        }
    }
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    public void Move(Vector2 input)
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position,_groundDistance,_groundMask);

        if (_isGrounded && _velocity.y < 0) 
        {
            _velocity.y = -2f;
        }

        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;
        Vector3 finalVelocity = moveDirection * _moveSpeed;
        _velocity.y += _gravity * Time.deltaTime;
        finalVelocity.y = _velocity.y;

        _characterController.Move(finalVelocity * Time.deltaTime);

    }
    public void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(-2f *_gravity* _jumpHeight);

        }
    }
}
