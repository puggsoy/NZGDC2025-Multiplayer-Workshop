using System;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
	public event Action<OptionUI, bool> OnSelected = null;

	[SerializeField]
	private GameObject m_hostText = null;

	[SerializeField]
	private GameObject m_clientText = null;

	//==================================================

	public void OnClick()
	{
		Select(true);
	}

	public void Select(bool host)
	{
		GameObject text = host ? m_hostText : m_clientText;

		text.SetActive(true);

		OnSelected?.Invoke(this, host);
	}

	public void Deselect(bool host)
	{
		GameObject text = host ? m_hostText : m_clientText;

		text.SetActive(false);
	}
}
