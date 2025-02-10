using System.Net.Mime;
using Domain.Authentications.Users;

namespace Api.Dtos.Users;

public record UserImageDto(Guid? Id, string FilePath)
{
    public static UserImageDto FromDomainModel(UserImageEntity userImageEntity)
    => new(userImageEntity.Id.Value, userImageEntity.FilePath);
}