using System;
using System.Threading.Tasks;
using Tofunaut.Bootstrap;
using UnityEngine;
using UnityEngine.UI;

namespace Tofunaut.TofuECS_Boids.Game.Canvas
{
    public class StatsCanvasViewModel
    {
        public Func<int> GetBoidsCount;
    }
    
    public class StatsCanvasViewController : CanvasViewController<StatsCanvasViewModel>
    {
        [SerializeField] private Text _boidsCountLabel;
        [SerializeField] private Text _fpsLabel;

        private Func<int> _getBoidsCount;
        private float _fpsTimer;
        private int _fpsCount;
        
        public override Task OnPushedToStack(StatsCanvasViewModel model)
        {
            _getBoidsCount = model.GetBoidsCount;

            _fpsTimer = 0f;
            _fpsCount = 0;
            
            return Task.CompletedTask;
        }

        private void Update()
        {
            UpdateBoidsCountLabel();
            UpdateFPSLabel();
        }

        private void UpdateBoidsCountLabel()
        {
            if (_getBoidsCount == null)
                return;
            
            _boidsCountLabel.text = $"Boids: {_getBoidsCount()}";
        }

        private void UpdateFPSLabel()
        {
            _fpsTimer += Time.deltaTime;
            _fpsCount++;
            
            if (_fpsTimer < 1f)
                return;

            _fpsLabel.text = $"FPS: {_fpsCount}";
            _fpsTimer = 0f;
            _fpsCount = 0;
        }
    }
}