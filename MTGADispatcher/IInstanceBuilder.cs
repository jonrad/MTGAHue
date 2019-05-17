using MTGADispatcher.ClientModels;

namespace MTGADispatcher
{
    public interface IInstanceBuilder
    {
        Instance Build(InstanceModel model);
    }
}
