using AutoMapper;
using Service.Entities.Models;
using Service.Entities.Responses;

namespace Service.Services.Utils;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FaturaModel, FaturaGetByIdResponse>();
        CreateMap<FaturaItemModel, FaturaItemFaturaGetByIdResponse>();
        CreateMap<FaturaModel, FaturaFaturaManagementGetAllResponse>();
        CreateMap<FaturaItemModel, FaturaItemsFaturaManagementGetAllResponse>();
    }
}
