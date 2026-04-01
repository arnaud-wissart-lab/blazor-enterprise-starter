namespace BlazorEnterpriseStarter.Components.Common;

internal sealed class CssClassBuilder
{
    private readonly List<string> _classes = [];

    public CssClassBuilder Add(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _classes.Add(value);
        }

        return this;
    }

    public CssClassBuilder AddIf(string value, bool condition)
    {
        if (condition)
        {
            _classes.Add(value);
        }

        return this;
    }

    public override string ToString() => string.Join(" ", _classes);
}
