using Godot;

public partial class NPC : CharacterBody2D
{
    private Area2D _playerDetector;
    private bool _playerInNPCArea = false;
    [Export] public string DialogScenePath = "res://scenes/core/dialog.tscn";

    public override void _Ready() {
        _playerDetector = GetNode<Area2D>("PlayerDetector");

        _playerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
        _playerDetector.BodyExited += OnPlayerDetectorBodyExited;
    }

    public override void _Process(double delta) {
        if (_playerInNPCArea && Input.IsActionJustPressed("ui_interact")) {
            GetTree().ChangeSceneToFile(DialogScenePath);
        }
    }

    private void OnPlayerDetectorBodyEntered(Node body) {
        if (body is Player) {
            _playerInNPCArea = true;
        }
    }

    private void OnPlayerDetectorBodyExited(Node body) {
        if (body is Player) {
            _playerInNPCArea = false;
        }
    }
}