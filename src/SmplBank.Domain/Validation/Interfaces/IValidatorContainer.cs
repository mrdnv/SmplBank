namespace SmplBank.Domain.Validation.Interfaces
{
    public interface IValidatorContainer<T> where T : Entity.Entity
    {
        IValidator<T, TDto> GetValidator<TDto>() where TDto : class;
    }
}
