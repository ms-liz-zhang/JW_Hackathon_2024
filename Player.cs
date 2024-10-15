using Godot;
using System;

public partial class Player : Node2D
{
    private RigidBody2D _characterBody;
    private Sprite2D _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_characterBody = GetNode<RigidBody2D>("%CharacterBody2D");
		_sprite = GetNode<Sprite2D>("%Sprite2D");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Move(Vector2 offset)
	{
		// _characterBody.MoveAndCollide(offset);
		_characterBody.GlobalPosition += offset;
	}

	public void ChangeColor(Color color)
	{
		_sprite.Modulate = color;
	}
}
