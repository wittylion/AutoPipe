using System;

namespace AutoPipe
{
    public interface ITypeFilter
    {
        bool Matches(Type type);
    }
}
