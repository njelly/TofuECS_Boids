using Tofunaut.Bootstrap;
using Tofunaut.TofuECS_Boids.Game;
using Tofunaut.TofuECS_Boids.Game.Canvas;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tofunaut.TofuECS_Boids
{
    public class AppContext : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private MonoBehaviourUpdateService _monoBehaviourUpdateService;
        [SerializeField] private SimulationConfig _simulationConfig;
        [SerializeField] private AssetReference _boidViewPrefabReference;
        [SerializeField] private AssetReference _statsCanvasViewControllerReference;
        [SerializeField] private AssetReference _adjustBoidConfigCanvasViewControllerReference;

        private CanvasStack _canvasStack;
        private AppStateMachine _appStateMachine;

        private async void Start()
        {
            _canvasStack = new CanvasStack(_canvas);
            _canvasStack.RegisterViewController<StatsCanvasViewController, StatsCanvasViewModel>(
                _statsCanvasViewControllerReference);
            _canvasStack.RegisterViewController<AdjustBoidConfigCanvasViewController, AdjustBoidConfigCanvasViewModel>(
                _adjustBoidConfigCanvasViewControllerReference);

            _appStateMachine = new AppStateMachine();
            _appStateMachine.RegisterState<InGameState, InGameStateRequest>(new InGameState(_canvasStack,
                _monoBehaviourUpdateService, _boidViewPrefabReference));

            await _appStateMachine.EnterState(new InGameStateRequest
            {
                SimulationConfig = _simulationConfig,
            });
        }
    }
}