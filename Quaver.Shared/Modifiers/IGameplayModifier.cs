/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) Swan & The Quaver Team <support@quavergame.com>.
*/

using Microsoft.Xna.Framework;
using Quaver.API.Enums;

namespace Quaver.Shared.Modifiers
{
    public interface IGameplayModifier
    {
        /// <summary>
        ///     The name of the Mod
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     The identifier of the the gameplayModifier.
        /// </summary>
        ModIdentifier ModIdentifier { get; set; }

        /// <summary>
        ///     The type of gameplayModifier as defined in the enum
        /// </summary>
        ModType Type { get; set; }

        /// <summary>
        ///     The description of the Mod
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///     Is the gameplayModifier ranked?
        /// </summary>
        bool Ranked { get; set; }

        /// <summary>
        ///     Is the modifier allowed in multiplayer?
        /// </summary>
        bool AllowedInMultiplayer { get; set; }

        /// <summary>
        ///     Dictates if only the host of the multiplayer game can change this modifier
        /// </summary>
        bool OnlyMultiplayerHostCanCanChange { get; set; }

        /// <summary>
        ///     The identifier of mods that are incompatible with this one.
        /// </summary>
        ModIdentifier[] IncompatibleMods { get; set; }

        Color ModColor { get; }

        /// <summary>
        ///     All the gameplayModifier logic should go here.
        /// </summary>
        void InitializeMod();
    }
}
