using Service.Business.Interfaces;
using Service.Entities.Requests;

namespace Service.Business.Business;

public class FaturaItemBusiness : IFaturaItemBusiness
{
    public bool CheckValueAndVerify(FaturaItemCreateRequest request)
    {
        if (request.Valor > 1000 && !request.VerificaValor)
        {
            return false;
        }

        return true;
    }

    public bool IsNegative(double valor)
    {
        if (valor > 0)
        {
            return false;
        }

        return true;
    }
}
