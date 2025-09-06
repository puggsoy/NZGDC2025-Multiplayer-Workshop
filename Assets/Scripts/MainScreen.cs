using System.Collections.Generic;
using UnityEngine;

public class MainScreen : MonoBehaviour
{
	[SerializeField]
	private List<OptionUI> m_options = null;

	private void Start()
	{
		for (int i = 0; i < m_options.Count; i++)
		{
			m_options[i].OnSelected += OnOptionSelected;
		}
	}

	private void OnOptionSelected(OptionUI option, bool host)
	{
		for (int i = 0; i < m_options.Count; i++)
		{
			if (m_options[i] != option)
			{
				m_options[i].Deselect(host);
			}
		}
	}
}
