using FluentValidation;
using Service.Entities.Requests;

namespace Service.Entities.Validators;

public class FaturaItemAddToFaturaRequestValidator : AbstractValidator<FaturaItemAddToFaturaRequest>
{
    public FaturaItemAddToFaturaRequestValidator()
    {
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é um campo obrigatório!")
            .MinimumLength(4).WithMessage("Descrição deve ter no mínimo 4 caracteres!")
            .MaximumLength(20).WithMessage("Descrição deve ter no máximo 20 caracteres");
    }
}
