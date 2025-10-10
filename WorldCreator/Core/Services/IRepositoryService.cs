using WorldCreator.Model;

namespace WorldCreator.Core.Services;

/// <summary>
///     用于获取所有的仓库
/// </summary>
public interface IRepositoryService
{
    WorldRepository CurrentRepository { get; }
    SortableObservableCollection<WorldRepository> Repositories { get; set; }
    void Load();
    void OpenRepository(WorldRepository repository);
    void Import(string uri);
}