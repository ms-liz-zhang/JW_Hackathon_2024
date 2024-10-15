using Godot;
using System;

public partial class Main : Node2D
{
    private Game _game;
    private Game _minimapGame;
    private Lobby _lobby;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_game = GetNode<Game>("%Game");
		_minimapGame = GetNode<Game>("%MinimapGame");
		_lobby = GetNode<Lobby>("%Lobby");
		_lobby.GameStart += () =>
		{
			_lobby.Visible = false;
			_game.Visible = true;
			_minimapGame.Visible = true;

			_game.StartGame(_lobby);
			_minimapGame.StartGame(_lobby);
			_minimapGame.GetNode<Camera2D>("%Camera2D").Zoom = new Vector2(.125f, .125f);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
