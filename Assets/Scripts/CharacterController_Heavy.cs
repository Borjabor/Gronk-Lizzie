using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterController_Heavy : Entity
{
	
	[SerializeField] 
	private LiveCount _liveCount;

	[Range(0, .3f)] [SerializeField]
	private float _movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField]
	private bool _airControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField]
	private LayerMask _whatIsGround;							// A mask determining what is ground to the character
	[SerializeField]
	private Transform _groundCheck;							// A position marking where to check if the player is grounded.

	const float _groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool _grounded;            // Whether or not the player is grounded.
	//private Rigidbody2D _rb;
	private bool _facingRight = true;  // For determining which way the player is currently facing.
	private Vector2 _velocity = Vector2.zero;
    private Vector2 _lastGrounded;
    private Vector2 _checkpoint;
    private bool _isRespawning = false;
    [SerializeField]
    private float _iFramesDuration;
    [SerializeField]
    private int _numberOfFlashes;
    [SerializeField] 
    private SpriteRenderer _bodyRenderer;
	private Animator _animator;

	[Header("Audio")]
	private AudioSource _audioSource;
	[SerializeField] 
	private AudioClip _buffPickupAudio;
	[SerializeField] 
	private AudioClip _checkpointAudio;
	[SerializeField]
	private AudioClip _jumpAudio;
	[SerializeField]
    private AudioClip _landAudio;
	[SerializeField]
	private AudioClip _deathAudio;


    [SerializeField]
    private float _moveSpeed = 60f;
	private float _horizontalMove = 0f;

	[Header("Knockback")]
    [SerializeField]
    private float _knockbackX = 25f;
	[SerializeField]
    private float _knockbackY = 5f;

    [Header("Jump")]
	[SerializeField]
	private float _jumpForce = 30f;	// Amount of force added when the player jumps.
	[SerializeField] 
	private float _fallMultiplier = 2.5f; //not in use
	//[SerializeField] 
	private float _lowJumpMultiplier = 0.5f; //value becomes 4 when serialized, for some reason
	[SerializeField] 
	private float _coyoteTime = 0.2f;
	private float _coyoteTimeCounter;
	[SerializeField] 
	private float _jumpBufferTime = 0.2f;
	private float _jumpBufferCounter;
	private bool _jump = false;
	[Tooltip("Drag Boxes here")]
	[SerializeField] private Bounceable[] _poundBounceObjects;
	
    
    private bool _onFallingPlatform;
    private bool _onEdge;

    [Header("Sprites")]
	[SerializeField]
	private GameObject _characterSprite;

	private SpriteRenderer _sprite;

	[Header("Particles")]
	[SerializeField]
	private ParticleSystem _deathParticles;
	[SerializeField]
	private ParticleSystem _moveParticles;
	[SerializeField]
	private ParticleSystem _jumpParticles;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	


	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


	private void Awake()
	{
		_sprite = GetComponent<SpriteRenderer>();
		_coyoteTimeCounter = _coyoteTime;
        _checkpoint = transform.position;
		_audioSource = GetComponent<AudioSource>();
		//_animator = GetComponentInChildren<Animator>();

        if (OnLandEvent == null) OnLandEvent = new UnityEvent();

    }

	void Update()
	{
		if(!_isRespawning) GetInputs();
		_coyoteTimeCounter -= Time.deltaTime;
		
		if (_horizontalMove != 0)
		{
			//_moveParticles.Play();
			//_animator.SetBool("Walking", true);
			if (!_audioSource.isPlaying)
			{
				_audioSource.Play();
			}
		}
		else
		{
			//_animator.SetBool("Walking", false);
		}
	}

	private void FixedUpdate()
	{
		if(_grounded) _lastGrounded = transform.position;
		if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f) _jump = true;

        Move(_horizontalMove * Time.fixedDeltaTime, _jump);
        
        if (_rb.velocity.y > 0f && Input.GetKeyUp(KeyCode.W))
        {
	        _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _lowJumpMultiplier);
	        _coyoteTimeCounter = 0f;
        }
        
        
        
        bool wasGrounded = _grounded;
		_grounded = false;
		
		_jump = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				_grounded = true;
				_coyoteTimeCounter = _coyoteTime;
				//_animator.SetTrigger("Idle");
				if (!wasGrounded) OnLandEvent.Invoke();
			}
		}
	}


	private void Move(float move, bool jump)
	{

		//only control the player if grounded or airControl is turned on
		if (_grounded || _airControl)
		{
			// Move the character by finding the target velocity
			Vector2 targetVelocity = new Vector2(move * 10f, _rb.velocity.y);
			// And then smoothing it out and applying it to the character
			_rb.velocity = Vector2.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, _movementSmoothing);

			// Flip player if input is opposite of way sprite is facing
			if (move > 0 && !_facingRight)
			{
				Flip();
			}
			else if (move < 0 && _facingRight)
			{
				Flip();
			}
		}
		
		if (jump)
		{
			if( _coyoteTimeCounter > 0f && _jumpBufferCounter > 0f){
				//_animator.SetTrigger("Jump");
				_audioSource.PlayOneShot(_jumpAudio);
				_jumpBufferCounter = 0f;
				_rb.velocity = Vector2.up * _jumpForce;
				_grounded = false;
				_onFallingPlatform = false;
				_onEdge = false;
                //_moveParticles.Stop();
                //_jumpParticles.Play();
            }
			
		}

		
	}

	private void GetInputs()
	{		
        _horizontalMove = Input.GetAxisRaw("Horizontal1") * _moveSpeed;
        if (Input.GetKeyDown(KeyCode.W))
        {
	        _jump = true;
	        _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
	        _jumpBufferCounter -= Time.deltaTime;
        }
    }

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;
		//_sprite.flipX = _facingRight ? false : true;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.CompareTag("Checkpoint"))
		{
			_audioSource.PlayOneShot(_checkpointAudio);
			_checkpoint = other.transform.position;
		}
		
		/*if (other.gameObject.CompareTag("Collectible"))
		{
			Destroy(other.gameObject);
			_audioSource.PlayOneShot(_buffPickupAudio);
			CollectiblesCounter.TotalPoints++;
		}*/
	}

	private void OnCollisionStay2D(Collision2D other)
	{
		foreach(ContactPoint2D hitPos in other.contacts)
		{
            if(hitPos.normal.y <= 0  && other.gameObject.CompareTag("Enemy"))
            {
                StartCoroutine(Respawn());
            }
			
			else if(hitPos.normal.y > 0  && other.gameObject.CompareTag("Enemy"))
            {
                _rb.velocity = Vector2.up * (_jumpForce/2);
                _coyoteTimeCounter = _coyoteTime;
            }
			
			/*if(hitPos.normal.y >= 0  && other.gameObject.CompareTag("FallingPlatform"))
			{
                _onFallingPlatform = true;
                _onEdge = false;
                _coyoteTimeCounter = _coyoteTime;
            }
			else
			{
                _onFallingPlatform = false;
            }
			
			if(hitPos.normal.y >= 0  && other.gameObject.CompareTag("Edge"))
			{
				_onEdge = true;
				_onFallingPlatform = false;
				_coyoteTimeCounter = _coyoteTime;
			}*/
        }
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		/*if(other.gameObject.CompareTag("FallingPlatform"))
		{
			_onFallingPlatform = false;
		}

		if(other.gameObject.CompareTag("Edge"))
		{
			_onEdge = false;
		}*/
	}

	private void OnCollisionEnter2D(Collision2D other)
	{

		foreach(ContactPoint2D hitPos in other.contacts)
		{
			if(hitPos.normal.y > 0  && other.gameObject.CompareTag("Enemy"))
            {
                _rb.velocity = Vector2.up * (_jumpForce/2);
                _coyoteTimeCounter = _coyoteTime;
            }
			
			if(hitPos.normal.y > 0  && other.gameObject.layer == 6)
			{
				foreach (var box in _poundBounceObjects)
				{
					CameraShake.Instance.ShakeCamera(3f, 0.1f);
					box.Bounce();
				}
				
			}
			
			/*if(hitPos.normal.y >= 0  && other.gameObject.CompareTag("FallingPlatform"))
			{
                _onFallingPlatform = true;
                _onEdge = false;
                _coyoteTimeCounter = _coyoteTime;
            }
			
			if(hitPos.normal.y >= 0  && other.gameObject.CompareTag("Edge"))
			{
				_onEdge = true;
				_onFallingPlatform = false;
				_coyoteTimeCounter = _coyoteTime;
			}*/
        }
		
	}

	private IEnumerator Respawn()
	{
		_isRespawning = true;
		_liveCount.LoseLife();
		_rb.velocity = Vector2.zero;
		_audioSource.PlayOneShot(_deathAudio);
		_characterSprite.SetActive(false);
		_deathParticles.Play();
		yield return new WaitForSeconds(1.5f);
		if(_liveCount._remainingLives <= 0)
		{
			_liveCount._remainingLives = 5;
			transform.position = _checkpoint;
		}
		else
		{
			transform.position = _lastGrounded;
		}
		_characterSprite.SetActive(true);
		_isRespawning = false;
		
        
		StartCoroutine(Invulnerability());

    }

	private IEnumerator Invulnerability()
	{
		
		for (int i = 0; i < _numberOfFlashes; i++)
		{
			_bodyRenderer.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
			yield return new WaitForSeconds(_iFramesDuration / (_numberOfFlashes * 2));
			_bodyRenderer.color = Color.white;
			yield return new WaitForSeconds(_iFramesDuration / (_numberOfFlashes * 2));
		}
		Physics2D.IgnoreLayerCollision(0, 6, false);
	}

}
