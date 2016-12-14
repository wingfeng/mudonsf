namespace GameServer
{
    public interface ICommand
    {
        PlayerProxy Invoker { get; }
     

        void Execute();
    }
}