using Reactive.Bindings;
using System.Collections.Generic;

namespace IdentityClient
{
    public class MainWindowViewModel
    {
        public ReactiveCollection<Dictionary<string,string>> Dic { get; set; }
        public MainWindowViewModel()
        {
            Dic = new ReactiveCollection<Dictionary<string, string>>();
        }
    }
}
