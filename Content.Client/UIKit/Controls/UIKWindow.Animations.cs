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

using Content.Shared.UIKit;
using Robust.Client.Animations;


namespace Content.Client.UIKit.Controls;


public partial class UIKWindow
{
    private const string OpenOpacityAnimationKey = "OpenOpacityAnimationKey";

    private static readonly Animation OpenOpacityAnimation = new()
    {
        Length = TimeSpan.FromSeconds(AnimationConstants.ControlFadeInDuration),
        AnimationTracks =
        {
            new AnimationTrackControlProperty
            {
                Property = nameof(Modulate),
                KeyFrames =
                {
                    new(new Color(255, 255, 255, 0), 0.0f),
                    new(new Color(255, 255, 255), AnimationConstants.ControlFadeInDuration)
                }
            }
        }
    };

    protected override void Opened()
    {
        base.Opened();

        if (HasRunningAnimation(OpenOpacityAnimationKey))
            StopAnimation(OpenOpacityAnimationKey);

        Modulate = new(255, 255, 255, 127);
        PlayAnimation(OpenOpacityAnimation, OpenOpacityAnimationKey);
    }
}
