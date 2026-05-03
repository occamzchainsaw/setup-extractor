namespace Extractor.Core.Model;

public class SetupShopsData
{
    public List<SetupShopDenomination> Shops { get; set; } = [];
}

public sealed class SetupShopDenomination
{
    public string Cardinal { get; set; } = string.Empty;
    public List<string> Aliases { get; set; } = [];
}