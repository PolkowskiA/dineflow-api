namespace DineFlow.Domain.Common.Exceptions
{
    public sealed class OrderCannotBeSubmittedException : DomainException
    {
        public OrderCannotBeSubmittedException(string reason)
            : base(reason)
        {
        }
    }
}
