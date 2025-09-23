using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using System;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
	private struct SelectBroadcast : IBroadcast
	{
		public string ObjectName;
		public bool SentByHost;
	}

	public event Action<OptionUI, bool> OnSelected = null;

	[SerializeField]
	private GameObject m_hostText = null;

	[SerializeField]
	private GameObject m_clientText = null;

	//==================================================

	private void OnEnable()
	{
		InstanceFinder.ServerManager.RegisterBroadcast<SelectBroadcast>(Server_SelectBroadcastHandler);
		InstanceFinder.ClientManager.RegisterBroadcast<SelectBroadcast>(Client_SelectBroadcastHandler);
	}

	private void Client_SelectBroadcastHandler(SelectBroadcast broadcast, Channel channel)
	{
		SelectBroadcastHandler(broadcast);
	}

	private void Server_SelectBroadcastHandler(NetworkConnection connection, SelectBroadcast broadcast, Channel channel)
	{
		SelectBroadcastHandler(broadcast);
	}

	private void SelectBroadcastHandler(SelectBroadcast broadcast)
	{
		if (broadcast.ObjectName == name)
		{
			Select(broadcast.SentByHost);
		}
	}

	public void OnClick()
	{
		bool isHost = InstanceFinder.IsHostStarted;

		Select(isHost);

		SelectBroadcast broadcast = new SelectBroadcast()
		{
			ObjectName = name,
			SentByHost = isHost
		};

		if (isHost)
		{
			InstanceFinder.ServerManager.Broadcast(broadcast);
		}
		else
		{
			InstanceFinder.ClientManager.Broadcast(broadcast);
		}
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

	private void OnDisable()
	{
		InstanceFinder.ServerManager.UnregisterBroadcast<SelectBroadcast>(Server_SelectBroadcastHandler);
		InstanceFinder.ClientManager.UnregisterBroadcast<SelectBroadcast>(Client_SelectBroadcastHandler);
	}
}
