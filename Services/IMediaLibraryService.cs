using Errors;
using Models;

namespace Services;

public interface IMediaLibraryService
{
  Task<(ArticleBlockModel? result, ServiceError? error)> Create(int blockId, Stream stream, string contentType, string fileName);
  Task<(FileData? result, ServiceError? error)> GetDocument(Guid documentId);
}
