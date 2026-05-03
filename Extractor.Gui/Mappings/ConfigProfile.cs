using System.Collections.Generic;
using AutoMapper;
using Extractor.Core.Model;
using Extractor.Gui.Models;

namespace Extractor.Gui.Mappings;

public class ConfigProfile : Profile
{
    public ConfigProfile()
    {
        CreateMap<CoreConfig, CoreConfigDto>().ReverseMap();
        CreateMap<TrackDenomination, TrackDenominationDto>()
            .ForMember(dest => dest.AliasesJoined, opt => opt.MapFrom(src => string.Join(',', src.Aliases)));
        CreateMap<TrackDenomination, TrackDenominationDto>().ReverseMap()
            .ForMember(dest => dest.Aliases, opt => opt.MapFrom(src => src.AliasesJoined.Split(',')));
        CreateMap<TracksData, TracksDataDto>().ReverseMap();
    }
}