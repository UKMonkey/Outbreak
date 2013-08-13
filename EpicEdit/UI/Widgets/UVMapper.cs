using System.Xml;
using Psy.Core;
using Psy.Core.Input;
using Psy.Gui;
using Psy.Gui.Components;
using Psy.Gui.Events;
using Psy.Gui.Loader;
using SlimMath;

namespace EpicEdit.UI.Widgets
{
    public delegate void UVPointChangedEvent(object sender, UVPointChangedEventArgs args);

    public class UVPointChangedEventArgs
    {
        public int PointIndex { get; private set; }
        public Vector2 Position { get; private set; }

        public UVPointChangedEventArgs(int pointIndex, Vector2 position)
        {
            PointIndex = pointIndex;
            Position = position;
        }
    }

    public class UVMapper : Widget
    {
        private const string XmlNodeName = "uvmapper";

        private const int PointClickDistanceThreshold = 8;
        private const float PointSelectorSquareSize = 4.0f;

        public event UVPointChangedEvent UVPointChanged;

        public string ImageName { get; set; }
        public Vector2[] Points { get; set; }

        private Vector2? TranslateStart { get; set; }
        private Vector2? SquareBoxStart { get; set; }
        private int? SelectedPointIndex { get; set; }
        private int? HoverPointIndex { get; set; }

        private UVMapper(GuiManager guiManager, Widget parent = null) : base(guiManager, parent) { }

        private static Widget Create(GuiManager guiManager, XmlElement xmlElement, Widget parent)
        {
            var widget = new UVMapper(guiManager, parent)
            {
                ImageName = xmlElement.GetString("imageName"),
            };
            return widget;
        }

        private void OnUVPointChanged(UVPointChangedEventArgs args)
        {
            var handler = UVPointChanged;
            if (handler != null) handler(this, args);
        }

        protected override void OnMouseMove(MouseMoveEventArgs args)
        {
            if (SquareBoxStart.HasValue)
            {
                SquareBoxUpdate(args.Position);
            }
            else if (TranslateStart.HasValue)
            {
                TranslateUpdate(args.Position);
            }
            else if (!SelectedPointIndex.HasValue)
            {
                var point = GetPointIndex(args.Position);
                HoverPointIndex = point.HasValue ? point : null;
            }
            else
            {
                var position = args.Position.InvScale(Size);
                Points[SelectedPointIndex.Value] = position;
                OnUVPointChanged(new UVPointChangedEventArgs(SelectedPointIndex.Value, position));
            }

            base.OnMouseMove(args);
        }

        protected override void OnMouseDown(MouseEventArguments args)
        {
            if (args.Button == MouseButton.Left)
            {
                var point = GetPointIndex(args.Position);
                SelectedPointIndex = point.HasValue ? point : null;
            }
            else if (args.Button == MouseButton.Right)
            {
                if (Points.Length == 4)
                {
                    SquareBoxStart = args.Position.InvScale(Size);    
                }
            }
            else if (args.Button == MouseButton.Middle)
            {
                TranslateStart = args.Position.InvScale(Size);
            }
            base.OnMouseDown(args);
        }

        protected override void OnMouseUp(MouseEventArguments args)
        {
            SelectedPointIndex = null;
            HoverPointIndex = null;

            if (SquareBoxStart.HasValue)
            {
                SquareBoxUpdate(args.Position);
            }

            SquareBoxStart = null;
            TranslateStart = null;
            base.OnMouseUp(args);
        }

        private void TranslateUpdate(Vector2 position)
        {
            var pos = position.InvScale(Size);

            var diff = pos - TranslateStart.Value;
            TranslateStart = pos;

            Points[0] = Points[0] + diff;
            OnUVPointChanged(new UVPointChangedEventArgs(0, Points[0]));

            Points[1] = Points[1] + diff;
            OnUVPointChanged(new UVPointChangedEventArgs(1, Points[1]));

            Points[2] = Points[2] + diff;
            OnUVPointChanged(new UVPointChangedEventArgs(2, Points[2]));

            Points[3] = Points[3] + diff;
            OnUVPointChanged(new UVPointChangedEventArgs(3, Points[3]));
        }

        private void SquareBoxUpdate(Vector2 position)
        {
            var sbs = SquareBoxStart.Value;
            var pos = position.InvScale(Size);

            var topX = sbs.X;
            var topY = sbs.Y;
            var botX = pos.X;
            var botY = pos.Y;

            Points[0] = new Vector2(topX, botY);
            OnUVPointChanged(new UVPointChangedEventArgs(0, Points[0]));

            Points[1] = new Vector2(topX, topY);
            OnUVPointChanged(new UVPointChangedEventArgs(1, Points[1]));

            Points[2] = new Vector2(botX, topY);
            OnUVPointChanged(new UVPointChangedEventArgs(2, Points[2]));

            Points[3] = new Vector2(botX, botY);
            OnUVPointChanged(new UVPointChangedEventArgs(3, Points[3]));
        }

        private int? GetPointIndex(Vector2 testPosition)
        {
            if (Points == null)
            {
                return null;
            }

            for (var index = 0; index < Points.Length; index++)
            {
                var pointPosition = Points[index];
                var dist = Vector2.Distance(pointPosition, testPosition.InvScale(Size));
                if (dist < PointClickDistanceThreshold / Size.X)
                {
                    return index;
                }
            }

            return null;
        }

        protected override void Render(IGuiRenderer guiRenderer)
        {
            if (ImageName != "")
            {
                guiRenderer.Image(ImageName, Size, margin: Margin);
            }

            if (IsMouseDown)
            {
                guiRenderer.Line(new Vector2(0, 0), new Vector2(20, 20), Colours.Red);
            }

            if (IsMouseOver)
            {
                guiRenderer.Line(new Vector2(5, 0), new Vector2(25, 20), Colours.Green);
            }

            if (Points != null)
            {
                if (Points.Length > 1)
                {
                    for (var i = 0; i < Points.Length; i++)
                    {
                        var start = new Vector2(Points[i].X * Size.X, Points[i].Y * Size.Y);
                        var endIndex = i == Points.Length - 1 ? 0 : i + 1;
                        var end = new Vector2(Points[endIndex].X * Size.X, Points[endIndex].Y * Size.Y);

                        guiRenderer.Line(start, end, Colours.Yellow);
                    }
                }

                if (SelectedPointIndex.HasValue)
                {
                    var selectedPointPosition = Points[SelectedPointIndex.Value].Scale(Size.X, Size.Y);

                    guiRenderer.Rectangle(
                        selectedPointPosition.Translate(-PointSelectorSquareSize, -PointSelectorSquareSize),
                        selectedPointPosition.Translate(PointSelectorSquareSize, PointSelectorSquareSize),
                        Colours.Green);
                }
                else if (HoverPointIndex.HasValue)
                {
                    var hoverPointPosition = Points[HoverPointIndex.Value].Scale(Size.X, Size.Y);

                    guiRenderer.Rectangle(
                        hoverPointPosition.Translate(-PointSelectorSquareSize, -PointSelectorSquareSize),
                        hoverPointPosition.Translate(PointSelectorSquareSize, PointSelectorSquareSize),
                        Colours.LightBlue);
                }
            }

            base.Render(guiRenderer);
        }

        public static void Register(XmlLoader loader)
        {
            loader.RegisterCustomWidget(XmlNodeName, Create);
        }
    }

}