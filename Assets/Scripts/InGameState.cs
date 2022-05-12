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
            
            await _canvasStack.Push<AdjustBoidConfigCanvasViewController, AdjustBoidConfigCanvasViewModel>(
                new AdjustBoidConfigCanvasViewModel
                {
                    AlignmentChanged = v =>
                    {
                        request.SimulationConfig.Alignment = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    AlignmentRadiusChanged = v =>
                    {
                        request.SimulationConfig.AlignmentRadius = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    CohesionChanged = v =>
                    {
                        request.SimulationConfig.Cohesion = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    CohesionRadiusChanged = v =>
                    {
                        request.SimulationConfig.CohesionRadius = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    SeparationChanged = v =>
                    {
                        request.SimulationConfig.Separation = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    SeparationRadiusChanged = v =>
                    {
                        request.SimulationConfig.SeparationRadius = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    MaxSpeedChanged = v =>
                    {
                        request.SimulationConfig.MaxSpeed = v;
                        _simulationRunner.UpdateBoidConfigValues();
                    },
                    InitialAlignment = request.SimulationConfig.Alignment,
                    InitialAlignmentRadius = request.SimulationConfig.AlignmentRadius,
                    InitialCohesion = request.SimulationConfig.Cohesion,
                    InitialCohesionRadius = request.SimulationConfig.CohesionRadius,
                    InitialSeparation = request.SimulationConfig.Separation,
                    InitialSeparationRadius = request.SimulationConfig.SeparationRadius,
                    InitialMaxSpeed = request.SimulationConfig.MaxSpeed,
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