using FluentValidation;
using UdemyMinimalApi.Coupons.Models.DTOs;

namespace UdemyMinimalApi.Coupons.Validation
{
    public class CouponUpdateValidation: AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1,100);
        }
    }
}
