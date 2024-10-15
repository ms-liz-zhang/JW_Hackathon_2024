using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	[Export]
	public float MoveSpeed = 2;

	private Player _player;
    private Lobby _lobby;

	private Dictionary<long, Player> _peerIdToPlayer;
    private Vector2 _globalPosition;
    private Camera2D _camera;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_peerIdToPlayer = new Dictionary<long, Player>();
		_globalPosition = new Vector2(0, 0);
		_camera = GetNode<Camera2D>("%Camera2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("MoveRight"))
		{
			Rpc(MethodName.MovePlayer, Multiplayer.MultiplayerPeer.GetUniqueId(), MoveSpeed, 0);
		}

		if (Input.IsActionPressed("MoveLeft"))
		{
			Rpc(MethodName.MovePlayer, Multiplayer.MultiplayerPeer.GetUniqueId(), -MoveSpeed, 0);
		}

		if (Input.IsActionPressed("MoveUp"))
		{
			Rpc(MethodName.MovePlayer, Multiplayer.MultiplayerPeer.GetUniqueId(), 0, -MoveSpeed);
		}

		if (Input.IsActionPressed("MoveDown"))
		{
			Rpc(MethodName.MovePlayer, Multiplayer.MultiplayerPeer.GetUniqueId(), 0, MoveSpeed);
		}

		if (Input.IsActionPressed("Action"))
		{
			Rpc(MethodName.PlayerFire, Multiplayer.MultiplayerPeer.GetUniqueId());
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void MovePlayer(long playerId, float x, float y)
	{
		var player = _peerIdToPlayer[playerId];
		player.MoveTo(new Vector2(x, y));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void PlayerFire(long playerId)
	{
		var player = _peerIdToPlayer[playerId];
		player.Fire();
	}

	public void StartGame(Lobby lobby)
	{
		_lobby = lobby;
		Multiplayer.MultiplayerPeer = lobby.Multiplayer.MultiplayerPeer;
		var playerScene = GD.Load<PackedScene>("uid://c4na1fi5ewo8s");

		foreach(var key in _lobby._players.Keys)
		{
			GD.Print(key);
			var playerInfo = _lobby._players[key];
			var player = playerScene.Instantiate<Player>();
			AddChild(player);
			_peerIdToPlayer.Add(key, player);
			player.ChangeColor(playerInfo.Color);
			player.PlayerInfo = playerInfo;
			player.CurrentAssignedPlayerInfo = playerInfo;

			// Is current player
			if (key == Multiplayer.MultiplayerPeer.GetUniqueId())
			{
				_camera.Reparent(player);
			}
		}
	}
}
