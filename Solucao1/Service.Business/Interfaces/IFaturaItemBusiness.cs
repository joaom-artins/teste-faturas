using Service.Entities.Requests;

namespace Service.Business.Interfaces;

public interface IFaturaItemBusiness
{
    bool CheckValueAndVerify(FaturaItemCreateRequest request);
    bool IsNegative(double valor);
}
