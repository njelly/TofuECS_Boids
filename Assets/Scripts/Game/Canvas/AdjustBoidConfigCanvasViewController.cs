using System;
using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.TofuECS_Boids.Game.Canvas
{
    public class AdjustBoidConfigCanvasViewModel
    {
        public float InitialAlignment;
        public float InitialAlignmentRadius;
        public float InitialCohesion;
        public float InitialCohesionRadius;
        public float InitialSeparation;
        public float InitialSeparationRadius;
        public float InitialMaxSpeed;
        public Action<float> AlignmentChanged;
        public Action<float> AlignmentRadiusChanged;
        public Action<float> CohesionChanged;
        public Action<float> CohesionRadiusChanged;
        public Action<float> SeparationChanged;
        public Action<float> SeparationRadiusChanged;
        public Action<float> MaxSpeedChanged;
    }

    public class AdjustBoidConfigCanvasViewController : CanvasViewController<AdjustBoidConfigCanvasViewModel>
    {
        [SerializeField] private Slider _alignmentSlider;
        [SerializeField] private Text _alignmentLabel;
        [SerializeField] private Slider _alignmentRadiusSlider;
        [SerializeField] private Text _alignmentRadiusLabel;
        [SerializeField] private Slider _cohesionSlider;
        [SerializeField] private Text _cohesionLabel;
        [SerializeField] private Slider _cohesionRadiusSlider;
        [SerializeField] private Text _cohesionRadiusLabel;
        [SerializeField] private Slider _separationSlider;
        [SerializeField] private Text _separationLabel;
        [SerializeField] private Slider _separationRadiusSlider;
        [SerializeField] private Text _separationRadiusLabel;
        [SerializeField] private Slider _maxSpeedSlider;
        [SerializeField] private Text _maxSpeedLabel;
        
        public override Task OnPushedToStack(AdjustBoidConfigCanvasViewModel model)
        {
            void setUpSlider(Slider slider, Text label, float initialValue, Action<float> callback)
            {
                slider.onValueChanged.RemoveAllListeners();
                slider.SetValueWithoutNotify(0.5f);
                label.text = initialValue.ToString("F2");
                slider.onValueChanged.AddListener(v =>
                {
                    var newValue = v * 2f * initialValue;
                    label.text = newValue.ToString("F2");
                    callback?.Invoke(newValue);
                });
            }
            
            setUpSlider(_alignmentSlider, _alignmentLabel, model.InitialAlignment, model.AlignmentChanged);
            setUpSlider(_alignmentRadiusSlider, _alignmentRadiusLabel, model.InitialAlignmentRadius, model.AlignmentRadiusChanged);
            setUpSlider(_cohesionSlider, _cohesionLabel, model.InitialCohesion, model.CohesionChanged);
            setUpSlider(_cohesionRadiusSlider, _cohesionRadiusLabel, model.InitialCohesionRadius, model.CohesionRadiusChanged);
            setUpSlider(_separationSlider, _separationLabel, model.InitialSeparation, model.SeparationChanged);
            setUpSlider(_separationRadiusSlider, _separationRadiusLabel, model.InitialSeparationRadius, model.SeparationRadiusChanged);
            setUpSlider(_maxSpeedSlider, _maxSpeedLabel, model.InitialMaxSpeed, model.MaxSpeedChanged);

            return Task.CompletedTask;
        }
    }
}