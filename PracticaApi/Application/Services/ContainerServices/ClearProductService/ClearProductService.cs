// using Application.Common;
// using Application.Common.Interfaces.Repositories;
// using Application.Exceptions;
// using Application.ViewModels;
// using Domain.Authentications.Users;
// using Domain.Containers;
// using Optional;
//
// namespace Application.Services.ContainerServices.ClearProductService;
// public class ClearProductService : IClearProductService
// {
//     private readonly IContainerRepository _containerRepository;
//
//     public ClearProductService(IContainerRepository containerRepository)
//     {
//         _containerRepository = containerRepository;
//     }
//
//     public async Task<Result<ContainerVM, ContainerException>> ClearProductAsync(
//         Guid containerId,
//         CancellationToken cancellationToken)
//     {
//         var containerIdObj = new ContainerId(containerId);
//         var existingContainer = await _containerRepository.GetById(containerIdObj, cancellationToken);
//
//         return await existingContainer.Match(
//             async container =>
//             {
//                 try
//                 {
//                     container.ClearProduct(UserId.Empty);
//                     var updatedContainer = await _containerRepository.Update(container, cancellationToken);
//                     return new ContainerVM(updatedContainer);
//                 }
//                 catch (ContainerException exception)
//                 {
//                     return new ContainerUnknownException(containerIdObj, exception);
//                 }
//             },
//             () => Task.FromResult<Result<ContainerVM, ContainerException>>(new ContainerNotFoundException(containerIdObj))
//         );
//     }
// }