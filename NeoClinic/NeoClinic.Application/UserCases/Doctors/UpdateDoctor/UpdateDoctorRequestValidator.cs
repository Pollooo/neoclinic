using FluentValidation;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctor;

public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
{
    public UpdateDoctorRequestValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.FullNameUz).NotEmpty();
        RuleFor(x => x.BioUz).NotEmpty();
        RuleFor(x => x.SpecialtyUz).NotEmpty();
        RuleFor(x => x.FullNameRu).NotEmpty();
        RuleFor(x => x.BioRu).NotEmpty();
        RuleFor(x => x.SpecialtyRu).NotEmpty();
    }
}
