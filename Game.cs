using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	private Player _player;
    private Lobby _lobby;

	private Dictionary<long, Player> _peerIdToPlayer;
    private Vector2 _globalPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_peerIdToPlayer = new Dictionary<long, Player>();
		_globalPosition = new Vector2(0, 0);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("MoveRight"))
		{
			Rpc(MethodName.MovePlayerRight, Multiplayer.MultiplayerPeer.GetUniqueId());
		}

		if (Input.IsActionPressed("MoveLeft"))
		{
			Rpc(MethodName.MovePlayerLeft, Multiplayer.MultiplayerPeer.GetUniqueId());
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void MovePlayerRight(long playerId)
	{
		// var playerId = Multiplayer.GetRemoteSenderId();
		var player = _peerIdToPlayer[playerId];
		GD.Print($"{Multiplayer.MultiplayerPeer.GetUniqueId()} Move {playerId}");
		// Rpc(MethodName.MovePlayerClient, playerId);
		player.Move(Vector2.Right * 10);
		player.Move(Vector2.Up * 10);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void MovePlayerLeft(long playerId)
	{
		// var playerId = Multiplayer.GetRemoteSenderId();
		var player = _peerIdToPlayer[playerId];
		GD.Print($"{Multiplayer.MultiplayerPeer.GetUniqueId()} Move {playerId}");
		// Rpc(MethodName.MovePlayerClient, playerId);
		player.Move(Vector2.Left * 10);
		player.Move(Vector2.Up * 10);
	}

	// [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]

	// public void MovePlayerClient(long playerId)
	// {
	// 	var player = _peerIdToPlayer[playerId];
	// 	player.Move(Vector2.Right * 10);
	// }


	public void StartGame(Lobby lobby)
	{
		_lobby = lobby;
		Multiplayer.MultiplayerPeer = lobby.Multiplayer.MultiplayerPeer;
		var playerScene = GD.Load<PackedScene>("uid://c4na1fi5ewo8s");

		foreach(var key in _lobby._players.Keys)
		{
			var playerInfo = _lobby._players[key];
			var player = playerScene.Instantiate<Player>();
			AddChild(player);
			_peerIdToPlayer.Add(key, player);
			player.ChangeColor(playerInfo.Color);
		}
	}
}
