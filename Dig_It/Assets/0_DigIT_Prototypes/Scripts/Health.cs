﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
	/// the current health of the character
	public int CurrentHealth;
	/// If this is true, this object can't take damage
	public bool Invulnerable = false;

	[Header("Health")]
	/// the initial amount of health of the object
	public int InitialHealth = 3;
	/// the maximum amount of health of the object
	public int MaximumHealth = 10;

	/// the feedback to play when getting damage

	[Header("Death")]
	public bool DestroyOnDeath = true;
	/// the time (in seconds) before the character is destroyed or disabled
	public float DelayBeforeDestruction = 0f;
	/// if this is true, collisions will be turned off when the character dies
	public bool DisableCollisionsOnDeath = true;

	/// the feedback to play when dying

	protected Vector3 _initialPosition;
	protected Collider2D _collider2D;
	protected Rigidbody2D _rigidbody;
	protected bool _initialized = false;
	protected Color _initialColor;
	
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
	public virtual void Damage(int damage, float flickerDuration, float invincibilityDuration)
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
		CurrentHealth = 0;

		// we prevent further damage
		DamageDisabled();

		// we make it ignore the collisions from now on
		if (DisableCollisionsOnDeath)
		{
			if (_collider2D != null)
			{
				_collider2D.enabled = false;
			}
			// do other effects
		}

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

		Initialization();
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
	}

	/// <summary>
	/// Called when the character gets health (from a stimpack for example)
	public virtual void IncreaseHelath(int health)
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
}
