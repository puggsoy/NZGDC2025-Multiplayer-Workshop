using FishNet;
using FishNet.Connection;
using FishNet.Transporting.UTP;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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

	private void Start()
	{
		m_buttonContainer.SetActive(false);

		InitializeService();
	}

	private async void InitializeService()
	{
		await UnityServices.InitializeAsync();

		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		m_buttonContainer.SetActive(true);
	}

	public void SoloClick()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void HostClick()
	{
		m_buttonContainer.SetActive(false);
		m_hostContainer.SetActive(true);

		SetupHost();
	}

	private async void SetupHost()
	{
		m_codeText.text = "Getting code...";

		Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
		string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

		UnityTransport transport = InstanceFinder.TransportManager.GetTransport<UnityTransport>();
		RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
		transport.SetRelayServerData(relayServerData);

		if (!InstanceFinder.ServerManager.StartConnection())
		{
			//error handling
			return;
		}

		if (!InstanceFinder.ClientManager.StartConnection())
		{
			//error handling
			return;
		}

		m_codeText.text = joinCode;

		m_backBtn.SetActive(true);

		InstanceFinder.ServerManager.OnAuthenticationResult += ServerOnAuthenticationResult;
	}

	private void ServerOnAuthenticationResult(FishNet.Connection.NetworkConnection connection, bool authenticated)
	{
		Dictionary<int, NetworkConnection> clients = InstanceFinder.ServerManager.Clients;

		if (clients.Count == 2)
		{
			foreach (NetworkConnection client in clients.Values)
			{
				if (!client.IsAuthenticated)
				{
					return;
				}
			}

			SceneManager.LoadScene("MainScene");
		}
	}

	public void JoinClick()
	{
		m_buttonContainer.SetActive(false);
		m_joinContainer.SetActive(true);
		m_backBtn.SetActive(true);
	}

	public void SubmitClick()
	{
		m_backBtn.SetActive(false);

		JoinGame();
	}

	public async void JoinGame()
	{
		JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(m_joinInputField.text);

		if (allocation == null)
		{
			//error handling
			return;
		}

		UnityTransport transport = InstanceFinder.TransportManager.GetTransport<UnityTransport>();
		RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
		transport.SetRelayServerData(relayServerData);

		InstanceFinder.ClientManager.OnAuthenticated += ClientOnAuthenticated;
		
		if (!InstanceFinder.ClientManager.StartConnection())
		{
			//error handling
			InstanceFinder.ClientManager.OnAuthenticated -= ClientOnAuthenticated;
		}
	}

	private void ClientOnAuthenticated()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void BackClick()
	{
		InstanceFinder.ServerManager.StopConnection(true);
		InstanceFinder.ClientManager.StopConnection();

		m_hostContainer.SetActive(false);
		m_joinContainer.SetActive(false);
		m_buttonContainer.SetActive(true);
		m_backBtn.SetActive(false);
	}
}
