using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
﻿using UnityEngine.Networking;

public class Living : NetworkBehaviour
{
	[System.Serializable]
	public class DamageLogEntry
	{
		public string Source;
		public float Damage;
		public float Health;

		public DamageLogEntry(string source, float damage, float health)
		{
			this.Source = source;
			this.Damage = damage;
			this.Health = health;
		}

		public override string ToString()
		{
			return string.Format("<{0}: {2} ({1})>", Source, Damage, Health);
		}

		public static bool operator ==(DamageLogEntry left, DamageLogEntry right)
		{
			return left.Source == right.Source && left.Damage == right.Damage && left.Health == right.Health;
		}

		public static bool operator !=(DamageLogEntry left, DamageLogEntry right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = 23*hash + Source.GetHashCode();
			hash = 23*hash + Damage.GetHashCode();
			hash = 23*hash + Health.GetHashCode();

			return hash;
		}

		public override bool Equals(object other_obj)
		{
			DamageLogEntry other = (DamageLogEntry)other_obj;

			if(object.ReferenceEquals(other, null)) {
				return false;
			} else {
				return Source == other.Source && Damage == other.Damage && Health == other.Health;
			}
		}

		public static bool IsNull(object other_obj)
		{
			return object.ReferenceEquals(other_obj, null);
		}
	}

	public bool IsAlive {
		get {
			return Health > 0.0f;
		}
	}

	public float RemainingRespawnCooldown {
		get {
			return RespawnTime - (Time.time - m_DeathTime);
		}
	}

	[SyncVar(hook="OnHealthChange")]
	public float Health = 100.0f;
	[SyncVar]
	public float MaxHealth = 100.0f;
	[SyncVar]
	public float RespawnTime = 10.0f;

	public float ImpactDamageRatio = 0.5f;

	[Header("Death Handling")]
	public GameObject ExplosionPrefab;
	public float ExplosionPrefabLifetime = 10.0f;
	public bool DisableChildrenOnDeath = true;
	public bool DisableCollidersOnDeath = true;
	public bool DisableRigidbodyOnDeath = true;
	public List<Behaviour> ComponentsToDisableOnDeath;

	[Header("Input Names")]
	public string ResurrectInputName = "Fire1";

	protected Rigidbody m_Rigidbody;
	protected GameObject m_Explosion;
	protected bool m_WasAlive;
	protected float m_DeathTime = 0.0f;
	protected float m_PreviousVelocity;

	public void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
	}

	public void Start()
	{
		m_WasAlive = IsAlive;
		setLifeComponents(IsAlive);
	}

	public void Update()
	{
		if(!IsAlive && isLocalPlayer && Input.GetButtonDown(ResurrectInputName) && RemainingRespawnCooldown <= 0.0f) {
			CmdResurrectPlayer();
		}

		if(Input.GetButtonDown("Cancel") && isLocalPlayer) {
			CmdSelfDestruct();
		}
	}

	public void FixedUpdate()
	{
		if(m_Rigidbody) {
			m_PreviousVelocity = this.transform.InverseTransformDirection(m_Rigidbody.velocity).z;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if(isServer) {
			float damage = 0.0f;
			float max_contact = float.NegativeInfinity;

			foreach(ContactPoint contact in collision.contacts) {
				float dot_product = -1*Vector3.Dot(this.transform.TransformDirection(Vector3.forward).normalized, contact.normal);

				if(max_contact < dot_product) {
					max_contact = dot_product;
				}

				Debug.DrawRay(contact.point, 5*this.transform.forward, Color.blue, 10.0f);
				Debug.DrawRay(contact.point, 5*contact.normal, Color.red, 10.0f);
			}

			Debug.Log(ImpactDamageRatio + ":" + max_contact + ":" + m_PreviousVelocity);

			damage = ImpactDamageRatio*Mathf.Clamp01(max_contact*max_contact)*m_PreviousVelocity;

			if(damage >= 0.0f) {
				ApplyDamage(damage);
			}
		}
	}

	[Command]
	public void CmdSelfDestruct()
	{
		ApplyDamage(Mathf.Pow(MaxHealth, Health), "self");
	}

	[Command]
	public void CmdResurrectPlayer()
	{
		Health = MaxHealth;

		GameObject netmanager = GameObject.Find("Network Manager");
		NetworkManager netmgr = netmanager.GetComponent<NetworkManager>();
		Transform spawn_transform = netmgr.startPositions[(int)(netmgr.startPositions.Count*UnityEngine.Random.value)];

		this.transform.position = spawn_transform.position;
		this.transform.localRotation = spawn_transform.localRotation;
	}

	public void ApplyDamage(float damage, string source = "fate")
	{
		if(isServer) {
			float original_health = Health;
			Health -= damage;

			if(!IsAlive && original_health > 0.0f) {
				Regex source_filter = new Regex(@"player\:([\dA-Fa-f]{8})");
				MatchCollection matches = source_filter.Matches(source);
				ScoreHandler player_score = GetComponent<ScoreHandler>();

				if(player_score) {
					player_score.AddDeaths();
				}

				if(matches.Count > 0) {
					uint attacker_netid = Convert.ToUInt32(matches[0].Groups[1].Value, 16);
					ScoreHandler attacker_score;

					foreach(GameObject attacker in GameObject.FindGameObjectsWithTag("Player")) {
						attacker_score = attacker.GetComponent<ScoreHandler>();

						if(attacker_score && attacker_score.netId.Value == attacker_netid) {
							attacker_score.AddFrags();
						}
					}
				}
			}
		}
	}

	protected void OnHealthChange(float value)
	{
		Health = value;

		if(m_WasAlive != IsAlive) {
			if(Health <= 0.0f) {
				m_DeathTime = Time.time;

				if(ExplosionPrefab) {
					m_Explosion = (GameObject)Instantiate(ExplosionPrefab, this.transform.position, Quaternion.identity);
					Destroy(m_Explosion, ExplosionPrefabLifetime);
				}
			} else if(m_Explosion) {
				Destroy(m_Explosion);
			}
		}

		m_WasAlive = IsAlive;
		setLifeComponents(IsAlive);
	}

	protected void setLifeComponents(bool value)
	{
		if(DisableChildrenOnDeath) {
			foreach(Transform child in transform) {
				child.gameObject.SetActive(value);
			}
		}

		if(DisableCollidersOnDeath) {
			foreach(Collider collider in GetComponents<Collider>()) {
				collider.enabled = value;
			}
		}

		if(DisableRigidbodyOnDeath) {
			Rigidbody body = GetComponent<Rigidbody>();

			if(body) {
				body.isKinematic = !value;
			}
		}

		foreach(Behaviour component in ComponentsToDisableOnDeath) {
			component.enabled = value;
		}
	}
}
