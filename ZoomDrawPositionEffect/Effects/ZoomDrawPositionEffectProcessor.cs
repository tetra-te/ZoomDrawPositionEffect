using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;

namespace ZoomDrawPositionEffect.Effects
{
    internal class ZoomDrawPositionEffectProcessor : IVideoEffectProcessor
    {
        readonly ZoomDrawPositionEffect item;
        ID2D1Image? input;

        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public ZoomDrawPositionEffectProcessor(ZoomDrawPositionEffect item)
        {
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var zoom = item.Zoom.GetValue(frame, length, fps) / 100;
            var zoomX = zoom * item.ZoomX.GetValue(frame, length, fps) / 100;
            var zoomY = zoom * item.ZoomY.GetValue(frame, length, fps) / 100;
            var centerX = item.CenterX.GetValue(frame, length, fps);
            var centerY = item.CenterY.GetValue(frame, length, fps);
            var rotationRatio = item.RotationRatio.GetValue(frame, length, fps) / 100;
            var direction = item.Direction;

            var drawDesc = effectDescription.DrawDescription;

            double xLength = zoomX * (drawDesc.Draw.X - centerX);
            double yLength = zoomY * (drawDesc.Draw.Y - centerY);

            if ((xLength == 0 && yLength == 0) || rotationRatio == 0)
            {
                return
                drawDesc with
                {
                    Draw = new(
                    (float)(centerX + xLength),
                    (float)(centerY + yLength),
                    drawDesc.Draw.Z)
                };
            }

            var theta = Math.Atan2(yLength, xLength) * 180 / Math.PI;
            double rotation = 0;

            switch (direction)
            {
                case DirectionEnum.Inward:
                    rotation = (xLength < 0 && yLength >= 0) ? theta - 270 : theta + 90;
                    break;
                case DirectionEnum.Outward:
                    rotation = (xLength < 0 && yLength < 0) ? theta + 270 : theta - 90;
                    break;
                case DirectionEnum.Downward:
                    rotation = (yLength >= 0) ? theta - 90 : theta + 90;
                    break;
            }
            rotation *= rotationRatio;

            return
                drawDesc with
                {
                    Draw = new(
                    (float)(centerX + xLength),
                    (float)(centerY + yLength),
                    drawDesc.Draw.Z),
                    Rotation = new(
                    0,
                    0,
                    drawDesc.Rotation.Z + (float)rotation)
                };
        }
        public void ClearInput()
        {
            input = null;
        }
        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public void Dispose()
        {

        }

    }
}