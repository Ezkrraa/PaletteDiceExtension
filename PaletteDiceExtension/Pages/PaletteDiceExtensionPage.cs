// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Collections.Generic;
using static PaletteDiceExtension.Parser.Parser;

namespace PaletteDiceExtension;

internal sealed partial class PaletteDiceExtensionPage : DynamicListPage
{
    public override IconInfo Icon => new IconInfo("🎲");

    // unused for now but will make a history feature later
    // TODO: make history feature
    private List<int> diceResults = [];
    private int? lastResult = null;
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
            string resultStr = $"{lastResult}";
            IconInfo iconInfo = new(resultStr.Length <= 2 ? resultStr : "🎲");
            return [
                new ListItem(new AnonymousCommand(() => UpdateSearchText(SearchText, SearchText)){Result = CommandResult.KeepOpen(), Icon = iconInfo}){Title = $"Rolled: {lastResult}"},
                new ListItem(new CopyTextCommand(resultStr)) {Title = "Copy result"},
                // TODO: make a page with probability graph
                //new ListItem(new ProbabilityGraphPage([(1, 80), (2, 20)])){Title="Probability graph"},
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
        int? result = ParseExpression(newSearch);
        if (result != null)
        {
            diceResults.Add(result ?? int.MinValue);
            lastResult = result;
        }
        else
        {
            lastResult = null;
        }
        RaiseItemsChanged();
    }
}
