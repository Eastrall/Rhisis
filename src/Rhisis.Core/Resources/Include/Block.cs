using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Core.Resources.Include
{
    public class Block : IStatement, IDisposable
    {
        private readonly ICollection<IStatement> _statements;
        private readonly ICollection<string> _unknownStatements;

        public string Name { get; set; }

        public StatementType Type => StatementType.Block;

        public IReadOnlyCollection<IStatement> Statements => this._statements as IReadOnlyCollection<IStatement>;

        public IReadOnlyCollection<string> UnknownStatements => this._unknownStatements as IReadOnlyCollection<string>;

        public Block this[string blockName] => this.GetBlockByName(blockName);

        public Block()
            : this(string.Empty)
        {
        }

        public Block(string name)
        {
            this.Name = name;
            this._statements = new List<IStatement>();
            this._unknownStatements = new List<string>();
        }

        public Block GetBlockByName(string name) => this.GetStatement<Block>(name, StatementType.Block);

        public Instruction GetInstruction(string name) => this.GetStatement<Instruction>(name, StatementType.Instruction);

        public IEnumerable<Instruction> GetInstructions(string name) => this.GetStatements<Instruction>(name, StatementType.Instruction);

        public Variable GetVariable(string name) => this.GetStatement<Variable>(name, StatementType.Variable);

        private T GetStatement<T>(string name, StatementType type) where T : IStatement
            => (T)this._statements.FirstOrDefault(x => x.Name == name && x.Type == type);

        private IEnumerable<T> GetStatements<T>(string name, StatementType type) where T : IStatement
            => this._statements.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Type == type).Cast<T>();

        internal void AddStatement(IStatement statement) => this._statements.Add(statement);

        internal void AddUnknownStatement(string statement) => this._unknownStatements.Add(statement);

        public void Dispose()
        {
            if (this._statements.Any())
                this._statements.Clear();
        }

        public override string ToString() => $"{this.Name}";
    }
}
