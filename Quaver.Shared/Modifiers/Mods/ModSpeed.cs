/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.Shared.Audio;
using Quaver.Shared.Helpers;

namespace Quaver.Shared.Modifiers.Mods
{
    internal class ModSpeed : IGameplayModifier
    {
        public string Name { get; set; } = "Speed";

        /// <inheritdoc />
        /// <summary>
        ///     Speed can have a variable amount of mod identifiers, so this should be handled manually.
        /// </summary>
        public ModIdentifier ModIdentifier { get; set; }

        public ModType Type { get; set; } = ModType.Speed;

        public string Description { get; set; } = "Change the audio playback rate of the song.";

        public bool Ranked { get; set; } = true;

        public bool AllowedInMultiplayer { get; set; } = true;

        public bool OnlyMultiplayerHostCanCanChange { get; set; }

        public ModIdentifier[] IncompatibleMods { get; set; } =
        {
            ModIdentifier.Speed05X,
            ModIdentifier.Speed055X,
            ModIdentifier.Speed06X,
            ModIdentifier.Speed065X,
            ModIdentifier.Speed07X,
            ModIdentifier.Speed075X,
            ModIdentifier.Speed08X,
            ModIdentifier.Speed085X,
            ModIdentifier.Speed09X,
            ModIdentifier.Speed095X,
            ModIdentifier.Speed11X,
            ModIdentifier.Speed12X,
            ModIdentifier.Speed13X,
            ModIdentifier.Speed14X,
            ModIdentifier.Speed15X,
            ModIdentifier.Speed16X,
            ModIdentifier.Speed17X,
            ModIdentifier.Speed18X,
            ModIdentifier.Speed19X,
            ModIdentifier.Speed20X,
        };

        public Color ModColor { get; } = ColorHelper.HexToColor("#A35596");

        /// <summary>
        /// </summary>
        /// <param name="modIdentifier"></param>
        public ModSpeed(ModIdentifier modIdentifier) => ModIdentifier = modIdentifier;

        public void InitializeMod()
        {
            try
            {
                AudioEngine.Track.Rate = ModHelper.GetRateFromMods(ModIdentifier);
            }
            catch (Exception)
            {
                // ignored
            }

            // Remove the incoming mod from the list of incompatible ones.
            var im = IncompatibleMods.ToList();
            im.Remove(ModIdentifier);
            IncompatibleMods = im.ToArray();
        }
    }
}
