using System;

namespace GitHelpers.Diff
{
    public enum OperationType
    {
        Added,
        Copied,
        Deleted,
        Modified,
        Renamed,
        TypeChanged,
        Unmerged,
        Unknown,
        BrokenPairing
    }

    public abstract class Operation
    {
        internal Operation(OperationType operationType)
        {
            OperationType = operationType;
        }

        internal abstract void Accept(OperationVisitor visitor);

        public OperationType OperationType { get; private set; }
    }

    public class SingleFileOperation : Operation
    {
        internal SingleFileOperation(OperationType operationType, string fileName) : base(operationType)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }

        internal override void Accept(OperationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class TwoFileOperation : Operation
    {
        public TwoFileOperation(OperationType operationType, string fromFileName, string toFileName) : base(operationType)
        {
            FromFileName = fromFileName;
            ToFileName = toFileName;
        }

        public string FromFileName { get; private set; }
        public string ToFileName { get; private set; }

        internal override void Accept(OperationVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
