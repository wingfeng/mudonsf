namespace GameServer
{
    public interface ICommand
    {
        object Invoker { get; }
     

        void Execute();
    }
}