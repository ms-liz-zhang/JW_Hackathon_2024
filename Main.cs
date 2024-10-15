using Godot;
using System;

public partial class Main : Node2D
{
    private Game _game;
    private Lobby _lobby;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_game = GetNode<Game>("%Game");
		_lobby = GetNode<Lobby>("%Lobby");
		_lobby.GameStart += () =>
		{
			_lobby.Visible = false;
			_game.Visible = true;

			_game.StartGame(_lobby);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
