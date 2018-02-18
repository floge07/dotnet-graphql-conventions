using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public abstract class WrapperBase : IWrapper
    {
        protected WrapperBase(IWrapper parent)
        {
            _parent = parent;
        }

        protected WrapperBase()
        {
        }

        protected readonly IWrapper _parent;

        protected WrapperBase NextWrapper { get; private set; }

        public WrapperBase Next(WrapperBase unwrapper)
        {
            NextWrapper = unwrapper;
            return NextWrapper;
        }
        public object Wrap(GraphEntityInfo entity, GraphTypeInfo type, object value)
        {
            value = WrapValue(entity, type, value);
            return NextWrapper != null
                ? NextWrapper.Wrap(entity, type, value)
                : value;
        }

        public abstract object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value);

        protected string GetEntityDescription(GraphEntityInfo entity)
        {
            if (entity is GraphTypeInfo)
            {
                return "type";
            }
            if (entity is GraphFieldInfo)
            {
                return "field";
            }
            if (entity is GraphArgumentInfo)
            {
                return "argument";
            }
            return "entity";
        }
    }
}