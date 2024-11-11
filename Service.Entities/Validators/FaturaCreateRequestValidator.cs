using FluentValidation;
using Service.Entities.Requests;

namespace Service.Entities.Validators;

public class FaturaCreateRequestValidator : AbstractValidator<FaturaCreateRequest>
{
    public FaturaCreateRequestValidator()
    {
        RuleFor(x => x.Cliente)
            .NotEmpty().WithMessage("Cliente é um campo obrigatório!")
            .MinimumLength(5).WithMessage("Cliente deve ter no mínimo 5 carateres!")
            .MaximumLength(50).WithMessage("Cliente deve ter no máximo 50 carateres!");

        RuleFor(x => x.Vencimento)
            .NotEmpty().WithMessage("Vencimento é um campo obrigatório!");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("Itens é um campo obrigatório!");

        RuleForEach(x => x.Itens).SetValidator(new FaturaItemCreateRequestValidator());
    }
}
