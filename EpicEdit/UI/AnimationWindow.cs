using System;
using System.Linq;
using Psy.Core.EpicModel;
using Psy.Gui.Components;
using Psy.Core;
using Psy.Gui.Events;

namespace EpicEdit.UI
{
    public class AnimationWindow
    {
        private readonly Widget _widget;
        private readonly Editor _editor;
        private DropdownList _animationDropdownList;
        private Label _selectedFrameTimeLabel;
        private Button _animationCopyButton;
        private Button _animationPasteButton;
        private Button _frameCreateButton;
        private Button _frameSaveButton;
        private Button _frameLoadButton;
        private Button _frameDeleteButton;
        private Button _frameDuplicateButton;
        private Timeline _frameTimeline;
        private Timeline _secondsTimeline;
        private ToggleButton _playButton;
        private const string AnimationTypeDropdownListName = "animationType";
        private const string SelectedFrameTimeLabelName = "selectedFrameTime";
        private const string AnimationCopyButtonName = "animCopy";
        private const string AnimationPasteButtonName = "animPaste";
        private const string FrameCreateButtonName = "frameCreate";
        private const string FrameSaveButtonName = "frameSave";
        private const string FrameLoadButtonName = "frameLoad";
        private const string FrameDeleteButtonName = "frameDelete";
        private const string FrameDuplicateButtonName = "frameDuplicate";
        private const string FrameTimelineName = "frameTimeline";
        private const string SecondsTimelineName = "secondsTimeline";
        private const string PlayButtonName = "play";

        public AnimationWindow(Widget widget, Editor editor)
        {
            _widget = widget;
            _widget.Visible = false;

            _editor = editor;

            HookupWidgets();
        }

        public bool Visible { 
            get { return _widget.Visible; } 
            set
            {
                if (_widget.Visible == value)
                    return;

                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            } 
        }

        private void HookupWidgets()
        {
            _animationDropdownList = _widget.FindWidgetByUniqueName<DropdownList>(AnimationTypeDropdownListName);
            PopulateDropdownList();
            _animationDropdownList.Change += AnimationDropdownListOnChange;

            _selectedFrameTimeLabel = _widget.FindWidgetByUniqueName<Label>(SelectedFrameTimeLabelName);
            _animationCopyButton = _widget.FindWidgetByUniqueName<Button>(AnimationCopyButtonName);
            _animationPasteButton = _widget.FindWidgetByUniqueName<Button>(AnimationPasteButtonName);
            
            _frameCreateButton = _widget.FindWidgetByUniqueName<Button>(FrameCreateButtonName);
            _frameCreateButton.Click += FrameCreateButtonOnClick;

            _frameSaveButton = _widget.FindWidgetByUniqueName<Button>(FrameSaveButtonName);
            _frameSaveButton.Click += FrameSaveButtonOnClick;

            _frameLoadButton = _widget.FindWidgetByUniqueName<Button>(FrameLoadButtonName);
            _frameLoadButton.Click += FrameLoadButtonOnClick;

            _frameDeleteButton = _widget.FindWidgetByUniqueName<Button>(FrameDeleteButtonName);
            _frameDeleteButton.Click += FrameDeleteButtonOnClick;

            _frameDuplicateButton = _widget.FindWidgetByUniqueName<Button>(FrameDuplicateButtonName);

            _frameTimeline = _widget.FindWidgetByUniqueName<Timeline>(FrameTimelineName);
            _frameTimeline.MarkerDoubleClick += (sender, args) => MarkerDoubleClick(args.Marker.Id);
            _frameTimeline.MarkerClick += (sender, args) => MarkerSingleClick(args.Marker.Id);
            _frameTimeline.MarkerMove += (sender, args) => MarkerMove(args.Marker.Id);
            _frameTimeline.TimelineClick += FrameTimelineOnTimelineClick;
            

            _secondsTimeline = _widget.FindWidgetByUniqueName<Timeline>(SecondsTimelineName);
            _secondsTimeline.MarkerDoubleClick += (sender, args) => MarkerDoubleClick(args.Marker.Id);
            _secondsTimeline.MarkerClick += (sender, args) => MarkerSingleClick(args.Marker.Id);
            _secondsTimeline.MarkerMove += (sender, args) => MarkerMove(args.Marker.Id);
            _secondsTimeline.TimelineClick += SecondsTimelineOnTimelineClick;

            _playButton = _widget.FindWidgetByUniqueName<ToggleButton>(PlayButtonName);
            _playButton.Toggled += PlayButtonOnToggled;

        }

        private void PlayButtonOnToggled(object sender)
        {
            _editor.AnimationPlaying = _playButton.Value;
        }

        private void SecondsTimelineOnTimelineClick(object sender, TimelineClickEventArgs args)
        {
            _editor.ApplyAnimationAtTime(args.Position);
        }

        private void FrameTimelineOnTimelineClick(object sender, TimelineClickEventArgs args)
        {
            _editor.ApplyAnimationAtTime(args.Position / 24.0f);
        }


        private void FrameDeleteButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.DeleteKeyFrame(_editor.SelectedKeyFrameIndex);
        }

        private void FrameLoadButtonOnClick(object sender, ClickEventArgs args)
        {
            if (_editor.Animation == null)
                return;
            _editor.Animation.ApplyKeyFrame(_editor.SelectedKeyFrameIndex);
        }

        private void FrameSaveButtonOnClick(object sender, ClickEventArgs args)
        {
            if (_editor.Animation == null)
                return;
            _editor.Animation.SaveModelStateIntoKeyFrame(_editor.SelectedKeyFrameIndex);
        }

        private void MarkerMove(string markerId)
        {
            int id;
            if (!int.TryParse(markerId, out id))
            {
                _editor.Error(string.Format("Marker with id `{0}` should have an integer id", markerId));
                return;
            }

            var marker = _secondsTimeline.Markers.Single(x => x.Id == markerId);
            _editor.MoveKeyFrame(id, marker.Position);

            Console.Write("Marker moved.");

        }

        private void MarkerSingleClick(string markerId)
        {
            int id;
            if (!int.TryParse(markerId, out id))
            {
                _editor.Error(string.Format("Marker with id `{0}` should have an integer id", markerId));
                return;
            }

            var marker = _secondsTimeline.Markers.Single(x => x.Id == markerId);
            _selectedFrameTimeLabel.Text = marker.Position.ToString("0.000");

            _editor.SelectKeyFrame(id);
        }

        private void MarkerDoubleClick(string markerId)
        {
            int id;
            if (!int.TryParse(markerId, out id))
            {
                _editor.Error(string.Format("Marker with id `{0}` should have an integer id", markerId));
                return;
            }

            var marker = _secondsTimeline.Markers.Single(x => x.Id == markerId);
            _selectedFrameTimeLabel.Text = marker.Position.ToString("0.000");

            _editor.SelectKeyFrame(id);
            _editor.ApplyKeyFrame(id);
        }

        private void AnimationDropdownListOnChange(object sender)
        {
            AnimationType animationType;
            var animationTypeName = _animationDropdownList.SelectedItemText;
            if (!Enum.TryParse(animationTypeName, out animationType))
            {
                _editor.Error(string.Format("Unknown animation type `{0}`", animationTypeName));
                return;
            }

            _editor.SelectedAnimationType = animationType;
        }

        private void FrameCreateButtonOnClick(object sender, ClickEventArgs args)
        {
            _editor.CreateAnimationFrame();

        }

        private void PopulateDropdownList()
        {
            _animationDropdownList.Items.Clear();
            foreach (var name in Enum.GetNames(typeof(AnimationType)))
            {
                _animationDropdownList.Items.Add(new DropdownListItem(name, name));
            }
        }

        public void Show()
        {
            _animationDropdownList.SelectedItemText = _editor.SelectedAnimationType.ToString();
            Refresh();
            _widget.Visible = true;
        }

        public void Refresh()
        {
            if (_editor.Animation == null)
            {
                _frameTimeline.Markers.Clear();
                _secondsTimeline.Markers.Clear();
                return;
            }

            RefreshPlayTimeMarker();

            var maxIndex = -1;

            foreach (var keyframe in _editor.Animation.Keyframes.IndexOver())
            {
                var id = keyframe.Index.ToString();
                maxIndex = keyframe.Index;
                var time = (float)keyframe.Value.Time;
                var frame = (float)keyframe.Value.Time * 24.0f;

                var colour = keyframe.Index == 0 ? Colours.Red : Colours.White;

                if (keyframe.Index == _editor.SelectedKeyFrameIndex)
                {
                    colour = Colours.Yellow;
                }

                var shape = keyframe.Index == 0 ? TimelineMarkerShape.Square : TimelineMarkerShape.Diamond;

                var frameTimelineMarker = _frameTimeline.Markers.SingleOrDefault(x => x.Id == id);
                if (frameTimelineMarker == null)
                {
                    _frameTimeline.Markers.Add(new TimelineMarker(id, frame, colour, shape));    
                }
                else
                {
                    frameTimelineMarker.Position = frame;
                    frameTimelineMarker.Colour = colour;
                }

                var secondsTimelineMarker = _secondsTimeline.Markers.SingleOrDefault(x => x.Id == id);
                if (secondsTimelineMarker == null)
                {
                    _secondsTimeline.Markers.Add(new TimelineMarker(id, time, colour, shape));
                }
                else
                {
                    secondsTimelineMarker.Position = time;
                    secondsTimelineMarker.Colour = colour;
                }
            }

            var deadMarkers = _frameTimeline.Markers.Where(marker => marker.Id != "PLAYCURSOR" && int.Parse(marker.Id) > maxIndex).ToList();
            foreach (var deadMarker in deadMarkers)
            {
                _frameTimeline.Markers.Remove(deadMarker);
            }

            deadMarkers = _secondsTimeline.Markers.Where(marker => marker.Id != "PLAYCURSOR" && int.Parse(marker.Id) > maxIndex).ToList();
            foreach (var deadMarker in deadMarkers)
            {
                _secondsTimeline.Markers.Remove(deadMarker);
            }
        }

        private void RefreshPlayTimeMarker()
        {
            var secondsPlayMarker = _secondsTimeline.Markers.SingleOrDefault(x => x.Id == "PLAYCURSOR");
            if (secondsPlayMarker != null)
            {
                secondsPlayMarker.Position = _editor.AnimationTime;
            }
            else
            {
                _secondsTimeline.Markers.Add(new TimelineMarker("PLAYCURSOR", 0, Colours.Orange, TimelineMarkerShape.Line));
            }
            var frameTimeMarker = _frameTimeline.Markers.SingleOrDefault(x => x.Id == "PLAYCURSOR");
            if (frameTimeMarker != null)
            {
                frameTimeMarker.Position = _editor.AnimationTime * 24;
            }
            else
            {
                _frameTimeline.Markers.Add(new TimelineMarker("PLAYCURSOR", 0, Colours.Orange, TimelineMarkerShape.Line));
            }
        }

        public void Hide()
        {
            _widget.Visible = false;
        }
    }
}