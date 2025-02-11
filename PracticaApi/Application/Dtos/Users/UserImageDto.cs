using Domain.Authentications.Users;

namespace Application.Dtos.Users;

public record UserImageDto(Guid? Id, string FilePath)
{
    public static UserImageDto FromDomainModel(UserImageEntity userImageEntity)
    => new(userImageEntity.Id.Value, userImageEntity.FilePath);
}