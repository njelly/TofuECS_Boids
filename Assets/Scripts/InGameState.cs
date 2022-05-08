using System.Threading.Tasks;
using Tofunaut.Bootstrap;

namespace Tofunaut.TofuECS_Boids
{
    public class InGameStateRequest
    {
        
    }
    
    public class InGameState : AppState<InGameStateRequest>
    {
        public override Task OnEnter(InGameStateRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}