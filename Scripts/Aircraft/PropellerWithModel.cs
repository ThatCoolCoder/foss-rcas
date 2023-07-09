using Godot;
using System;

namespace Aircraft
{
    [Tool]
    public partial class PropellerWithModel : Physics.Forcers.Propeller
    {
        // A Propeller with a model that sizes itself to the dimensions given in the inspector.
        // The models should be of radius 1 when not scaled.
        // The resting pitch doesn't really matter as it can be adjusted with PitchScaleMultiplier.

        [Export] public float ModelHeightMultiplier { get; set; } = 1;
        [Export] public float ColliderHeightMultiplier { get; set; } = 1;

        private Node3D blurShape;
        private Node3D actualModel;
        private CollisionShape3D collider;

        private float minBlurRpm = 2000; // min rpm at which the blur model displays
        private float collidingBodies = 0;

        public override void _Ready()
        {
            blurShape = GetNode<Node3D>("BlurShape");
            actualModel = GetNode<Node3D>("SlowModel");
            collider = GetNode<CollisionShape3D>("Area3D/CollisionShape3D");
            UpdateModelSizes();
            base._Ready();
        }

        public override void _Process(double delta)
        {
            if (Engine.IsEditorHint() && Engine.GetFramesDrawn() % 10 == 0) UpdateModelSizes();
            if (!Engine.IsEditorHint())
            {
                base._Process(delta);
                UpdateModelVisiblities();
                if (collidingBodies > 0) HitObject();
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (!Engine.IsEditorHint()) base._PhysicsProcess(delta);
        }

        private void UpdateModelSizes()
        {
            blurShape.Scale = new Vector3(RadiusMetres * (Clockwise ? 1 : -1), RadiusMetres, PitchMetres * ModelHeightMultiplier);
            actualModel.Scale = blurShape.Scale;
            var s = collider.Shape as CylinderShape3D;
            s.Radius = RadiusMetres;
            s.Height = PitchMetres * ColliderHeightMultiplier;
        }

        private void UpdateModelVisiblities()
        {
            blurShape.Visible = Mathf.Abs(Rpm) * Engine.TimeScale >= minBlurRpm;
        }

        private void _on_Area_body_entered(PhysicsBody3D _body)
        {
            // todo: perhaps this colliding stuff should be in the base propeller class,
            // otherwise we can rename class to spatialpropeller or something
            collidingBodies++;
        }

        private void _on_Area_body_exited(PhysicsBody3D _body)
        {
            collidingBodies--;
        }
    }
}