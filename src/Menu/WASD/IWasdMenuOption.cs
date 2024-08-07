using CounterStrikeSharp.API.Core;

namespace BlockBuilder;

public interface IWasdMenuOption
{
    public IWasdMenu? Parent { get; set; }
    public string? OptionDisplay { get; set; }
    public Action<CCSPlayerController, IWasdMenuOption>? OnChoose { get; set; }
    public int Index { get; set; }
}