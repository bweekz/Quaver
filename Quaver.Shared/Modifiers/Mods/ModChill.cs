/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using Microsoft.Xna.Framework;
using Quaver.API.Enums;

namespace Quaver.Shared.Modifiers.Mods
{
    /// <summary>
    ///     Chill gameplayModifier. Makes the hit timing windows
    /// </summary>
    internal class ModChill : IGameplayModifier
    {
        public string Name { get; set; } = "Chill";

        public ModIdentifier ModIdentifier { get; set; } = ModIdentifier.Chill;

        public ModType Type { get; set; } = ModType.DifficultyDecrease;

        public string Description { get; set; } = "Make it easier on yourself.";

        public bool Ranked { get; set; } = true;

        public bool AllowedInMultiplayer { get; set; } = true;

        public bool OnlyMultiplayerHostCanCanChange { get; set; }

        public ModIdentifier[] IncompatibleMods { get; set; } = { ModIdentifier.Strict };

        public Color ModColor { get; }

        public void InitializeMod() => throw new NotImplementedException();
    }
}
