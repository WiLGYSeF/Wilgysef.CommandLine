namespace Wilgysef.CommandLine.Commands;

internal class DelegateCommandFactory<T>(string name, Func<T> factory) : Command, ICommandFactory<T>
    where T : ICommand
{
    public override string Name => name;

    public T Create() => factory();

    public override void Execute(ICommandExecutionContext context)
        => throw new NotImplementedException();

    ICommand ICommandFactory.Create()
        => Create();
}
