using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.DeclaredElements;

namespace AsyncConverter.Helpers
{
    public interface IAsyncReplacer
    {
        void ReplaceToAsync([NotNull] IMethod methodDeclaredElement);

        void ReplaceToAsync(ILocalFunction method);
    }
}