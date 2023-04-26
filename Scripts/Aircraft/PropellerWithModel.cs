using Godot;
using System;

namespace Aircraft
{
    [Tool]
    public class PropellerWithModel : Physics.Forcers.Propeller
    {
        // A Propeller with a model that sizes itself to the dimensions given in the inspector.
        // The models should be of radius 1 when not scaled.
        // The resting pitch doesn't really matter as it can be adjusted with PitchScaleMultiplier.

        [Export] public float PitchScaleMultiplier { get; set; } = 1;

        private Spatial blurShape;
        private Spatial actualModel;

        private float minBlurRpm = 2000; // min rpm at which the blur model displays

        public override void _Ready()
        {
            blurShape = GetNode<Spatial>("BlurShape");
            actualModel = GetNode<Spatial>("SlowModel");
            base._Ready();
        }

        public override void _Process(float delta)
        {
            if (Engine.EditorHint && Engine.GetFramesDrawn() % 10 == 0) UpdateModelSizes();
            if (!Engine.EditorHint)
            {
                base._Process(delta);
                UpdateModelVisiblities();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (! Engine.EditorHint) base._PhysicsProcess(delta);
        }

        private void UpdateModelSizes()
        {
            blurShape.Scale = new Vector3(RadiusMetres * (Clockwise ? 1 : -1), RadiusMetres, PitchMetres * PitchScaleMultiplier);
            actualModel.Scale = blurShape.Scale;
        }

        private void UpdateModelVisiblities()
        {
            blurShape.Visible = Mathf.Abs(Rpm) >= minBlurRpm;
        }
    }
}