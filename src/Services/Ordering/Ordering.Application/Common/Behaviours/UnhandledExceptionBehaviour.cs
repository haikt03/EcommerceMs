using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Common.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Bọc request trong try-catch để bắt lỗi.
        /// Gọi next() để tiếp tục xử lý request.
        /// Nếu xảy ra lỗi, log lỗi với thông tin request.
        /// Ném lại exception để không nuốt mất lỗi.
        /// </summary>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(ex, "Application Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);
                throw;
            }
        }
    }
}
