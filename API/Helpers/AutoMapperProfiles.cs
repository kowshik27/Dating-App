using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, MemberDTO>()
        .ForMember(d => d.Age, o =>
        o.MapFrom(s => s.DateOfBirth.CalculateAge()))
        .ForMember(d => d.PhotoUrl, o =>
        o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));

        CreateMap<Message, MessageDTO>()
        .ForMember(d => d.SenderPhotoUrl, o =>
        o.MapFrom(s => s.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url))
        .ForMember(d => d.ReceiverPhotoUrl, o =>
        o.MapFrom(s => s.Receiver.Photos.FirstOrDefault(p => p.IsMain)!.Url));

        CreateMap<Photo, PhotoDTO>();
        CreateMap<MemberUpdateDTO, User>();
        CreateMap<RegisterDTO, User>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        // For getting UTC format dates from Sqlite
        CreateMap<DateTime, DateTime>()
        .ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        // When  Optional Dates are present
        CreateMap<DateTime?, DateTime?>()
        .ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}
