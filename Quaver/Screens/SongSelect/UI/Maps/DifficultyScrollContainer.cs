﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Quaver.Database.Maps;
using Quaver.Scheduling;
using Quaver.Screens.SongSelect.UI.Mapsets;
using Wobble.Graphics;
using Wobble.Graphics.Animations;
using Wobble.Graphics.Sprites;
using Wobble.Graphics.UI.Dialogs;
using Wobble.Input;
using Wobble.Logging;
using Wobble.Screens;
using Wobble.Window;

namespace Quaver.Screens.SongSelect.UI.Maps
{
    public class DifficultyScrollContainer : ScrollContainer
    {
        /// <summary>
        ///     Reference to the game screen.
        /// </summary>
        public SongSelectScreen Screen => View.Screen as SongSelectScreen;

        /// <summary>
        ///     Reference to the parent ScreenView
        /// </summary>
        public SongSelectScreenView View { get; }

        /// <summary>
        ///     The original size of the container for reference.
        /// </summary>
        public ScalableVector2 OriginalContainerSize { get; }

        /// <summary>
        ///     The index of the selected map in the set.
        /// </summary>
        public int SelectedMapIndex { get; private set; }

        /// <summary>
        ///     The buffer of DrawableDifficulty that's used to display the mapset's containing maps.
        /// </summary>
        public List<DrawableDifficulty> DifficultyBuffer { get; private set; }

        /// <summary>
        ///     The amount of maps able to be used in <see cref="DifficultyBuffer"/>
        /// </summary>
        public static int MAX_BUFFER_SIZE { get; } = 8;

        /// <summary>
        ///     The index of difficulties at which the difficulties will be displayed.
        ///     If the index is 2, it'll display difficulties: PoolStartingIndex + <see cref="MAX_BUFFER_SIZE"/>
        /// </summary>
        public int PoolStartingIndex { get; private set; }

        /// <summary>
        ///     The amount of space between each difficulty.
        /// </summary>
        public int YSpacing { get; } = 10;

        /// <summary>
        ///     The amount of y space that is given before the first difficulty,
        ///     used so that the difficulties don't appear directly at the top of the container because
        ///     it can be hidden by the search interface.
        /// </summary>
        public int YSpaceBeforeFirstDifficulty { get; } = 300;

        /// <summary>
        ///     Keeps track of the Y position of the content container in the previous frame
        ///     So we can know how to shift the pool.
        /// </summary>
        private float PreviousContentContainerY { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="view"></param>
        public DifficultyScrollContainer(SongSelectScreenView view) : base(
            new ScalableVector2(515, WindowManager.Height - 54 * 2 - 2),
            new ScalableVector2(515, WindowManager.Height - 54 * 2 - 2))
        {
            View = view;
            OriginalContainerSize = Size;
            Alpha = 0;

            InputEnabled = true;
            Scrollbar.Tint = Color.White;
            Scrollbar.Width = 5;
            Scrollbar.X += 10;
            ScrollSpeed = 150;
            EasingType = Easing.OutQuint;
            TimeToCompleteScroll = 1500;

            // The index of the selected map.
            InitializeBuffer();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            InputEnabled = GraphicsHelper.RectangleContains(ScreenRectangle, MouseManager.CurrentState.Position) && DialogManager.Dialogs.Count == 0;

            if (ContentContainer.Y < PreviousContentContainerY)
                HandlePoolShifting(Direction.Forward);
            else if (ContentContainer.Y > PreviousContentContainerY)
                HandlePoolShifting(Direction.Backward);

            // Update the previous y, AFTER checking and handling the pool shifting.
            PreviousContentContainerY = ContentContainer.Y;
            base.Update(gameTime);
        }

        /// <summary>
        ///     Contains the buffer of DrawableDifficulties that'll be used to display the difficulties of the map.
        /// </summary>
        private void InitializeBuffer()
        {
            DifficultyBuffer = new List<DrawableDifficulty>(MAX_BUFFER_SIZE);
            SetPoolStartingIndex();

            // Create MAX_BUFFER_SIZE  amount of DrawableDifficulties
            for (var i = 0; i < MAX_BUFFER_SIZE; i++)
            {
                var difficulty = new DrawableDifficulty()
                {
                    Alignment = Alignment.TopRight,
                    Y = (PoolStartingIndex + i) * DrawableDifficulty.HEIGHT + (PoolStartingIndex + i) * YSpacing + YSpaceBeforeFirstDifficulty,
                    DestroyIfParentIsNull = false
                };

                if (i >= MapManager.Selected.Value.Mapset.Maps.Count)
                    break;
                difficulty.UpdateWithNewMap(MapManager.Selected.Value.Mapset.Maps[PoolStartingIndex + i]);
                DifficultyBuffer.Add(difficulty);

                if (i >= MapManager.Selected.Value.Mapset.Maps.Count)
                    continue;

                AddContainedDrawable(difficulty);

                /*if (i == SelectedMapsetIndex)
                    mapset.DisplayAsSelected(MapManager.Selected.Value);*/
            }

            RecalculateContainerHeight();
            SnapToInitialDifficulty();
            // UpdateButtonSelectedStatus();
        }

        /// <summary>
        ///    Based on the currently selected mapset, calculate starting index of which to update and draw
        ///    the mapset buttons in the container.
        /// </summary>
        private void SetPoolStartingIndex()
        {
            SelectedMapIndex = Screen.AvailableMapsets[View.MapsetScrollContainer.SelectedMapsetIndex].Maps.FindIndex(x => x == MapManager.Selected.Value);

            if (SelectedMapIndex < MAX_BUFFER_SIZE / 2)
                PoolStartingIndex = 0;
            else if (SelectedMapIndex + MAX_BUFFER_SIZE > MapManager.Selected.Value.Mapset.Maps.Count)
                PoolStartingIndex = MapManager.Selected.Value.Mapset.Maps.Count - MAX_BUFFER_SIZE;
            else
                PoolStartingIndex = SelectedMapIndex - MAX_BUFFER_SIZE / 2;
        }

        /// <summary>
        ///     Recalculates the height of the container based on the amount of difficulties so that we have room to scroll.
        /// </summary>
        private void RecalculateContainerHeight()
        {
            var totalUserHeight = DrawableDifficulty.HEIGHT * MapManager.Selected.Value.Mapset.Maps.Count +
                                  MapManager.Selected.Value.Mapset.Maps.Count * YSpacing + YSpaceBeforeFirstDifficulty * 2;

            if (totalUserHeight > Height)
                ContentContainer.Height = totalUserHeight;
            else
                ContentContainer.Height = Height;
        }

        /// <summary>
        ///     Snaps the scroll container to the proper difficulty.
        /// </summary>
        private void SnapToInitialDifficulty()
        {
            ContentContainer.Y = (-SelectedMapIndex - 3) * DrawableDifficulty.HEIGHT + (-SelectedMapIndex - 3) * YSpacing + YSpaceBeforeFirstDifficulty;

            PreviousContentContainerY = ContentContainer.Y;
            TargetY = PreviousContentContainerY;
            PreviousTargetY = PreviousContentContainerY;
            ContentContainer.Animations.Clear();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="direction"></param>
        private void HandlePoolShifting(Direction direction)
        {
            switch (direction)
            {
                case Direction.Forward:
                    // If there are no available maps then there's no need to do anything.
                    if (MapManager.Selected.Value.Mapset.Maps.ElementAtOrDefault(PoolStartingIndex) == null
                        || MapManager.Selected.Value.Mapset.Maps.ElementAtOrDefault(PoolStartingIndex + MAX_BUFFER_SIZE) == null)
                        return;

                    var firstDifficulty = DifficultyBuffer.First();

                    // Check if the object is in the rect of the ScrollContainer.
                    // If it is, then there's no updating that needs to happen.
                    if (!Rectangle.Intersect(firstDifficulty.ScreenRectangle.ToRectangle(), ScreenRectangle.ToRectangle()).IsEmpty)
                        return;

                    // Update the mapset's information and y position.
                    firstDifficulty.Y = (PoolStartingIndex + MAX_BUFFER_SIZE) * DrawableDifficulty.HEIGHT +
                                    (PoolStartingIndex + MAX_BUFFER_SIZE) * YSpacing + YSpaceBeforeFirstDifficulty;

                    lock (MapManager.Selected.Value.Mapset.Maps)
                    {
                        firstDifficulty.UpdateWithNewMap(MapManager.Selected.Value.Mapset.Maps[PoolStartingIndex + MAX_BUFFER_SIZE]);
                    }

                    // Circuluarly Shift the list forward one.
                    DifficultyBuffer.Remove(firstDifficulty);
                    DifficultyBuffer.Add(firstDifficulty);

                    /*// Make sure the set is corrected selected/deselected
                    if (PoolStartingIndex + MAX_BUFFER_SIZE == SelectedMapsetIndex)
                        //firstDifficulty.DisplayAsSelected(MapManager.Selected.Value);
                    else
                        //firstDifficulty.DisplayAsDeselected();*/

                    PoolStartingIndex++;
                    break;
                case Direction.Backward:
                    // If there are no previous available map then there's no need to shift.
                    if (MapManager.Selected.Value.Mapset.Maps.ElementAtOrDefault(PoolStartingIndex - 1) == null)
                        return;

                    var lastDifficulty = DifficultyBuffer.Last();

                    // Check if the object is in the rect of the ScrollContainer.
                    // If it is, then there's no updating that needs to happen.
                    if (!Rectangle.Intersect(lastDifficulty.ScreenRectangle.ToRectangle(), ScreenRectangle.ToRectangle()).IsEmpty)
                        return;

                    lastDifficulty.Y = (PoolStartingIndex - 1) * DrawableDifficulty.HEIGHT + (PoolStartingIndex - 1) * YSpacing + YSpaceBeforeFirstDifficulty;

                    lock (MapManager.Selected.Value.Mapset.Maps)
                    {
                        lastDifficulty.UpdateWithNewMap(MapManager.Selected.Value.Mapset.Maps[PoolStartingIndex - 1]);
                    }


                    DifficultyBuffer.Remove(lastDifficulty);
                    DifficultyBuffer.Insert(0, lastDifficulty);

                    // Make sure the set is correctly selected/deselected.
                    /*if (PoolStartingIndex - 1 == SelectedMapsetIndex)
                        lastDifficulty.DisplayAsSelected(MapManager.Selected.Value);
                    else
                        lastDifficulty.DisplayAsDeselected();*/

                    PoolStartingIndex--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        /// <summary>
        ///     Refreshes the buffer with the mapset's updated difficulties.
        /// </summary>
        public void ReInitializeDifficulties()
        {
            SetPoolStartingIndex();

            var maps = MapManager.Selected.Value.Mapset.Maps;

            // Go through each map, determine if it is needed to be displayed.
            // If it is, update the map contents/add it to the contained drawables.
            // if not, remove it from the contained drawables to hide it.
            for (var i = 0; i < DifficultyBuffer.Count; i++)
            {
                var difficulty = DifficultyBuffer[i];

                // Drawable is needed.
                if (i < MAX_BUFFER_SIZE && i < maps.Count)
                {
                    difficulty.UpdateWithNewMap(MapManager.Selected.Value.Mapset.Maps[PoolStartingIndex + i]);

                    if (difficulty.Parent != ContentContainer)
                        AddContainedDrawable(difficulty);
                }
                // Drawable needs to be hidden.
                else
                {
                    if (difficulty.Parent == ContentContainer)
                        RemoveContainedDrawable(difficulty);
                }
            }

            RecalculateContainerHeight();
            SnapToInitialDifficulty();
        }
    }
}