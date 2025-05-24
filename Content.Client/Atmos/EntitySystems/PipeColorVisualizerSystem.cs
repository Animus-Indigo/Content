using Content.Client.Atmos.Components;
using Content.Client.Atmos.Visualizers;
using Robust.Client.GameObjects;
using Content.Shared.Atmos.Piping;


namespace Content.Client.Atmos.EntitySystems;


public sealed class PipeColorVisualizerSystem : VisualizerSystem<PipeColorVisualsComponent>
{
    protected override void OnAppearanceChange(
        EntityUid uid,
        PipeColorVisualsComponent component,
        ref AppearanceChangeEvent args
    )
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)
            || !AppearanceSystem.TryGetData<Color>(uid, PipeColorVisuals.Color, out var color, args.Component))
            return;

        if (!sprite.LayerMapTryGet(PipeVisualLayers.Pipe, out var index))
            return;

        // T-ray scanner / sub floor runs after this visualizer. Lets not bulldoze transparency.
        var layer = sprite[index];
        layer.Color = color.WithAlpha(layer.Color.A);
    }
}
