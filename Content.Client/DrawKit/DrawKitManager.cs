// Copyright (C) 2025 Igor Spichkin

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Numerics;
using Content.Client.UIKit;
using Content.Client.UserInterface;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;
using Vector4 = Robust.Shared.Maths.Vector4;


namespace Content.Client.DrawKit;


public sealed class DrawKitManager
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = null!;

    private static readonly ProtoId<ShaderPrototype> CircleShaderPrototype    = "DrawKitCircle";
    private static readonly ProtoId<ShaderPrototype> RectangleShaderPrototype = "DrawKitRectangle";

    private ShaderPrototypes _shaders;

    public void Initialize()
    {
        _shaders = new(
            _prototypeManager.Index(CircleShaderPrototype),
            _prototypeManager.Index(RectangleShaderPrototype)
        );
    }

    public void UsePen(DrawingHandleScreen handle, UIBox2 rect, Action<Pen> draw)
    {
        var pen = new Pen(handle, rect, _shaders);

        try
        {
            draw(pen);
        }
        finally
        {
            pen.Dispose();
        }
    }

    internal readonly record struct ShaderPrototypes(ShaderPrototype CircleShader, ShaderPrototype RectangleShader);

    public readonly record struct Pen
    {
        private readonly DrawingHandleScreen  _handle;
        private readonly Vector2[]            _vertices;
        private readonly ShaderPrototypes     _shaders;
        private readonly List<ShaderInstance> _shaderInstances = [];

        internal Pen(
            DrawingHandleScreen handle,
            UIBox2              dstRect,
            ShaderPrototypes    shaders
        )
        {
            _handle = handle;
            _vertices =
            [
                dstRect.TopLeft, dstRect.TopRight, dstRect.BottomRight, dstRect.TopLeft, dstRect.BottomRight,
                dstRect.BottomLeft
            ];
            _shaders = shaders;
        }

        public void DrawCircle(
            Vector2 center,
            float   radius,
            Color   color,
            float   border      = 0.0f,
            Color?  borderColor = null,
            float   inset       = 0.0f,
            Color?  insetColor  = null
        )
        {
            borderColor ??= Color.Transparent;
            insetColor  ??= Color.Transparent;

            var shader       = UseShader(_shaders.CircleShader);
            var globalCenter = Vector2.Transform(center, _handle.GetTransform());

            shader.SetParameter("fillColor", color);
            shader.SetParameter("borderColor", borderColor.Value);
            shader.SetParameter("insetColor", insetColor.Value);
            shader.SetParameter("params", new Vector4(globalCenter.X, globalCenter.Y, radius, 0.0f));
            shader.SetParameter("thickness", new Vector4(border, inset, 0.0f, 0.0f));

            var oldShader = _handle.GetShader();

            try
            {
                _handle.UseShader(shader);
                _handle.DrawPrimitives(DrawPrimitiveTopology.TriangleList, _vertices, Color.White);
            }
            finally
            {
                _handle.UseShader(oldShader);
            }
        }

        // TODO: Add variable border thickness
        public void DrawRectangle(
            UIBox2   rect,
            Color    color,
            Rounding rounding = new(),
            Border   border   = new(),
            Border   inset    = new()
        )
        {
            var shader    = UseShader(_shaders.RectangleShader);
            var oldShader = _handle.GetShader();

            var globalCenter = Vector2.Transform(rect.Center, _handle.GetTransform());

            shader.SetParameter("fillColor", color);
            shader.SetParameter("borderColor", border.Color);
            shader.SetParameter("insetColor", inset.Color);
            shader.SetParameter(
                "rect",
                new Vector4(globalCenter.X, globalCenter.Y, rect.Width / 2.0f, rect.Height / 2.0f)
            );
            shader.SetParameter(
                "rounding",
                new Vector4(rounding.TopLeft, rounding.TopRight, rounding.BottomRight, rounding.BottomLeft)
            );
            shader.SetParameter(
                "thickness",
                new Vector4(
                    border.Thickness.Left,
                    inset.Thickness.Left,
                    0.0f,
                    0.0f
                )
            );

            try
            {
                _handle.UseShader(shader);
                _handle.DrawPrimitives(DrawPrimitiveTopology.TriangleList, _vertices, Color.White);
            }
            finally
            {
                _handle.UseShader(oldShader);
            }
        }

        private ShaderInstance UseShader(ShaderPrototype shaderPrototype)
        {
            var instance = shaderPrototype.InstanceUnique();
            _shaderInstances.Add(instance);

            return instance;
        }

        internal void Dispose()
        {
            foreach (var shader in _shaderInstances)
                shader.Dispose();
        }
    }
}
