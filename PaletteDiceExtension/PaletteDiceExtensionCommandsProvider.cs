// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace PaletteDiceExtension;

public partial class PaletteDiceExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public PaletteDiceExtensionCommandsProvider()
    {
        DisplayName = "Random dice";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        _commands = [
            new CommandItem(new PaletteDiceExtensionPage()) { Title = DisplayName },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
