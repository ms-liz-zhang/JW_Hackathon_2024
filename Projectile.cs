using Godot;
using System;

public partial class Projectile : Node2D
{
	public Player Player;
	public PlayerInfo CurrentAssignedPlayerInfo;
    public RigidBody2D _rigidBody;
    private Sprite2D _sprite;
    private Vector2 _impulse;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_rigidBody = GetNode<RigidBody2D>("%RigidBody2D");
		_sprite = GetNode<Sprite2D>("%Sprite2D");	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// _rigidBody.GlobalPosition += _impulse * (float)delta;
	}

	public void ApplyImpulse(Vector2 impulse)
	{
		_rigidBody.ApplyImpulse(impulse);
	}

	public void ChangeColor(Color color)
	{
		_sprite.Modulate = color;
	}

	public void _on_rigid_body_2d_body_entered(Node body)
	{
		var wall = body as Wall;

		if (wall == null)
		{
			return;
		}

		QueueFree();
	}
}
