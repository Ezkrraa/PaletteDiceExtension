using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteDiceExtension.Pages
{
    internal sealed partial class ProbabilityGraphPage : ContentPage
    {
        private readonly List<(int, int)> _chances;
        public ProbabilityGraphPage(List<(int, int)> chances)
        {
            _chances = chances;
        }
        public override IContent[] GetContent()
        {
            return [new MarkdownContent("Hi this is a doc :3 \n# I AM A HEADERRR")];
        }
    }
}
