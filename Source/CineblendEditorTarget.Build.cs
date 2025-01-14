using Flax.Build;

public class CineblendEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add("Cineblend");
        Modules.Add("CineblendEditor");
    }
}
