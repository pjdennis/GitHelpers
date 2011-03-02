using System;

namespace GitHelpers.Diff
{
    public interface OperationVisitor
    {
        void Visit(SingleFileOperation operation);
        void Visit(TwoFileOperation operation);
    }
}
