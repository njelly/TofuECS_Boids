using System;
using System.Numerics;
using Tofunaut.TofuECS;

namespace Game.ECS
{
    public class BoidSystem : ISystem
    {
        public void Initialize(Simulation s) { }

        public unsafe void Process(Simulation s)
        {
            var boidConfig = s.GetSingletonComponent<BoidConfig>();
            var boidBuffer = s.Buffer<Boid>();
            var i = 0;
            var j = 0;
            var alignmentRadiusSquared = boidConfig.AlignmentRadius * boidConfig.AlignmentRadius;
            var cohesionRadiusSquared = boidConfig.CohesionRadius * boidConfig.CohesionRadius;
            var separationRadiusSquared = boidConfig.SeparationRadius * boidConfig.SeparationRadius;
            while (boidBuffer.NextUnsafe(ref i, out var entityId, out var boid))
            {
                boid->Force = Vector2.Zero;
                
                var alignment = Vector2.Zero;
                var numAlignment = 0;
                var cohesion = Vector2.Zero;
                var numCohesion = 0;
                var separation = Vector2.Zero;
                var numSeparation = 0;

                while (boidBuffer.NextUnsafe(ref j, out var otherEntityId, out var otherBoid))
                {
                    if (entityId == otherEntityId)
                        continue;

                    var distSquared = (boid->Position - otherBoid->Position).LengthSquared();
                    
                    if (distSquared < alignmentRadiusSquared)
                    {
                        alignment += otherBoid->Velocity;
                        numAlignment++;
                    }

                    if (distSquared < cohesionRadiusSquared)
                    {
                        cohesion += otherBoid->Position;
                        numCohesion++;
                    }

                    if (distSquared < separationRadiusSquared)
                    {
                        separation += otherBoid->Position;
                        numSeparation++;
                    }
                }

                if (numAlignment > 0)
                {
                    alignment /= numAlignment;
                    boid->Force += (alignment - boid->Velocity) * boidConfig.Alignment;
                }

                if (numCohesion > 0)
                {
                    cohesion /= numCohesion;
                    boid->Force += (cohesion - boid->Position) * boidConfig.Cohesion;
                }

                if (numSeparation > 0)
                {
                    separation /= numSeparation;
                    boid->Force += (boid->Position - separation) * boidConfig.Separation;
                }
            }

            i = 0;
            while (boidBuffer.NextUnsafe(ref i, out _, out var boid))
            {
                boid->Velocity += boid->Force * boidConfig.DeltaTime;
                boid->Position += boid->Velocity * boidConfig.DeltaTime;
                
                // loop around world
                if (boid->Position.X < -boidConfig.WorldExtents.X)
                    boid->Position.X = boidConfig.WorldExtents.X;
                if (boid->Position.X > boidConfig.WorldExtents.X)
                    boid->Position.X = -boidConfig.WorldExtents.X;
                if (boid->Position.Y < -boidConfig.WorldExtents.Y)
                    boid->Position.Y = boidConfig.WorldExtents.Y;
                if (boid->Position.Y > boidConfig.WorldExtents.Y)
                    boid->Position.Y = -boidConfig.WorldExtents.Y;
            }
        }
    }
}