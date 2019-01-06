using System;
using Rhisis.Core.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhisis.Core.Resources.Include
{
    public sealed class IncludeFile : IDisposable
    {
        private readonly FileTokenScanner _scanner;
        private readonly ICollection<IStatement> _statements;

        public IReadOnlyCollection<IStatement> Statements => this._statements as IReadOnlyCollection<IStatement>;

        public IncludeFile(string filePath, string regexPattern = @"([(){}=,;\n\r\t])")
        {
            this._scanner = new FileTokenScanner(filePath, regexPattern);
            this._statements = new List<IStatement>();

            this.Read();
        }

        private void Read()
        {
            this._scanner.Read();

            string token = null;
            while ((token = this._scanner.GetToken()) != null)
            {
                switch (token)
                {
                    case "{":
                        this._statements.Add(this.ParseBlock());
                        break;
                    case "(":
                        this._statements.Add(this.ParseInstruction());
                        break;
                    case "=":
                        this._statements.Add(this.ParseVariable());
                        break;
                }
            }
        }

        private Block ParseBlock()
        {
            string token = null;
            var block = new Block()
            {
                Name = this._scanner.GetPreviousToken()
            };
            
            while ((token = this._scanner.GetToken()) != "}")
            {
                if (token == null)
                    break;
                switch (token)
                {
                    case "{":
                        block.AddStatement(this.ParseBlock());
                        break;
                    case "(":
                        block.AddStatement(this.ParseInstruction());
                        break;
                    case "=":
                        block.AddStatement(this.ParseVariable());
                        break;
                    default:
                        block.AddUnknownStatement(token);
                        break;
                }
            }

            return block;
        }

        private Instruction ParseInstruction()
        {
            string parameter = null;
            var instruction = new Instruction
            {
                Name = this._scanner.GetPreviousToken()
            };

            while ((parameter = this._scanner.GetToken()) != ")")
                instruction.AddParameter(parameter);

            this._scanner.GetToken();

            return instruction;
        }

        private Variable ParseVariable()
        {
            string variableName = this._scanner.GetPreviousToken();
            string variableValue = this._scanner.GetToken();
            string endDelimiter = this._scanner.GetToken();

            if (endDelimiter != ";")
                throw new InvalidDataException("Invalid variable format. Missing ';' for variable " + variableName);

            return new Variable(variableName, variableValue);
        }

        public Block GetBlock(string blockName) => this._statements.FirstOrDefault(x => x.Type == StatementType.Block && x.Name.Equals(blockName)) as Block;

        public void Dispose()
        {
            foreach (IStatement statement in this._statements)
            {
                statement.Dispose();
            }

            this._statements.Clear();
            this._scanner.Dispose();
        }
    }
}
