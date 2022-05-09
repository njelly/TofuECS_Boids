using System;
using System.Threading.Tasks;
using TMPro;
using Tofunaut.Bootstrap;
using Tofunaut.Bootstrap.Interfaces;
using Tofunaut.TofuECS_Boids.Game;
using Tofunaut.TofuECS_Boids.Game.Canvas;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tofunaut.TofuECS_Boids
{
    public class InGameStateRequest
    {
        public SimulationConfig SimulationConfig;
    }
    
    public class InGameState : AppState<InGameStateRequest>
    {
        private readonly CanvasStack _canvasStack;
        private readonly IUpdateService _updateService;
        private readonly AssetReference _boidPrefabReference;
        private AsyncOperationHandle<GameObject> _boidPrefabHandle;
        private SimulationRunner _simulationRunner;

        public InGameState(CanvasStack canvasStack, IUpdateService updateService, AssetReference boidPrefabReference)
        {
            _canvasStack = canvasStack;
            _updateService = updateService;
            _boidPrefabReference = boidPrefabReference;
        }
        
        public override async Task OnEnter(InGameStateRequest request)
        {
            _boidPrefabHandle = Addressables.LoadAssetAsync<GameObject>(_boidPrefabReference);
            while (!_boidPrefabHandle.IsDone)
                await Task.Yield();

            if (_boidPrefabHandle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception("could not load boid view prefab");

            _simulationRunner = new SimulationRunner(request.SimulationConfig, _boidPrefabHandle.Result);
            _updateService.Updated += OnUpdated;

            await _canvasStack.Push<StatsCanvasViewController, StatsCanvasViewModel>(new StatsCanvasViewModel
            {
                GetBoidsCount = () => _simulationRunner.NumBoids,
            });
        }

        public override Task OnExit()
        {
            _updateService.Updated -= OnUpdated;
            _simulationRunner.Dispose();
            Addressables.Release(_boidPrefabHandle);
            
            return Task.CompletedTask;
        }

        private void OnUpdated()
        {
            _simulationRunner.DoTick();
        }
    }
}