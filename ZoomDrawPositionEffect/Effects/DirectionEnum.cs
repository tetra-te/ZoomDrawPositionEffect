using System.ComponentModel.DataAnnotations;

namespace ZoomDrawPositionEffect.Effects
{
    public enum DirectionEnum
    {
        [Display(Name = "内向き", Description = "内向き")]
        Inward,
        [Display(Name = "外向き", Description = "外向き")]
        Outward,
        [Display(Name = "下向き", Description = "下向き")]
        Downward,
    }
}