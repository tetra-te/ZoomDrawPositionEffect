using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace ZoomDrawPositionEffect.Effects
{
    internal class ZoomDrawPositionEffectProcessor(IGraphicsDevicesAndContext devices, ZoomDrawPositionEffect item) : VideoEffectProcessorBase(devices)
    {
        bool isFirst = true;
        double rotation;
        AffineTransform2DInterpolationMode interpolationMode;
        
        AffineTransform2D? transform2D;

        public override DrawDescription Update(EffectDescription effectDescription)
        {
            var drawDesc = effectDescription.DrawDescription;
            if (transform2D is null)
                return drawDesc;

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
            var interpolationMode = drawDesc.ZoomInterpolationMode.ToTransform2D();

            //拡大率が0だと自然な回転を行えない
            zoomX = zoomX == 0 ? 0.0000000001 : zoomX;
            zoomY = zoomY == 0 ? 0.0000000001 : zoomY;

            double xLength = zoomX * (drawDesc.Draw.X - centerX);
            double yLength = zoomY * (drawDesc.Draw.Y - centerY);
           
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
            rotation *= rotationRatio * Math.PI / 180d;

            if (isFirst || this.rotation != rotation)
            {
                transform2D.TransformMatrix = Matrix3x2.CreateRotation((float)rotation);
                this.rotation = rotation;
            }
            if (isFirst || this.interpolationMode != interpolationMode)
            {
                transform2D.InterPolationMode = interpolationMode;
                this.interpolationMode = interpolationMode;
            }               

            var control =
                new VideoEffectController(
                    item,
                    [
                        new ControllerPoint(
                            new((float)-xLength, (float)-yLength, 0f),
                            x=>
                            {
                                item.CenterX.AddToEachValues(x.Delta.X);
                                item.CenterY.AddToEachValues(x.Delta.Y);
                            })
                        ]);

            return
                drawDesc with
                {
                    Draw = new(
                    (float)(centerX + xLength),
                    (float)(centerY + yLength),
                    drawDesc.Draw.Z),
                    Controllers =
                    [
                        ..effectDescription.DrawDescription.Controllers,
                        control
                        ]
                };
        }
        
        protected override void setInput(ID2D1Image? input)
        {
            transform2D?.SetInput(0, input, true);
        }
        
        protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
        {
            transform2D = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(transform2D);
            var output = transform2D.Output;
            disposer.Collect(output);
            return output;
        }

        protected override void ClearEffectChain()
        {
            transform2D?.SetInput(0, null, true);
        }
    }
}