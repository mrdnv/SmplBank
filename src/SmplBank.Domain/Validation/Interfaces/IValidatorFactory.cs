namespace SmplBank.Domain.Validation.Interfaces
{
    public interface IValidatorFactory<T> where T : Entity.Entity
    {
        IValidator<T, TDto> Resolve<TDto>() where TDto : class;
    }
}
