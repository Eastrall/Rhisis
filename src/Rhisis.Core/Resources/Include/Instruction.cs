using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.Core.Resources.Include
{
    public class Instruction : IStatement, IDisposable
    {
        private readonly ICollection<object> _parameters;

        /// <summary>
        /// Gets the instruction's name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the instruction's parameters.
        /// </summary>
        public IReadOnlyCollection<object> Parameters => this._parameters as IReadOnlyCollection<object>;

        /// <summary>
        /// Gets the statement type.
        /// </summary>
        public StatementType Type => StatementType.Instruction;

        /// <summary>
        /// Creates a new <see cref="Instruction"/> instance without any parameters.
        /// </summary>
        public Instruction()
            : this(string.Empty, new List<object>())
        {
        }

        /// <summary>
        /// Creates a new <see cref="Instruction"/> instance with a name and parameters.
        /// </summary>
        /// <param name="name">Instruction name</param>
        /// <param name="parameters">Parameters</param>
        public Instruction(string name, ICollection<object> parameters)
        {
            this.Name = name;
            this._parameters = parameters;
        }

        /// <summary>
        /// Add a parameter to the instruction.
        /// </summary>
        /// <param name="parameter">Parameter</param>
        internal void AddParameter(object parameter)
        {
            if (parameter.ToString() != ",")
                this._parameters.Add(parameter);
        }
        
        /// <inheritdoc />
        public void Dispose()
        {
            if (this._parameters.Any())
                this._parameters.Clear();
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Name}({string.Join(", ", this.Parameters)})";
    }
}
