// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using PaletteDiceExtension.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Gaming.XboxLive.Storage;

namespace PaletteDiceExtension;

internal sealed partial class PaletteDiceExtensionPage : DynamicListPage
{
    public override IconInfo Icon => new IconInfo("🎲");

    private List<DiceResult> diceResults = [];
    private DiceResult? lastResult = null;
    public PaletteDiceExtensionPage()
    {
        Title = "Random dice";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        ShowMessageCommand showMessageCommand = new();
        if (lastResult == null)
        {
            return [
            new ListItem(new NoOpCommand()) { Icon= new IconInfo(">"), Title = "Enter a valid D&D style dice (ex: 1D6 rolls a singular 6-sided die)"},
                DiceCommand(4),
                DiceCommand(6),
                DiceCommand(8),
                DiceCommand(12),
                DiceCommand(20),
                DiceCommand(100, "💯"),
        ];
        }
        else
        {
            return [
                new ListItem(new AnonymousCommand(() => UpdateSearchText(SearchText, SearchText)){Result = CommandResult.KeepOpen()}) {Title = $"Rolled: {lastResult.Result} ({lastResult.Quantity}D{lastResult.Range})"},
                new ListItem(new CopyTextCommand(lastResult.Result.ToString())) {Title = "Copy result"},
                new ListItem(new ProbabilityGraphPage([(1, 80), (2, 20)])){Title="Probability graph"},
            ];
        }
    }

    private ListItem DiceCommand(int range, string iconStr = "")
    {
        string text = $"D{range}";
        IconInfo icon;
        if (!string.IsNullOrEmpty(iconStr))
        {
            icon = new(iconStr);
        }
        else
        {
            icon = new(text[1..]);
        }

        return new ListItem(new AnonymousCommand(() =>
        {
            SearchText = text;
        })
        {
            Icon = icon,
            Name = $"Roll a D{range}", Result = CommandResult.KeepOpen()
        });
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        DiceResult? result = GetResult(newSearch);
        if (result != null)
        {
            diceResults.Add(result);
            lastResult = result;
        }
        else
        {
            lastResult = null;
        }
        RaiseItemsChanged();
    }

    private static DiceResult? GetResult(string text)
    {
        char[] allowedCharacters = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'D'];
        text = text.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(text) || text == "D" || text.Any((ch) => !allowedCharacters.Contains(ch)))
            return null;

        string[] splitText = text.Split("D");
        if ((splitText.Length == 2 && text[0] == 'D') || splitText.Length == 1)
        {
            bool rSuccess = int.TryParse(splitText.Last(), out int r);
            if (!rSuccess)
            {
                return null;
            }
            return new(r, 1);
        }
        else if (splitText.Length == 2)
        {

            bool qSuccess = int.TryParse(splitText[0], out int q);
            bool rSuccess = int.TryParse(splitText[1], out int r);
            if (!(rSuccess && qSuccess))
            {
                return null;
            }
            DiceResult result = new(r, q);
            return result;
        }
        else
        {
            return null;
        }
    }
}
