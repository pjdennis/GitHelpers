using System;
using System.IO;
using System.Text;

namespace GitHelpers.Diff
{
    public class DiffParser
    {
        private readonly OperationVisitor _visitor;
        private State _state;
        private OperationType _operationType;
        private string _firstFileName;

        private enum State
        {
            AwaitingType,
            AwaitingFileName,
            AwaitingFirstOfTwoFileNames,
            AwaitingSecondOfTwoFileNames,
        }

        public static void Parse(Stream input, OperationVisitor visitor)
        {
            new DiffParser(input, visitor);
        }

        private DiffParser(Stream input, OperationVisitor visitor)
        {
            _visitor = visitor;
            var token = new StringBuilder();
            int b;
            while ((b = input.ReadByte()) != -1)
            {
                if (b == 0)
                {
                    HandleToken(token.ToString());
                    token.Length = 0;
                    continue;
                }
                token.Append((char) b);
            }
        }

        private void HandleToken(string token)
        {
            switch (_state)
            {
                case State.AwaitingType:
                    HandleType(token);
                    break;
                case State.AwaitingFileName:
                    new SingleFileOperation(_operationType, token).Accept(_visitor);
                    _state = State.AwaitingType;
                    break;
                case State.AwaitingFirstOfTwoFileNames:
                    _firstFileName = token;
                    _state = State.AwaitingSecondOfTwoFileNames;
                    break;
                case State.AwaitingSecondOfTwoFileNames:
                    new TwoFileOperation(_operationType, _firstFileName, token).Accept(_visitor);
                    _state = State.AwaitingType;
                    break;
                default:
                    throw new ApplicationException("Unhandled State");
            }
        }

        private void HandleType(string typeToken)
        {
            switch (typeToken.Substring(0, 1))
            {
                case "A":
                    _operationType = OperationType.Added;
                    _state = State.AwaitingFileName;
                    break;
                case "C":
                    _operationType = OperationType.Copied;
                    _state = State.AwaitingFirstOfTwoFileNames;
                    break;
                case "D":
                    _operationType = OperationType.Deleted;
                    _state = State.AwaitingFileName;
                    break;
                case "M":
                    _operationType = OperationType.Modified;
                    _state = State.AwaitingFileName;
                    break;
                case "R":
                    _operationType = OperationType.Renamed;
                    _state = State.AwaitingFirstOfTwoFileNames;
                    break;
                case "T":
                    _operationType = OperationType.TypeChanged;
                    _state = State.AwaitingFileName;
                    break;
                case "U":
                    _operationType = OperationType.Unmerged;
                    _state = State.AwaitingFileName;
                    break;
                case "X":
                    _operationType = OperationType.Unknown;
                    _state = State.AwaitingFileName;
                    break;
                case "B":
                    _operationType = OperationType.BrokenPairing;
                    _state = State.AwaitingFileName;
                    break;
                default:
                    throw new ApplicationException(string.Format("Unknown change type '{0}'", typeToken));
            }
        }
    }
}
