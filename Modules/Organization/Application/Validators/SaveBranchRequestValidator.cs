using FluentValidation;
using verii_metivon_api.Modules.Organization.Application.Dtos;

namespace verii_metivon_api.Modules.Organization.Application.Validators;

public sealed class SaveBranchRequestValidator : AbstractValidator<SaveBranchRequest>
{
    public SaveBranchRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithErrorCode("Organization.Branch.CodeRequired")
            .MaximumLength(30).WithErrorCode("Organization.Branch.CodeLength");

        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("Organization.Branch.NameRequired")
            .MinimumLength(2).WithErrorCode("Organization.Branch.NameLength")
            .MaximumLength(150).WithErrorCode("Organization.Branch.NameLength");
    }
}

