using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class UICameraMouseRotateWiggle : MonoBehaviour
{
	public Vector2 MaxOffset = new Vector2(6.0f, 2.0f);
	public float Speed = 0.01f;

	private Transform m_CameraAnchor;
	private Quaternion m_AnchoredRotation;

	void Start()
	{
		m_CameraAnchor = this.GetComponent<Transform>();
		m_AnchoredRotation = m_CameraAnchor.localRotation;
	}

	void Update()
	{
		Vector2 normalizedTravel = new Vector2(Mathf.Clamp(2*Input.mousePosition.x/Screen.width - 1.0f, -1.0f, 1.0f), Mathf.Clamp(2*Input.mousePosition.y/Screen.height - 1.0f, -1.0f, 1.0f));
		m_CameraAnchor.localRotation = Quaternion.Lerp(m_CameraAnchor.localRotation, m_AnchoredRotation*Quaternion.Euler(-MaxOffset.y*normalizedTravel.y, MaxOffset.x*normalizedTravel.x, 0.0f), Time.deltaTime*Speed);
	}
}
