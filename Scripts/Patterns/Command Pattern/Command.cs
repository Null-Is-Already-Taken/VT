using System.Collections.Generic;

namespace VT.Patterns.CommandPattern
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> commands;
        public IReadOnlyList<ICommand> Commands => commands;

        public CompositeCommand(List<ICommand> commands)
        {
            this.commands = commands;
        }

        public CompositeCommand(IEnumerable<ICommand> commands)
        {
            this.commands = new List<ICommand>(commands);
        }

        public void Execute()
        {
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Execute();
            }
        }

        public void Undo()
        {
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                commands[i].Undo();
            }
        }
    }
}
