using FluentValidation;

namespace NeoClinic.Application.UserCases.Appointments.GetAppointments;

public class GetAppointmentsRequestValidator : AbstractValidator<GetAppointmentsRequest>
{
    public GetAppointmentsRequestValidator()
    {
        When(x => x.StartDate.HasValue, () =>
        {
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate!.Value)
                .WithMessage("EndDate must be greater than or equal to StartDate");
        });
    }
}
