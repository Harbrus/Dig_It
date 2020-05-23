using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{

	/// the current health of the character
	[ReadOnly]
	public int CurrentHealth;
	/// If this is true, this object can't take damage
	[ReadOnly]
	public bool Invulnerable = false;

	[Header("Health")]
	/// the initial amount of health of the object
	public int InitialHealth = 10;
	/// the maximum amount of health of the object
	public int MaximumHealth = 10;

	[Header("Damage")]
	public bool ImmuneToKnockback = false;

	/// the feedback to play when getting damage

	[Header("Death")]
	public bool DestroyOnDeath = true;
	/// the time (in seconds) before the character is destroyed or disabled
	public float DelayBeforeDestruction = 0f;
	/// if this is set to false, the character will respawn at the location of its death, otherwise it'll be moved to its initial position (when the scene started)
	public bool RespawnAtInitialLocation = false;
	/// if this is true, the controller will be disabled on death
	//public bool DisableControllerOnDeath = true;
	/// if this is true, collisions will be turned off when the character dies
	public bool DisableCollisionsOnDeath = true;

	/// the feedback to play when dying

	// hit delegate
	public delegate void OnHitDelegate();
	public OnHitDelegate OnHit; // can be called and a method could be assigned from other classes to define what happens when it is used here.
								// consider using Action/Func/Events instead https://docs.google.com/spreadsheets/d/16UAPOYnCByODiieQghWC9ldQkSCeeKpKaF_YrYLUOXc/edit?usp=sharing

	// respawn delegate
	public delegate void OnReviveDelegate();
	public OnReviveDelegate OnRevive;

	// death delegate
	public delegate void OnDeathDelegate();
	public OnDeathDelegate OnDeath;

	protected Vector3 _initialPosition;
	protected Renderer _renderer;
	protected Player _character;

	protected Collider2D _collider2D;
	protected Rigidbody2D _rigidbody;
	protected bool _initialized = false;
	protected Color _initialColor;
	protected Animator _animator;
	
	/// <summary>
	/// On Start, we initialize our health
	/// </summary>
	protected virtual void Start()
	{
		Initialization();
	}

	/// <summary>
	/// Grabs useful components, enables damage and gets the inital color
	/// </summary>
	protected virtual void Initialization()
	{
		_character = GetComponent<Player>();

		if (_renderer != null)
		{
			if (_renderer.material.HasProperty("_Color"))
			{
				_initialColor = _renderer.material.color;
			}
		}

		// we grab our animator
		if (_character != null)
		{
			if (_character.CharacterAnimator != null)
			{
				_animator = _character.CharacterAnimator;
			}
			else
			{
				_animator = GetComponent<Animator>();
			}
		}
		else
		{
			_animator = GetComponent<Animator>();
		}

		if (_animator != null)
		{
			_animator.logWarnings = false;
		}

		_rigidbody = GetComponent<Rigidbody2D>();
		_collider2D = GetComponent<Collider2D>();

		_initialPosition = transform.position;
		_initialized = true;
		CurrentHealth = InitialHealth;
		DamageEnabled();
	}

	/// <summary>
	/// When the object is enabled (on respawn for example), we restore its initial health levels
	/// </summary>
	protected virtual void OnEnable()
	{
		CurrentHealth = InitialHealth;
		DamageEnabled();
	}

	/// <summary>
	/// Called when the object takes damage
	/// </summary>
	/// <param name="damage">The amount of health points that will get lost.</param>
	/// <param name="instigator">The object that caused the damage.</param>
	/// <param name="flickerDuration">The time (in seconds) the object should flicker after taking the damage.</param>
	/// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
	public virtual void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration)
	{
		// if the object is invulnerable, we do nothing and exit
		if (Invulnerable)
		{
			return;
		}

		// if we're already below zero, we do nothing and exit
		if ((CurrentHealth <= 0) && (InitialHealth != 0))
		{
			return;
		}

		// we decrease the character's health by the damage
		float previousHealth = CurrentHealth;
		CurrentHealth -= damage;

		if (OnHit != null)
		{
			OnHit();
		}

		if (CurrentHealth < 0)
		{
			CurrentHealth = 0;
		}

		// we prevent the character from colliding with Projectiles, Player and Enemies
		if (invincibilityDuration > 0)
		{
			DamageDisabled();
			StartCoroutine(DamageEnabled(invincibilityDuration));
		}

		// we trigger a damage taken event

		if (_animator != null)
		{
			_animator.SetTrigger("Damage");
		}

		// if health has reached zero
		if (CurrentHealth <= 0)
		{
			// we set its health to zero (useful for the healthbar)
			CurrentHealth = 0;

			Kill();
		}
	}

	/// <summary>
	/// Kills the character, vibrates the device, instantiates death effects, handles points, etc
	/// </summary>
	public virtual void Kill()
	{
		if (_character != null)
		{
			// we set its dead state to true
			_character.CurrentState = PlayerState.Dead;
			//_character.Reset(); -> reset player position
		}
		CurrentHealth = 0;

		// we prevent further damage
		DamageDisabled();

		//DeathMMFeedbacks?.PlayFeedbacks(this.transform.position);

		if (_animator != null)
		{
			_animator.SetTrigger("Death");
		}
		// we make it ignore the collisions from now on
		if (DisableCollisionsOnDeath)
		{
			if (_collider2D != null)
			{
				_collider2D.enabled = false;
			}
			// removes collisions, restores parameters for a potential respawn, and applies a death force
		}

		OnDeath?.Invoke();

		if (DelayBeforeDestruction > 0f)
		{
			Invoke("DestroyObject", DelayBeforeDestruction);
		}
		else
		{
			// finally we destroy the object
			DestroyObject();
		}
	}

	/// <summary>
	/// Revive this object.
	/// </summary>
	public virtual void Revive()
	{
		if (!_initialized)
		{
			return;
		}

		if (_collider2D != null)
		{
			_collider2D.enabled = true;
		}

		this.gameObject.SetActive(true);
		// activate collisions and other components -> worth thinking having a controller - see top down controller

		if (_character != null)
		{
			_character.CurrentState = PlayerState.Idle;
		}
		if (_renderer != null)
		{
			_renderer.material.color = _initialColor;
		}

		if (RespawnAtInitialLocation)
		{
			transform.position = _initialPosition;
		}

		Initialization();

		if (OnRevive != null)
		{
			OnRevive();
		}
	}

	/// <summary>
	/// Destroys the object, or tries to, depending on the character's settings
	/// </summary>
	protected virtual void DestroyObject()
	{
		if (!DestroyOnDeath)
		{
			this.gameObject.SetActive(false);
		}
		else
		{
			Destroy(this);
		}

		// autorespawn / respawn
	}

	/// <summary>
	/// Called when the character gets health (from a stimpack for example)
	/// </summary>
	/// <param name="health">The health the character gets.</param>
	/// <param name="instigator">The thing that gives the character health.</param>
	public virtual void GetHealth(int health, GameObject instigator)
	{
		// this function adds health to the character's Health and prevents it to go above MaxHealth.
		CurrentHealth = Mathf.Min(CurrentHealth + health, MaximumHealth);
	}

	/// <summary>
	/// Resets the character's health to its max value
	/// </summary>
	public virtual void ResetHealthToMaxHealth()
	{
		CurrentHealth = MaximumHealth;
	}

	/// <summary>
	/// Prevents the character from taking any damage
	/// </summary>
	public virtual void DamageDisabled()
	{
		Invulnerable = true;
	}

	/// <summary>
	/// Allows the character to take damage
	/// </summary>
	public virtual void DamageEnabled()
	{
		Invulnerable = false;
	}

	/// <summary>
	/// makes the character able to take damage again after the specified delay
	/// </summary>
	/// <returns>The layer collision.</returns>
	public virtual IEnumerator DamageEnabled(float delay)
	{
		yield return new WaitForSeconds(delay);
		Invulnerable = false;
	}

	/// <summary>
	/// On Disable, we prevent any delayed destruction from running
	/// </summary>
	protected virtual void OnDisable()
	{
		CancelInvoke();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		// temporary, should be called from other scripts (weapon and inventory (?)).
		if (other.tag == "Weapon")
		{
			Damage(1, other.gameObject, 0, 0.5f);
		}
		
		if(this.CompareTag("Player") && other.CompareTag("Ghost"))
		{
			// lose jewel
		}
	}
}
