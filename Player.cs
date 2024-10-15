using Godot;
using System;
using System.Formats.Asn1;

public partial class Player : RigidBody2D
{
	[Export]
	public PackedScene ProjectileScene;

	[Signal]
	public delegate void PlayerMovedEventHandler(Vector2 globalPosition);

	public PlayerInfo PlayerInfo;

	// The player info that is checked when dealing with projectiles
	public PlayerInfo CurrentAssignedPlayerInfo;
    private RigidBody2D _characterBody;
    private Sprite2D _sprite;
    private Node2D _firingPoint;
    private Timer _timer;
    private bool _canFire;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_characterBody = this;
		_sprite = GetNode<Sprite2D>("%Sprite2D");
		_firingPoint = GetNode<Node2D>("%FiringPoint");
		_timer = GetNode<Timer>("%Timer");
		_canFire = true;
		_timer.Timeout += () => {
			_canFire = true;
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void MoveTo(Vector2 motion)
	{
		LookAt(_characterBody.GlobalPosition + motion);
		MoveAndCollide(motion);
		EmitSignal(SignalName.PlayerMoved, GlobalPosition);
	}

	public void Fire()
	{
		if (!_canFire)
		{
			return;
		}
		_canFire = false;
		
		var projectile = ProjectileScene.Instantiate<Projectile>();
		GetParent().AddChild(projectile);
		projectile.Player = this;
		projectile.CurrentAssignedPlayerInfo = CurrentAssignedPlayerInfo;

		var unitVector = (_firingPoint.GlobalPosition - _characterBody.GlobalPosition).Normalized();

		LookAt(_firingPoint.GlobalPosition + unitVector * 5);
		AddCollisionExceptionWith(projectile._rigidBody);
		projectile.GlobalPosition = _firingPoint.GlobalPosition;
		projectile.LookAt(_firingPoint.GlobalPosition + unitVector * 5);
		projectile.ApplyImpulse(unitVector * 1000f);
		projectile.ChangeColor(CurrentAssignedPlayerInfo.Color);
		_timer.Start();
	}

	public void ChangeColor(Color color)
	{
		_sprite.Modulate = color;
	}

	private void _on_body_entered(Node body)
	{
		var projectile = body.Owner as Projectile;

		if (projectile == null)
		{
			return;
		}

		// Delete project if hitting same side
		if (projectile.CurrentAssignedPlayerInfo == PlayerInfo)
		{
			projectile.QueueFree();

			return;
		}

		PlayerInfo = projectile.CurrentAssignedPlayerInfo;
		projectile.QueueFree();
		ChangeColor(PlayerInfo.Color);
	}
}
