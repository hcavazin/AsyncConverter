﻿using System;
using AsyncConverter.Helpers;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AsyncConverter.ContextActions
{
    [ContextAction(Group = "C#", Name = "ConvertToAsync", Description = "Convert method to async and replace all inner call to async version if exist.")]
    public class MethodToAsyncConverter : ContextActionBase
    {
        private IAsyncReplacer asyncReplacer;

        private ICSharpContextActionDataProvider Provider { get; }

        public MethodToAsyncConverter(ICSharpContextActionDataProvider provider)
        {
            Provider = provider;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            asyncReplacer = solution.GetComponent<IAsyncReplacer>();

            var method = GetMethodFromCarretPosition();
            var localFunction = GetLocalFunctionFromCarretPosition();

            var methodDeclaredElement = method?.DeclaredElement;
            var localFunctionDeclaredElement = localFunction?.DeclaredElement;

            if (methodDeclaredElement != null)
            {
                asyncReplacer.ReplaceToAsync(methodDeclaredElement);
            }
            else if (localFunctionDeclaredElement != null)
            {
                asyncReplacer.ReplaceToAsync(localFunctionDeclaredElement);
            }

            return null;
        }

        public override string Text { get; } = "Convert method to async and replace all inner call to async version if exist.";

        public override bool IsAvailable(IUserDataHolder cache)
        {
            var method = GetMethodFromCarretPosition();
            var localFunction = GetLocalFunctionFromCarretPosition();

            var returnType = method?.DeclaredElement?.ReturnType
                ?? localFunction?.DeclaredElement.ReturnType;

            return returnType != null && !(returnType.IsTask() || returnType.IsGenericTask());
        }

        [CanBeNull]
        private IMethodDeclaration GetMethodFromCarretPosition()
        {
            var identifier = Provider.TokenAfterCaret as ICSharpIdentifier;
            identifier = identifier ?? Provider.TokenBeforeCaret as ICSharpIdentifier;
            return identifier?.Parent as IMethodDeclaration;
        }

        [CanBeNull]
        private ILocalFunctionDeclaration GetLocalFunctionFromCarretPosition()
        {
            var identifier = Provider.TokenAfterCaret as ICSharpIdentifier;
            identifier = identifier ?? Provider.TokenBeforeCaret as ICSharpIdentifier;
            return identifier?.Parent as ILocalFunctionDeclaration;
        }
    }
}