using MTGADispatcher.ClientModels;

namespace MTGADispatcher.Specs.Fixtures
{
    public static class InstanceModelFixture
    {
        public static InstanceModel Build()
        {
            return new InstanceModel
            {
                Id = 1,
                CardId = 1
            };
        }

        public static InstanceModel WithColors(this InstanceModel instance, params string[] colors)
        {
            instance.Colors = colors;

            return instance;
        }

        public static InstanceModel WithCardTypes(this InstanceModel instance, params string[] types)
        {
            instance.CardTypes = types;

            return instance;
        }

        public static InstanceModel WithSubTypes(this InstanceModel instance, params string[] types)
        {
            instance.CardSubtypes = types;

            return instance;
        }
    }
}
