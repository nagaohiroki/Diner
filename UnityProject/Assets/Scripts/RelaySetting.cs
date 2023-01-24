using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
public class RelaySetting
{
	public static IEnumerator StartClient(string inCode, Action inOnJoin)
	{
		var clientRelayUtilityTask = JoinRelayServerFromJoinCode(inCode);
		while(!clientRelayUtilityTask.IsCompleted)
		{
			yield return null;
		}
		if(clientRelayUtilityTask.IsFaulted)
		{
			Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
			yield break;
		}
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(clientRelayUtilityTask.Result);
		inOnJoin();
		yield return null;
	}
	public static IEnumerator StartHost(int inCount, Action<string> inOnJoin)
	{
		var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(inCount);
		while(!serverRelayUtilityTask.IsCompleted)
		{
			yield return null;
		}
		if(serverRelayUtilityTask.IsFaulted)
		{
			Debug.LogError("Exception thrown when attempting to start Relay Server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
			yield break;
		}
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverRelayUtilityTask.Result.data);
		inOnJoin(serverRelayUtilityTask.Result.code);
		yield return null;
	}
	static async Task AuthenticatingAPlayer()
	{
		try
		{
			await UnityServices.InitializeAsync();
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
	static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
	{
		await AuthenticatingAPlayer();
		JoinAllocation allocation;
		try
		{
			allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
		}
		catch
		{
			Debug.LogError("Relay create join code request failed");
			throw;
		}
		Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
		Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
		Debug.Log($"client: {allocation.AllocationId}");
		return new RelayServerData(allocation, "dtls");
	}
	static async Task<(RelayServerData data, string code)> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
	{
		await AuthenticatingAPlayer();
		Allocation allocation;
		string createJoinCode;
		try
		{
			allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
		}
		catch(Exception e)
		{
			Debug.LogError($"Relay create allocation request failed {e.Message}");
			throw;
		}

		Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
		Debug.Log($"server: {allocation.AllocationId}");
		try
		{
			createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
		}
		catch
		{
			Debug.LogError("Relay create join code request failed");
			throw;
		}
		Debug.Log(createJoinCode);
		return (new RelayServerData(allocation, "dtls"), createJoinCode);
	}
}
