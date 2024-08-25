using FluentValidation;
using UdemyMinimalApi.Coupons.Models.DTOs;

namespace UdemyMinimalApi.Coupons.Validation
{
    public class CouponCreateValidation: AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1,100);
        }
    }
}
