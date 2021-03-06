﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Vim;
using Vim.Extensions;
using Vim.UI.Wpf;

namespace VsVim.Implementation.Misc
{
    [Export(typeof(IKeyProcessorProvider))]
    [Order(Before = Constants.VisualStudioKeyProcessorName)]
    [Name(Constants.VsKeyProcessorName)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(Vim.Constants.ContentType)]
    internal sealed class VsKeyProcessorProvider : IKeyProcessorProvider
    {
        private readonly IVimBufferCoordinatorFactory _bufferCoordinatorFactory;
        private readonly IVsAdapter _adapter;
        private readonly IVim _vim;
        private readonly IKeyUtil _keyUtil;

        [ImportingConstructor]
        internal VsKeyProcessorProvider(IVim vim, IVsAdapter adapter, IVimBufferCoordinatorFactory bufferCoordinatorFactory, IKeyUtil keyUtil)
        {
            _vim = vim;
            _adapter = adapter;
            _bufferCoordinatorFactory = bufferCoordinatorFactory;
            _keyUtil = keyUtil;
        }

        KeyProcessor IKeyProcessorProvider.GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            var opt = _vim.GetOrCreateVimBufferForHost(wpfTextView);
            if (opt.IsNone())
            {
                return null;
            }

            var vimBuffer = opt.Value;
            var vimBufferCoordinator = _bufferCoordinatorFactory.GetVimBufferCoordinator(vimBuffer);
            return new VsKeyProcessor(_adapter, vimBufferCoordinator, _keyUtil);
        }
    }
}
