using System.Threading.Tasks;

namespace SmplBank.Domain.Validation.Interfaces
{
    public interface IValidator<in T, TDto>
        where T : Entity.Entity
        where TDto : class
    {
        Task<IValidatedObject<TDto>> ValidateAsync(TDto dto);
    }
}
