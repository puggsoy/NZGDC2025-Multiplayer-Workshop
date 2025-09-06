using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject m_buttonContainer = null;

	[SerializeField]
	private GameObject m_backBtn = null;

	[Header("Host")]
	[SerializeField]
	private GameObject m_hostContainer = null;

	[SerializeField]
	private TMP_Text m_codeText = null;

	[Header("Join")]
	[SerializeField]
	private GameObject m_joinContainer = null;

	[SerializeField]
	private TMP_InputField m_joinInputField = null;

	public void SoloClick()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void HostClick()
	{
		m_buttonContainer.SetActive(false);
		m_hostContainer.SetActive(true);
		m_backBtn.SetActive(true);
	}

	public void JoinClick()
	{
		m_buttonContainer.SetActive(false);
		m_joinContainer.SetActive(true);
		m_backBtn.SetActive(true);
	}

	public void BackClick()
	{
		m_hostContainer.SetActive(false);
		m_joinContainer.SetActive(false);
		m_buttonContainer.SetActive(true);
		m_backBtn.SetActive(false);
	}
}
