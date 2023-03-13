using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
public class RelaySetting
{
	public static IEnumerator StartClient(string inCode, Action<bool> inOnJoin)
	{
		var clientRelayUtilityTask = JoinRelayServerFromJoinCode(inCode);
		while(!clientRelayUtilityTask.IsCompleted)
		{
			yield return null;
		}
		if(clientRelayUtilityTask.IsFaulted)
		{
			inOnJoin(false);
			yield break;
		}
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(clientRelayUtilityTask.Result);
		inOnJoin(true);
		yield return null;
	}
	public static IEnumerator StartHost(int inCount, Action<bool, string> inOnJoin)
	{
		var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(inCount);
		while(!serverRelayUtilityTask.IsCompleted)
		{
			yield return null;
		}
		if(serverRelayUtilityTask.IsFaulted)
		{
			inOnJoin(false, null);
			yield break;
		}
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverRelayUtilityTask.Result.data);
		inOnJoin(true, serverRelayUtilityTask.Result.code);
		yield return null;
	}
	static async Task AuthenticatingAPlayer()
	{
		try
		{
			await UnityServices.InitializeAsync();
			if (!AuthenticationService.Instance.IsSignedIn)
			{
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
			}
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}
	static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
	{
		await AuthenticatingAPlayer();
		var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
		return new RelayServerData(allocation, "dtls");
	}
	static async Task<(RelayServerData data, string code)> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
	{
		await AuthenticatingAPlayer();
		var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
		var createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
		return (new RelayServerData(allocation, "dtls"), createJoinCode);
	}
}
