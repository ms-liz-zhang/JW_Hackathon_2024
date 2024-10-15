using Godot;
using System;

public partial class Lobby : Control
{
	public event EventHandler<MultiplayerPeer> MultiplayerPeerCreated;
	
    public static Lobby Instance { get; private set; }

    // These signals can be connected to by a UI lobby scene or the game scene.
    [Signal]
    public delegate void PlayerConnectedEventHandler(int peerId, PlayerInfo playerInfo);
    [Signal]
    public delegate void PlayerDisconnectedEventHandler(int peerId);
    [Signal]
    public delegate void ServerDisconnectedEventHandler();

	[Signal]
	public delegate void GameStartEventHandler();

    private const int Port = 8080;
    private const string DefaultServerIP = "127.0.0.1"; // IPv4 localhost
    private const int MaxConnections = 20;

    // This will contain player info for every player,
    // with the keys being each player's unique IDs.
    public Godot.Collections.Dictionary<long, PlayerInfo> _players = new Godot.Collections.Dictionary<long, PlayerInfo>();

    // This is the local player info. This should be modified locally
    // before the connection is made. It will be passed to every other peer.
    // For example, the value of "name" can be set to something the player
    // entered in a UI scene.
    public PlayerInfo _playerInfo;

    private int _playersLoaded = 0;
    private bool _joinAsClient;

    public override void _Ready()
    {
		_playerInfo = new PlayerInfo
		{
			Color = new Color(GD.Randf(), GD.Randf(), GD.Randf())
		};
        Instance = this;
        Multiplayer.PeerConnected += OnPlayerConnected;
        Multiplayer.PeerDisconnected += OnPlayerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectOk;
        Multiplayer.ConnectionFailed += OnConnectionFail;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    private Error JoinGame(string address = "")
    {
        if (string.IsNullOrEmpty(address))
        {
            address = DefaultServerIP;
        }

        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, 6655);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;
        return Error.Ok;
    }

    private Error CreateGame()
    {
        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(Port, MaxConnections);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;
        _players[1] = _playerInfo;
        EmitSignal(SignalName.PlayerConnected, 1, _playerInfo);
        return Error.Ok;
    }

    private void RemoveMultiplayerPeer()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    // When the server decides to start the game from a UI scene,
    // do Rpc(Lobby.MethodName.LoadGame, filePath);
    [Rpc(CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void LoadGame(string gameScenePath)
    {
        GetTree().ChangeSceneToFile(gameScenePath);
    }

    // Every peer will call this when they have loaded the game scene.
    // [Rpc(MultiplayerApi.RpcMode.AnyPeer,CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    // private void PlayerLoaded()
    // {
    //     if (Multiplayer.IsServer())
    //     {
    //         _playersLoaded += 1;
    //         if (_playersLoaded == _players.Count)
    //         {
    //             GetNode<Game>("/root/Game").StartGame();
    //             _playersLoaded = 0;
    //         }
    //     }
    // }

    // When a peer connects, send them my player info.
    // This allows transfer of all desired data for each player, not only the unique ID.
    private void OnPlayerConnected(long id)
    {
		GD.Print($"Player connected {id}");
        RpcId(id, MethodName.RegisterPlayer, _playerInfo.Serialize());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RegisterPlayer(string newPlayerInfo)
    {
        int newPlayerId = Multiplayer.GetRemoteSenderId();
		GD.Print($"Register {newPlayerId}");
        _players[newPlayerId] = PlayerInfo.Deserialize(newPlayerInfo);
        EmitSignal(SignalName.PlayerConnected, newPlayerId, newPlayerInfo);
    }

    private void OnPlayerDisconnected(long id)
    {
        _players.Remove(id);
        EmitSignal(SignalName.PlayerDisconnected, id);
    }

    private void OnConnectOk()
    {
        int peerId = Multiplayer.GetUniqueId();
        _players[peerId] = _playerInfo;
        EmitSignal(SignalName.PlayerConnected, peerId, _playerInfo);
    }

    private void OnConnectionFail()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        _players.Clear();
        EmitSignal(SignalName.ServerDisconnected);
    }

	public void _on_client_button_pressed()
	{
		JoinGame("ffw1v6j.localto.net");
	}

	public void _on_server_button_pressed()
	{
		CreateGame();
	}

	public void _on_start_game_button_pressed()
	{
		EmitSignal(SignalName.GameStart);
	}
}
