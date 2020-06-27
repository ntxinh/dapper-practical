namespace App.Application.Interfaces
{
    public interface IUnitOfWork
    {
        ITaskRepository Tasks { get; }
    }
}
