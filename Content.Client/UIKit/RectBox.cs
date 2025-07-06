using Content.Client.DrawKit;
using Robust.Client.Graphics;


namespace Content.Client.UIKit;


public sealed class RectBox : StyleBox
{
    [Dependency] private readonly DrawKitManager _drawKitManager = null!;

    public Color Color { get; set; }

    public Border Border { get; set; }

    public Border Inset { get; set; }

    public Rounding Rounding { get; set; }

    public RectBox()
    {
        IoCManager.InjectDependencies(this);
    }

    protected override void DoDraw(DrawingHandleScreen handle, UIBox2 box, float uiScale)
    {
        _drawKitManager.UsePen(
            handle,
            box,
            pen =>
            {
                pen.DrawRectangle(
                    box,
                    Color,
                    rounding: new(
                        Rounding.TopLeft * uiScale,
                        Rounding.TopRight * uiScale,
                        Rounding.BottomRight * uiScale,
                        Rounding.BottomLeft * uiScale
                    ),
                    border: Border with
                    {
                        Thickness = new(
                            Border.Thickness.Left * uiScale,
                            Border.Thickness.Top * uiScale,
                            Border.Thickness.Right * uiScale,
                            Border.Thickness.Bottom * uiScale
                        )
                    },
                    inset: Inset with
                    {
                        Thickness = new(
                            Inset.Thickness.Left * uiScale,
                            Inset.Thickness.Top * uiScale,
                            Inset.Thickness.Right * uiScale,
                            Inset.Thickness.Bottom * uiScale
                        )
                    }
                );
            });
    }
}
