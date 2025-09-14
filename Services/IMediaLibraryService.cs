using Errors;
using Models;

namespace Services;

public interface IMediaLibraryService
{
  Task<(ArticleBlockModel? result, ServiceError? error)> Create(Stream stream, string contentType, string fileName);
}
