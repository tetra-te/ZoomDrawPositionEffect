using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace ZoomDrawPositionEffect.Effects
{
    [VideoEffect("拡大縮小（描画位置）", ["描画"], ["Zoom", "Drawing", "Position", "Coordinate"], isAviUtlSupported:false)]
    internal class ZoomDrawPositionEffect : VideoEffectBase
    {
        public override string Label => "拡大縮小（描画位置）";

        [Display(GroupName="拡大縮小（描画位置）", Name = "全体", Description = "全体の拡大率")]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Zoom { get; } = new Animation(100, -5000, 5000);
        [Display(GroupName = "拡大縮小（描画位置）", Name = "横方向", Description = "横方向の拡大率")]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation ZoomX { get; } = new Animation(100, -5000, 5000);
        [Display(GroupName = "拡大縮小（描画位置）", Name = "縦方向", Description = "縦方向の拡大率")]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation ZoomY { get; } = new Animation(100, -5000, 5000);
        [Display(GroupName = "拡大縮小（描画位置）", Name = "中心X", Description = "中心の位置（横方向）")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation CenterX { get; } = new Animation(0, -999999, 999999);
        [Display(GroupName = "拡大縮小（描画位置）", Name = "中心Y", Description = "中心の位置（縦方向）")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation CenterY { get; } = new Animation(0, -999999, 999999);
        [Display(GroupName = "拡大縮小（描画位置）", Name = "回転率", Description = "回転率")]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation RotationRatio { get; } = new Animation(0, -999999, 999999);
        [Display(GroupName = "拡大縮小（描画位置）", Name = "回転方向", Description = "回転の方向")]
        [EnumComboBox]
        public DirectionEnum Direction { get => direction; set => Set(ref direction, value); }
        DirectionEnum direction = DirectionEnum.Inward;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new ZoomDrawPositionEffectProcessor(this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Zoom, ZoomX, ZoomY, CenterX, CenterY, RotationRatio];
    }
}
