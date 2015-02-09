using Engine.Player;

namespace Engine.Commands
{
    public class Command
    {
        public virtual void Execute(GamePlayer gp)
        {
        }
    }

    public class BuildCommand : Command
    {
        public override void Execute(GamePlayer gp)
        {
            gp.Build();
        }
    }  

    public class TiltCommand : Command
    {
        public override void Execute(GamePlayer gp)
        {
            gp.TiltStructure();
        }
    }
}
