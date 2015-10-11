using System.Collections;
﻿using UnityEngine;
﻿using UnityEngine.Networking;

[RequireComponent(typeof(CapsuleCollider))]
public class LaserController : NetworkBehaviour
{
	[System.NonSerialized] public uint OwnerID;
	public float Lifetime = 5.0f;
	public float Speed = 1.0f;

	protected float m_BirthTime;
	protected CapsuleCollider m_Collider;

	public void Awake()
	{
		m_Collider = GetComponent<CapsuleCollider>();
	}

	public void Start()
	{
		m_BirthTime = Time.time;
	}

	public void Update()
	{
		if(isServer && Time.time >= m_BirthTime + Lifetime) {
			NetworkServer.Destroy(this.gameObject);
		}
	}

	public void FixedUpdate()
	{
		if(isServer) {
			RaycastHit hit;

			Vector3 offset = 0.5f*m_Collider.height*colliderDirectionToVector(m_Collider.direction);
			Vector3 first = this.transform.TransformPoint(m_Collider.center + offset);
			Vector3 second = this.transform.TransformPoint(m_Collider.center - offset);
			if(Physics.CapsuleCast(first, second, m_Collider.radius, this.transform.forward, out hit, Speed*Time.deltaTime)) {
				destroyLaserObject(hit.collider);
			}
		}

		this.transform.position += Speed*this.transform.forward*Time.deltaTime;
	}

	public void OnTriggerEnter(Collider other)
	{
		if(isServer) {
			destroyLaserObject(other);
		}
	}

	protected void destroyLaserObject(Collider other)
	{
		GameObject target = other.gameObject;
		NetworkIdentity net_id = other.GetComponentInParent<NetworkIdentity>();
		Living player = null;

		if(net_id) {
			target = net_id.gameObject;
			player = target.GetComponent<Living>();
		}

		if(isServer) {
			switch(target.tag) {
				case "Player":
					if(net_id.netId.Value != OwnerID && player) {
						player.ApplyDamage(5.0f, "player:" + OwnerID.ToString("X8"));
					}
					break;

				default:
					break;
			}

			NetworkServer.Destroy(this.gameObject);
		}
	}

	protected Vector3 colliderDirectionToVector(int direction)
	{
		switch(direction) {
			case 0:
				return new Vector3(1.0f, 0.0f, 0.0f);

			case 1:
				return new Vector3(0.0f, 1.0f, 0.0f);

			case 2:
				return new Vector3(0.0f, 0.0f, 1.0f);

			default:
				return new Vector3(1.0f, 0.0f, 0.0f);
		}
	}
}
