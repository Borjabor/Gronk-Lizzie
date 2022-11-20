using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterController_Light : Entity
{
	[SerializeField] 
	private Rigidbody2D _heavyRb;

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

	//Audio
	private AudioSource _audioSource;
	[SerializeField] 
	private AudioClip _buffPickupAudio;
	[SerializeField] 
	private AudioClip _checkpointAudio;
	[SerializeField]
	private AudioClip _jumpAudio;
	[SerializeField]
    private AudioClip _dashAudio;
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
	private float _jumpForce = 30f;						// Amount of force added when the player jumps.
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

    [Header("Dash")]
	[SerializeField]
	private float _dashAmount = 10f;
	[SerializeField]
    private float _dashingTime = 0.25f;
    [SerializeField]
	private float _dashingCooldown = 1f;
    private bool _dash;
    private bool _canDash = true;
    private bool _isDashing;
    
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
	[SerializeField]
	private TrailRenderer _dashTrail;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	private bool _isOnHeavy = false;


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
		if (_isDashing) return;
		if(!_isRespawning) GetInputs();
		if(_dash && _canDash) StartCoroutine(Dash());
		_coyoteTimeCounter -= Time.deltaTime;
		if(_horizontalMove != 0 && _rb.velocity.y == 0)
		{
			if (!_audioSource.isPlaying)
			{
				_audioSource.Play();
			}
		}
	}

	private void FixedUpdate()
	{
		if(_isDashing) return;
		if(_grounded) _lastGrounded = transform.position;
		if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f) _jump = true;

        Move(_horizontalMove * Time.fixedDeltaTime, _jump);
        
        if (_rb.velocity.y > 0f && Input.GetKeyUp(KeyCode.UpArrow))
        {
	        _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * _lowJumpMultiplier);
	        _coyoteTimeCounter = 0f;
        }
        
        if (_horizontalMove != 0 && _rb.velocity.y == 0)
        {
	        //_moveParticles.Play();
	        //_animator.SetBool("Walking", true);
        }
        else
        {
	        //_animator.SetBool("Walking", false);
        }

        bool wasGrounded = _grounded;
		_grounded = false;
		
		_jump = false;
		_dash = false;

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
			//Vector2 parentVelocity = transform.parent == null ? Vector2.zero : _heavyRb.velocity;
			Vector2 parentVelocity = _isOnHeavy ? new Vector2(_heavyRb.velocity.x, 0f) : Vector2.zero;
			_rb.velocity = Vector2.SmoothDamp(_rb.velocity, targetVelocity + parentVelocity, ref _velocity, _movementSmoothing);

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
        _horizontalMove = Input.GetAxisRaw("Horizontal2") * _moveSpeed;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
	        _jump = true;
	        _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
	        _jumpBufferCounter -= Time.deltaTime;
        }
		if(Input.GetKey(KeyCode.RightShift) && !_isRespawning) _dash = true;
    }

	public void Flip()
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
		
		if (other.gameObject.CompareTag("Collectible"))
		{
			Destroy(other.gameObject);
			_audioSource.PlayOneShot(_buffPickupAudio);
			CollectiblesCounter.TotalPoints++;
		}
		
		if(other.gameObject.CompareTag("Hazard") && !_isRespawning)
		{
			//_rb.AddForce(new Vector2(-transform.localScale.x * _knockbackX, _knockbackY), ForceMode2D.Impulse);
			StartCoroutine(Respawn());
		}
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
            
            if(hitPos.normal.y > 0  && other.gameObject.CompareTag("Heavy"))
            {
	            //transform.SetParent(other.gameObject.transform);
	            _isOnHeavy = true;
	            _grounded = true;
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
		if(other.gameObject.CompareTag("Heavy"))
		{
			transform.parent = null;
			_isOnHeavy = false;
		}
		
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
		if(other.gameObject.CompareTag("Hazard") && !_isRespawning)
		{
            //_rb.AddForce(new Vector2(-transform.localScale.x * _knockbackX, _knockbackY), ForceMode2D.Impulse);
            StartCoroutine(Respawn());
        }

		foreach(ContactPoint2D hitPos in other.contacts)
		{
            if(hitPos.normal.y <= 0  && other.gameObject.CompareTag("Enemy"))
            {
	            Physics2D.IgnoreLayerCollision(0, 6, true);
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
			
			if(hitPos.normal.y >= 0  && other.gameObject.CompareTag("Edge"))
			{
				_onEdge = true;
				_onFallingPlatform = false;
				_coyoteTimeCounter = _coyoteTime;
			}*/
        }
		
	}

	private IEnumerator Dash()
	{
        _canDash = false;
        _isDashing = true;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        _rb.velocity = new Vector2(transform.localScale.x * _dashAmount, 0f);
        _dashTrail.emitting = true;
        //_animator.SetTrigger("Dash");
		_audioSource.PlayOneShot(_dashAudio);
        yield return new WaitForSeconds(_dashingTime);
		_dashTrail.emitting = false;
        _rb.gravityScale = originalGravity;
        _isDashing = false;
		yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
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
