using FluentValidation;
using Ordering.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using ValidationException = FluentValidation.ValidationException;

namespace Ordering.Application.Behaviours
{

    // HTTP Request → Command → [Exception Decorator] → [Validation Decorator] → Real Handler → Response
    // ✅ WITH decorator — Fluent validation is CENTRALIZED here, handlers stay clean
    public class ValidationCommandHandlerDecorator<TCommand,TResult> : ICommandHandler<TCommand,TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _inner;
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        public ValidationCommandHandlerDecorator(ICommandHandler<TCommand, TResult> inner,
            IEnumerable<IValidator<TCommand>> validators)
        {
            _inner = inner;
            _validators = validators;
        }
        public async Task<TResult> Handle(TCommand command, CancellationToken cancellationToken)
        {
            if(_validators.Any())
            {
                var context = new ValidationContext<TCommand>(command);
                 // Run ALL validators for this command in parallel
                var results = await Task.WhenAll(_validators.Select(v=>v.ValidateAsync(context,cancellationToken)));
                 // Collect all failures from all validators
                var failures = results.
                                SelectMany(r=>r.Errors)
                                .Where(f=>f !=null)
                                .ToList();
                if(failures.Any())
                {
                    throw new ValidationException(failures); // inner handler NOT called
                }
            }
            return await _inner.Handle(command, cancellationToken); // ✅ only runs if VALID
        }
    }
}
