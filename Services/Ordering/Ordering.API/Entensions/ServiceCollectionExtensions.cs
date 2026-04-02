using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions;
using Ordering.Application.Behaviours;
using Ordering.Application.Validators;
using Ordering.Core.Repositories;
using Ordering.Infrastucture.Data;
using Ordering.Infrastucture.Repositories;
using RabbitMQ.Client;

namespace Ordering.API.Entensions
{
    // instead of adding in program.cs DI code added here in Extensions
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1.Database
            services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(
                        configuration.GetConnectionString("OrderingConnectionString"),
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 5,
                                    maxRetryDelay: TimeSpan.FromSeconds(10),
                                    errorNumbersToAdd: null
                                );
                            // THIS IS THE FIX
                            //sqlOptions.MigrationsAssembly("Ordering.Infrastructure");
                            sqlOptions.MigrationsAssembly(typeof(OrderContext).Assembly.FullName);
                        }
                    );
            });

            //services.AddOrderingInfrastructure(configuration);

            //2 Repositories
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            //3 CQRS
            // without result i.e ICommandHandler<>
            services.Scan(scan => scan.FromAssemblies(typeof(ICommandHandler<>).Assembly)
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                // with result i.e ICommandHandler<,>
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()

                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            //4 Fluent Validation
            services.AddValidatorsFromAssembly(typeof(CreateOrderCommandValidator).Assembly);

            // 5. Decorators Pipeline
            services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
            services.Decorate(typeof(ICommandHandler<,>),typeof(UnhandledExceptionCommandHandlerDecorator<,>));

            return services;
        }
    }
}
