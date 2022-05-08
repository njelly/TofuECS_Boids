using Tofunaut.Bootstrap;
using UnityEngine;

namespace Tofunaut.TofuECS_Boids
{
    public class AppContext : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        private CanvasStack _canvasStack;
        private AppStateMachine _appStateMachine;

        private async void Start()
        {
            _canvasStack = new CanvasStack(_canvas);

            _appStateMachine = new AppStateMachine();
            _appStateMachine.RegisterState<InGameState, InGameStateRequest>(new InGameState());

            await _appStateMachine.EnterState(new InGameStateRequest());
        }
    }
}